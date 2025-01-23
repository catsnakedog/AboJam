using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.Licensing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class HP : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spr_empty;
    public SpriteRenderer spr_max;
    public GameObject entity; // HP 모듈을 적용할 대상
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    [SerializeField] private string ID; // Primary Key

    /* Field & Property */
    public static List<HP> instances = new List<HP>();
    [SerializeField] private float speed = 0.02f; // 체력바 속도
    [SerializeField] private bool hideFullHp = true; // 최대 체력일 때 체력바 숨기기
    [SerializeField] private float hp_max = 100f; /* 초기 최대 체력 */ public float Hp_max { get; private set; } /* 현재 최대 체력 */
    public float HP_current { get; private set; } // 현재 체력
    public float HP_ratio { get; private set; } // 현재 체력 비율
    public UnityEvent death; // HP 가 0 이 되었을 때 작동할 메서드
    private Material material; // spr_max 를 우측부터 자르기 위한 머터리얼

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        Load();
        material = spr_max.material;
        spr_max.sortingOrder = spr_empty.sortingOrder + 1;
        death.AddListener(() => { Debug.Log($"{entity.name} 의 체력이 0 에 도달했습니다."); });
    }
    private void FixedUpdate()
    {
        UpdateHP();
    }
    /// <summary>
    /// UI 상의 HP 를 업데이트 합니다.
    /// </summary>
    /// <param name="HP_ratio"></param>
    private void UpdateHP()
    {
        // HP 비율에 변동이 생겼을 때만 동작
        float HP_ratio_new = HP_current / Hp_max;
        if (Mathf.Abs(HP_ratio - HP_ratio_new) < 0.01f) return;

        // UI 업데이트
        HP_ratio = Mathf.Lerp(material.GetFloat("_Fill"), HP_ratio_new, speed);
        material.SetFloat("_Fill", HP_ratio);

        // UI 업데이트 종료 시 Visble 체크
        if (Mathf.Abs(HP_ratio - HP_ratio_new) < 0.01f) CheckVisible();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        database_abojam.ExportHP(ID, ref hp_max, ref hideFullHp);

        Hp_max = hp_max;
        HP_current = hp_max;
        CheckVisible();
    } // Import 시 자동 실행

    /* Public Method */
    /// <summary>
    /// 피해를 입습니다.
    /// </summary>
    /// <param name="value">피해량</param>
    public void Damage(float value)
    {
        if (HP_current == 0) return;
        HP_current = Mathf.Max(HP_current - value, 0);
        if (HP_current == 0) { death?.Invoke(); }
        CheckVisible();
    }
    /// <summary>
    /// 체력을 회복합니다.
    /// </summary>
    /// <param name="value">회복량</param>
    public void Heal(float value)
    {
        HP_current = Mathf.Min(HP_current + value, Hp_max);
    }
    /// <summary>
    /// 최대 체력을 조정합니다.
    /// </summary>
    /// <param name="value">최대 체력</param>
    public void SetMaxHP(float value)
    {
        if (value <= 0) { Debug.Log($"최대 체력은 0 이하로 설정할 수 없습니다,"); return; }
        if (HP_current > value) { HP_current = value; }
        Hp_max = value;
    }
    /// <summary>
    /// 타겟 오브젝트와 연결된 HP 모듈을 탐색합니다.
    /// </summary>
    /// <param name="target">발사체와 충돌한 오브젝트</param>
    /// <returns>타겟 오브젝트의 HP</returns>
    public static HP FindHP(GameObject target)
    {
        foreach (var HP in HP.instances)
        {
            if (HP.entity == target) return HP;
        }

        Debug.Log($"{target.name} 과 연결된 HP 가 존재하지 않습니다.");
        return null;
    }

    /* Private Method */
    /// <summary>
    /// <br>현재 상태에 따라 체력바를 보이게 / 안보이게 합니다.</br>
    /// </summary>
    private void CheckVisible()
    {
        // 최대 체력이면 체력바 숨김 On / Off
        if (hideFullHp == true && HP_current == Hp_max)
        {
            spr_empty.enabled = false;
            spr_max.enabled = false;
        }
        else
        {
            spr_empty.enabled = true;
            spr_max.enabled = true;
        }
    }

    /* Test Method */
    [ContextMenu("DamageTest50")]
    public void DamageTest50()
    {
        Damage(50f);
        Debug.Log($"Current HP : {HP_current}");
    }
    [ContextMenu("DamageTest10")]
    public void DamageTest10()
    {
        Damage(10f);
        Debug.Log($"Current HP : {HP_current}");
    }
    [ContextMenu("HealTest10")]
    public void HealTest10()
    {
        Heal(10f);
        Debug.Log($"Current HP : {HP_current}");
    }
    [ContextMenu("HealTest50")]
    public void HealTest50()
    {
        Heal(50f);
        Debug.Log($"Current HP : {HP_current}");
    }
    [ContextMenu("VisibleTest")]
    public void VisibleTest()
    {
        CheckVisible();
        Debug.Log($"Current HP : {HP_current}");
    }
}