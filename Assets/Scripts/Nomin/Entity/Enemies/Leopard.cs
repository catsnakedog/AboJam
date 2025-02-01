using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem.HID;
using static Targeter;
using static UnityEngine.GraphicsBuffer;

public class Leopard : Enemy<Table_Leopard, Record_Leopard>, IPoolee
{
    /* Dependency */
    [Header("[ Dependency ]")]
    public Melee melee;
    public GameObject indicator_circle;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Move move;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Leopard> instances = new List<Leopard>(); // 모든 표범 인스턴스

    [Header("[ Leopard ]")]
    [SerializeField] private float delay = 0.8f; // 공격 딜레이
    [SerializeField] private float delay_anim = 0.3f; // 애니메이션 대기
    public float detection = 1f; // 적 감지 범위
    private WaitForSeconds delay_waitForSeconds;
    private WaitForSeconds delay_waitForSecondsAnim;
    private bool isAttack = true;
    private Coroutine corAttack;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);

        // 인디케이터 스케일링
        float scale = detection * 4;
        indicator_circle.transform.localScale = new Vector2(scale, scale);

        Attack(true);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportLeopard(initialRecords[0].ID, ref delay, ref detection);
        base.Load();

        animator.enabled = true;
        move.isMove = true;
        delay_waitForSeconds = new WaitForSeconds(delay - delay_anim);
        delay_waitForSecondsAnim = new WaitForSeconds(delay_anim);

        Attack(true);
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
    public void Attack(bool OnOff)
    {
        isAttack = OnOff;
        if (OnOff == true) corAttack = StartCoroutine(CorAttack());
        else StopCoroutine(corAttack);
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

    /* Private Method */
    /// <summary>
    /// 공격을 개시합니다.
    /// </summary>
    private IEnumerator CorAttack()
    {
        while (isAttack == true)
        {
            GameObject target = melee.targeter.Targetting(Targeter.TargetType.Near, melee.ClashTags, detection);

            if (target != null)
            {
                // 공격 애니메이션 재생
                Flip(target.transform.position);
                animator.Play("Attack", 0, 0f);
                move.isMove = false;
                yield return delay_waitForSecondsAnim;

                // 공격 코드
                melee.Attack(target.transform.position);

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
        if (destination.x < transform.position.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }
}