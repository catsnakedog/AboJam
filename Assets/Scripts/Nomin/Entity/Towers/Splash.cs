using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Splash : Tower<Table_Splash, Record_Splash>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    [SerializeField] private string ID; // Primary Key

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
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);
        launcher.eventAfterLoad += SetIndicatorSize;
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportSplash(ID, ref reinforceCost, ref delay, ref detection);
        base.Load();

        MaxLevel = ReinforceCost.Length;
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
        if (OnOff == true)
        {
            if (corFire != null) { StopCoroutine(corFire); corFire = null; }
            corFire = StartCoroutine(CorFire());
        }
        else { StopCoroutine(corFire); corFire = null; }
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
        else launcher.projectile = projectile;
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.3f));

        while (true)
        {
            GameObject temp = pool.Get(launcher.projectile.name);
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.Near, temp.GetComponent<Projectile>().ClashTags, detection);
            pool.Return(temp);

            if (target != null)
            {
                launcher.Launch(Targeter.TargetType.Near, detection);
            }

            yield return delay_waitForSeconds;
        }
    }
    private void SetIndicatorSize()
    {
        float scale = launcher.range * 2;
        indicator_circle.transform.localScale = new Vector2(scale, scale);
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
