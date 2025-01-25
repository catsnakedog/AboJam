using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static ObjectPool;

public class Auto : Tower<Table_Auto, Record_Auto>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    [SerializeField] private string ID; // Primary Key

    /* Field & Property */
    public static List<Auto> instances = new List<Auto>(); // 모든 연사 타워 인스턴스

    [Header("[ Auto ]")]
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float detection = 5f; // 적 감지 범위
    public float angle = 90f;
    public int subCount = 0;
    public int subCountPlus = 1;
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corFire;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);
            
        float scale = launcher.range * 4;
        indicator_circle.transform.localScale = new Vector2(scale, scale);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportAuto(ID, ref reinforceCost, ref delay, ref detection, ref angle, ref subCount, ref subCountPlus);
        base.Load();

        MaxLevel = ReinforceCost.Length;
        delay_waitForSeconds = new WaitForSeconds(delay);
        Fire(true);
    } // 풀에서 꺼낼 때 / Import 시 자동 실행
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
        subCount += subCountPlus;
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        while (true)
        {
            GameObject temp = pool.Get(launcher.projectile.name);
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.Near, temp.GetComponent<Projectile>().ClashTags, detection);
            pool.Return(temp);

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
