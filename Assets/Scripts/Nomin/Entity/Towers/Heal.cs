using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Tower<Table_Heal, Record_Heal>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    [SerializeField] private string ID; // Primary Key

    /* Field & Property */
    public static List<Heal> instances = new List<Heal>(); // 모든 회복 타워 인스턴스
    public static Action<Vector3> eventFire;

    [Header("[ Heal ]")]
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float detection = 5f; // 아군 감지 범위
    public float ratio = 0.8f; // 체력 회복 기준 비율
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corHeal;
    private string path_projectile = $"Prefabs/Entity/Projectiles/Projectile_Heal";

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);

        // 인디케이터 스케일링
        float scale = launcher.range * 2;
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
        database_abojam.ExportHeal(ID, ref reinforceCost, ref delay, ref detection, ref ratio);
        base.Load();

        MaxLevel = ReinforceCost.Length;
        delay_waitForSeconds = new WaitForSeconds(delay);
        Healing(true);
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save()
    {
        base.Save();
    } // 풀에 집어 넣을 때 자동 실행

    /* Public Method */
    /// <summary>
    /// 인접한 타워에게 치유를 개시합니다.
    /// </summary>
    /// <param name="OnOff">치유 모드 On / Off</param>
    public void Healing(bool OnOff)
    {
        if (OnOff == true)
        {
            if (corHeal != null) { StopCoroutine(corHeal); corHeal = null; }
            corHeal = StartCoroutine(CorHeal());
        }
        else { StopCoroutine(corHeal); corHeal = null; }
    }
    /// <summary>
    /// 치유 딜레이를 재설정 합니다.
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
    /// 회복을 개시합니다.
    /// </summary>
    private IEnumerator CorHeal()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.3f));

        while (true)
        {
            GameObject temp = pool.Get(launcher.projectile.name);
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.LowHP, temp.GetComponent<Projectile>().ClashTags, detection, ratio);
            pool.Return(temp);
            if (target != null)
            {
                eventFire.Invoke(gameObject.transform.position);
                launcher.Launch(Targeter.TargetType.LowHP, detection, ratio);
            }

            yield return delay_waitForSeconds;
        }
    }

    /* Test Method */
    [ContextMenu("FireOnTest")]
    private void FireOnTest()
    {
        Healing(true);
    }
    [ContextMenu("FireOffTest")]
    private void FireOffTest()
    {
        Healing(false);
    }
}
