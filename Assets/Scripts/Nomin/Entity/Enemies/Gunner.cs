using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : Enemy<Table_Gunner, Record_Gunner>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Launcher launcher;
    public GameObject indicator_circle;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Move move;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Gunner> instances = new List<Gunner>(); // 모든 거너 인스턴스

    [Header("[ Gunner ]")]
    [SerializeField] private float delay = 0.8f; // 총 공격 딜레이
    [SerializeField] private float delay_fire = 0.05f; // 사격 간격
    [SerializeField] private float delay_anim = 0.3f; // 애니메이션 대기
    [SerializeField] private float delay_attack = 1f; // 스폰 후 N 초 후 공격 시작
    public float detection = 5f; // 적 감지 범위
    public int subCount = 2;
    private bool isFire = true;
    private Coroutine corFire;
    private WaitForSeconds delay_waitForSeconds;
    private WaitForSeconds delay_waitForSecondsFire;
    private WaitForSeconds delay_waitForSecondsAnim;
    private WaitForSeconds delay_waitForSecondsAttack;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportGunner(initialRecords[0].ID, ref delay, ref delay_fire, ref detection, ref subCount);
        base.Load();
        launcher.Load();

        animator.enabled = true;
        move.isMove = true;
        delay_waitForSeconds = new WaitForSeconds(delay - (delay_fire * subCount) - delay_anim);
        delay_waitForSecondsFire = new WaitForSeconds(delay_fire);
        delay_waitForSecondsAnim = new WaitForSeconds(delay_anim);
        delay_waitForSecondsAttack = new WaitForSeconds(delay_attack);

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
        isFire = OnOff;
        if (corFire != null) StopCoroutine(corFire);
        if (OnOff == true) corFire = StartCoroutine(CorFire());
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
    /// 레벨업 시 능력치 상승을 정의합니다.
    /// </summary>
    public override void Reinforce()
    {
        hp.SetMaxHP(hp.Hp_max * 1.25f);
        subCount++;
        base.Reinforce();
    }
    /// <summary>
    /// 보스 몬스터로 승격됩니다.
    /// </summary>
    public override void Promotion(EnumData.SpecialLevel level)
    {
        launcher.range *= 1.15f;

        switch (level)
        {
            case EnumData.SpecialLevel.FirstBoss:
                hp.SetMaxHP(2000);
                subCount += 45;
                SetDelay(0.01f);
                break;
            case EnumData.SpecialLevel.SecondsBoss:
                hp.SetMaxHP(9999);
                SetDelay(0.01f);
                break;
            case EnumData.SpecialLevel.ThirdBoss:
                hp.SetMaxHP(99999);
                SetDelay(0.01f);
                break;
        }

        base.Promotion(level);
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        yield return delay_waitForSecondsAttack;

        while (isFire == true)
        {
            GameObject temp = pool.Get(launcher.projectile.name);
            GameObject target = launcher.targeter.Targetting(Targeter.TargetType.Near, temp.GetComponent<Projectile>().ClashTags, detection);
            pool.Return(temp);

            if (target != null)
            {
                // 공격 애니메이션 재생
                Flip(target.transform.position);
                animator.Play("Attack", 0, 0f);
                move.isMove = false;
                yield return delay_waitForSecondsAnim;

                // 메인 탄환
                launcher.align = true;
                launcher.Launch(Targeter.TargetType.Near, detection);
                launcher.align = false;

                // 서브 탄환
                for (int i = 1; i <= subCount; i++)
                {
                    yield return delay_waitForSecondsFire;
                    launcher.Launch(Targeter.TargetType.Near, detection);
                }

                move.isMove = true;
            }

            yield return delay_waitForSeconds;
        }
    }
    /// <summary>
    /// 타겟 기준으로 스프라이트를 뒤집습니다.
    /// </summary>
    private void Flip(Vector3 destination)
    {
        // 왼쪽 바라보기
        if (destination.x < transform.position.x)
        {
            spriteRenderer.flipX = false;

            // 런처의 스프라이트 & 위치 뒤집기
            launcher.spriteRenderer.flipX = false;
            Vector3 launcherLocalPos = launcher.transform.localPosition;
            launcher.transform.localPosition = new Vector3(-Mathf.Abs(launcherLocalPos.x), launcherLocalPos.y, launcherLocalPos.z);
        }
        // 오른쪽 바라보기
        else
        {
            spriteRenderer.flipX = true;

            // 런처의 스프라이트 & 위치 뒤집기
            launcher.spriteRenderer.flipX = true;
            Vector3 launcherLocalPos = launcher.transform.localPosition;
            launcher.transform.localPosition = new Vector3(Mathf.Abs(launcherLocalPos.x), launcherLocalPos.y, launcherLocalPos.z);
        }
    }
}