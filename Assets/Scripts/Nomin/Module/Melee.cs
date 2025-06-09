using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Melee : RecordInstance<Table_Melee, Record_Melee>
{
    /* Dependency */
    public Targeter targeter;
    public GameObject effect; // 피격 이펙트, 없어도 작동
    private Pool pool => Pool.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance;

    /* Field & Property */
    public static List<Melee> instances = new List<Melee>(); // 모든 Melee 인스턴스
    [SerializeField] private string[] clashTags; public string[] ClashTags { get { Start(); return clashTags; } set => clashTags = value; } // 충돌 대상 태그
    [SerializeField] private int penetrate; // 최대 피격 수
    [SerializeField] public float radius; // 피격 반지름
    [SerializeField] public float damage; // 공격 데미지
    [SerializeField] public float multiplierDamage = 1.0f; // 데미지 계수
    [SerializeField] public float knockback = 0.1f; // 넉백
    [SerializeField] public float multiplierKnockback = 1.0f; // 넉백 계수
    [SerializeField] private float effectTime; // 이펙트 지속 시간
    [SerializeField] float knockbackTime = 0.25f; // 넉백 지속 시간
    [SerializeField] float updateKnockback = 0.016f; // 넉백 업데이트 시간
    private WaitForSeconds waitForSeconds;
    private WaitForSeconds waitForSecondsUpdateKnockback;
    public static Action<string, Vector2> eventAttack;
    public static Action<string, Vector2> eventHit;

    /* Intializer & Finalizer */
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
        database_abojam.ExportMelee(initialRecords[0].ID, ref clashTags, ref penetrate, ref radius, ref damage, ref effectTime, ref knockback);

        waitForSeconds = new WaitForSeconds(effectTime);
        waitForSecondsUpdateKnockback = new WaitForSeconds(updateKnockback);
    } // Import 시 자동 실행

    /* Public Method */
    /// <summary>
    /// 지정한 위치에 원형 범위의 공격을 시전합니다.
    /// </summary>
    public void Attack(Vector3 worldPos)
    {
        eventAttack.Invoke(initialRecords[0].ID, gameObject.transform.position);

        // 범위 내의 모든 타겟 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, radius);
        int currentPenetrate = penetrate; // 현재 남은 관통 수

        // 태그 검사 후 데미지 적용
        foreach (Collider2D collider in colliders)
        {
            if (currentPenetrate < 1) break;
            if (clashTags.Contains(collider.tag))
            {
                eventHit.Invoke(initialRecords[0].ID, gameObject.transform.position);
                if (effect != null) StartCoroutine(CorEffect(collider.transform.position));
                HP.FindHP(collider.gameObject).Damage(damage * multiplierDamage); currentPenetrate--;
                if (new string[] {"Player", "Enemies"}.Contains(collider.tag)) Knockback(collider, knockback * multiplierKnockback);
                Debug.Log("피해 : " + damage * multiplierDamage);
            }
        }

        StartCoroutine(Draw(worldPos, radius, 0.5f));
    }
    /// <summary>
    /// 지정한 위치에 원형 범위의 공격을 시전합니다.
    /// </summary>
    public void Attack(Vector3 worldPos, float radius, float damage,  float knockback)
    {
        eventAttack.Invoke(initialRecords[0].ID, gameObject.transform.position);

        // 범위 내의 모든 타겟 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, radius);
        int currentPenetrate = penetrate; // 현재 남은 관통 수

        // 태그 검사 후 데미지 적용
        foreach (Collider2D collider in colliders)
        {
            if (currentPenetrate < 1) break;
            if (clashTags.Contains(collider.tag))
            {
                eventHit.Invoke(initialRecords[0].ID, gameObject.transform.position);
                if (effect != null) StartCoroutine(CorEffect(collider.transform.position));
                HP.FindHP(collider.gameObject).Damage(damage * multiplierDamage); currentPenetrate--;
                if (new string[] { "Player", "Enemies" }.Contains(collider.tag)) Knockback(collider, knockback * multiplierKnockback);
            }
        }

        StartCoroutine(Draw(worldPos, radius, 0.5f));
    }
    /* Private Method */
    /// <summary>
    /// effectTime 만큼 이펙트를 출력시킵니다.
    /// </summary>
    private IEnumerator CorEffect(Vector3 pos)
    {
        GameObject effect = pool.Get(this.effect.name);
        effect.transform.position = pos;
        yield return waitForSeconds;
        pool.Return(effect);
    }
    /// <summary>
    /// <br>지정한 반지름의 원을 일정 시간동안 그립니다.</br>
    /// </summary>
    private IEnumerator Draw(Vector3 worldPos, float radius, float seconds)
    {
        LineRenderer lineRenderer = new GameObject("Circle").AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 50;
        lineRenderer.sortingLayerName = "Entity";
        lineRenderer.sortingOrder = 0;

        for (int i = 0; i < 50; i++)
        {
            float angle = i * (360f / 50) * Mathf.Deg2Rad;
            float x = worldPos.x + Mathf.Cos(angle) * radius;
            float y = worldPos.y + Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, worldPos.z));
        }

        yield return new WaitForSeconds(seconds);
        Destroy(lineRenderer.gameObject);
    }

    /// <summary>
    /// 피격 대상을 무기의 반대 방향으로 일정 거리 이동시킵니다.
    /// </summary>
    private void Knockback(Collider2D collider, float knockback)
    {
        // 보스 몬스터면 넉백을 무시합니다.
        try { if (Enum.IsDefined(typeof(EnumData.SpecialLevel), collider.GetComponent<IEnemy>().Level)) return; }
        catch (Exception) { }

        StartCoroutine(CorKnockback(collider, knockback));
    }
    private IEnumerator CorKnockback(Collider2D collider, float knockback)
    {
        float elapsedTime = 0f;

        while (elapsedTime < knockbackTime)
        {
            elapsedTime += updateKnockback;
            float ratio = elapsedTime / knockbackTime;
            if (ratio > 1) ratio = 1;

            Vector2 direction = (collider.transform.position - transform.position).normalized;
            float force = Mathf.Lerp(knockback, 0, ratio);
            collider.transform.position += (Vector3)(direction * force);

            yield return waitForSecondsUpdateKnockback;
        }
    }
}