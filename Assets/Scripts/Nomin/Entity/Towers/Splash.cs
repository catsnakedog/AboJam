using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : Tower
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;

    /* Field & Property */
    public static List<Splash> instances = new List<Splash>(); // 모든 연사 타워 인스턴스

    [Header("[ Splash ]")]
    [SerializeField] private float delay = 1f; // 공격 딜레이
    public float detection = 8f; // 적 감지 범위
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corFire;
    private string path_projectile = $"Prefabs/Entity/Projectiles/Projectile_Splash";

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        base.Start();
        instances.Add(this);
        delay_waitForSeconds = new WaitForSeconds(delay);

        // 인디케이터 스케일링
        float scale = launcher.range * 4;
        indicator_circle.transform.localScale = new Vector2(scale, scale);

        Fire(true);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

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
    /// <br>발사체가 업그레이드 됩니다.</br>
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();

        // 상위 발사체로 변경
        GameObject projectile = Resources.Load<GameObject>(path_projectile + Level);
        if (projectile == null) { Debug.Log($"{path_projectile + Level} 가 존재하지 않습니다."); return; }
        else launcher.SetProjectile(projectile);
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
            if (target != null) launcher.Launch(Targeter.TargetType.Near, detection);

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
