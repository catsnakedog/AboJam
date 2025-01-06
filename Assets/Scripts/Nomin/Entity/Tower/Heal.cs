using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Tower
{
    /* Field & Property */
    public static List<Heal> instances = new List<Heal>(); // 모든 회복 타워 인스턴스
    public Launcher launcher;
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float detection = 5f; // 아군 감지 범위
    public float ratio = 0.8f; // 체력 회복 기준 비율
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corHeal;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        base.Start();
        instances.Add(this);
        delay_waitForSeconds = new WaitForSeconds(delay);

        Healing(true);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 인접한 적에게 공격을 개시합니다.
    /// </summary>
    /// <param name="OnOff">치유 모드 On / Off</param>
    public void Healing(bool OnOff)
    {
        if (OnOff == true) corHeal = StartCoroutine(CorHeal());
        else StopCoroutine(corHeal);
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
    /// 타워를 증강합니다.
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
    }

    /* Private Method */
    /// <summary>
    /// 회복을 개시합니다.
    /// </summary>
    private IEnumerator CorHeal()
    {
        while (true)
        {
            launcher.Launch(Targeter.TargetType.LowHP, detection, ratio);
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
