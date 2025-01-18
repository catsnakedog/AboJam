using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Auto : Tower, IScriptableObject<SO_Auto>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    [SerializeField] private SO_Auto so; public SO_Auto SO { get => so; set => so = value; }

    /* Field & Property */
    public static List<Auto> instances = new List<Auto>(); // 모든 연사 타워 인스턴스

    [Header("[ Auto ]")]
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float detection = 5f; // 적 감지 범위
    public float angle = 90f;
    public int subCount = 0;
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corFire;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        base.Start();
        instances.Add(this);
        Load();
            
        // 인디케이터 스케일링
        float scale = launcher.range * 4;
        indicator_circle.transform.localScale = new Vector2(scale, scale);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        base.Load();

        delay = SO.delay;
        detection = SO.detection;
        angle = SO.angle;
        subCount = SO.subCount;

        delay_waitForSeconds = new WaitForSeconds(delay);
        Fire(true);
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save()
    {
        base.Save();
    } // 풀에 집어 넣을 때 자동 실행

    /* Public Method */
    /// <summary>
    /// 인접한 적에게 공격을 개시합니다.
    /// </summary>
    /// <param name="OnOff">공격 모드 On / Off</param>
    public void Fire(bool OnOff)
    {
        if (OnOff == true) corFire = StartCoroutine(CorFire());
        else StopCoroutine(corFire);
    }
    /// <summary>
    /// 공격 딜레이를 재설정 합니다.
    /// </summary>
    /// <param name="delay">딜레이</param>
    public void SetDelay(float delay)
    {
        this.delay = delay;
        delay_waitForSeconds = new WaitForSeconds(delay);
    }
    /// <summary>
    /// <br>타워를 증강합니다.</br>
    /// <br>사격 시 발사체가 2 발 추가됩니다.</br>
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
        subCount++;
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        while (true)
        {
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.Near, launcher.Projectile.GetComponent<Projectile>().clashTags, detection);

            if (target != null)
            {
                // 메인 탄환
                launcher.align = true;
                launcher.Launch(Targeter.TargetType.Near, detection);
                launcher.align = false;

                // 서브 탄환
                for (int i = 1; i <= subCount; i++)
                {
                    float currentAngle = ((angle / 2) / subCount) * i;

                    launcher.Launch(Targeter.TargetType.Near, detection, angle: currentAngle);
                    launcher.Launch(Targeter.TargetType.Near, detection, angle: -currentAngle);
                }
            }

            yield return delay_waitForSeconds;
        }
    }

    /* Test Method */
    [ContextMenu("FireOnTest")]
    private void FireOnTest()
    {
        Fire(true);
    }
    [ContextMenu("FireOffTest")]
    private void FireOffTest()
    {
        Fire(false);
    }
}
