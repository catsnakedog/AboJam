using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto : Tower
{
    /* Field & Property */
    public static List<Auto> instances = new List<Auto>(); // 모든 연사 타워 인스턴스
    public Launcher launcher;
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float detection = 5f; // 적 감지 범위
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corFire;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        delay_waitForSeconds = new WaitForSeconds(delay);

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
    /// 타워를 증강합니다.
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        while (true)
        {
            launcher.Launch(Targeter.TargetType.Near, detection);
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
