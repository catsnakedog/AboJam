using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.Licensing;
using UnityEngine;
using UnityEngine.Events;

public class HP : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spr_empty;
    public SpriteRenderer spr_max;
    public GameObject entity; // HP 모듈을 적용할 대상

    /* Field & Property */
    [SerializeField] private float HP_MAX = 100f; // 최대 체력 (초기화에만 사용됨)
    [SerializeField] private float speed = 0.02f; // 체력바 속도
    [SerializeField] private bool HideFullHP = false; // 최대 체력일 때 체력바 숨기기
    public static List<HP> instances = new List<HP>();
    public UnityEvent death; // HP 가 0 이 되었을 때 작동할 메서드
    public float HP_max { get; private set; }
    public float HP_current { get; private set; }
    public float HP_ratio { get; private set; }
    private Material material; // spr_max 를 우측부터 자르기 위한 머터리얼

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        material = spr_max.material;
        spr_max.sortingOrder = spr_empty.sortingOrder + 1;
        CheckVisible();
        death.AddListener(() => { Debug.Log("체력이 0 에 도달했습니다."); });

        if (HP_MAX <= 0)
        {
            Debug.Log($"{entity.name} 의 체력이 0 이하로 설정되어 있습니다.");
            enabled = false;
            return;
        }
        HP_max = HP_MAX;
        HP_current = HP_MAX;
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
        float HP_ratio_new = HP_current / HP_max;
        if (HP_ratio == HP_ratio_new) return;
        HP_ratio = Mathf.Lerp(material.GetFloat("_Fill"), HP_ratio_new, speed);
        material.SetFloat("_Fill", HP_ratio);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

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
        HP_current = Mathf.Min(HP_current + value, HP_max);
        if (HP_current == HP_max) CheckVisible();
    }
    /// <summary>
    /// 최대 체력을 조정합니다.
    /// </summary>
    /// <param name="value">최대 체력</param>
    public void SetMaxHP(float value)
    {
        if (value <= 0) { Debug.Log($"최대 체력은 0 이하로 설정할 수 없습니다,"); return; }
        if (HP_current > value) { HP_current = value; CheckVisible(); }
        HP_max = value;
    }

    /* Private Method */
    /// <summary>
    /// <br>현재 상태에 따라 체력바를 보이게 / 안보이게 합니다.</br>
    /// </summary>
    private void CheckVisible()
    {
        // 최대 체력이면 체력바 숨김 On / Off
        if (HideFullHP == true && HP_current == HP_max)
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