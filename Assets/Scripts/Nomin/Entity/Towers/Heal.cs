using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Tower, IScriptableObject<SO_Heal>
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    [SerializeField] private SO_Heal so; public SO_Heal SO { get => so; set => so = value; }

    /* Field & Property */
    public static List<Heal> instances = new List<Heal>(); // 모든 회복 타워 인스턴스

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
        ratio = SO.ratio;

        delay_waitForSeconds = new WaitForSeconds(delay);
        Healing(true);
    }

    /* Public Method */
    /// <summary>
    /// 인접한 타워에게 치유를 개시합니다.
    /// </summary>
    /// <param name="OnOff">치유 모드 On / Off</param>
    public void Healing(bool OnOff)
    {
        if (OnOff == true) corHeal = StartCoroutine(CorHeal());
        else StopCoroutine(corHeal);
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
        else launcher.SetProjectile(projectile);
    }

    /* Private Method */
    /// <summary>
    /// 회복을 개시합니다.
    /// </summary>
    private IEnumerator CorHeal()
    {
        while (true)
        {
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.LowHP, launcher.Projectile.GetComponent<Projectile>().clashTags, detection, ratio);
            if (target != null) launcher.Launch(Targeter.TargetType.LowHP, detection, ratio);

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
