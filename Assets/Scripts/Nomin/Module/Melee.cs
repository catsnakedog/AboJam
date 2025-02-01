using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderData;
using static UnityEngine.GraphicsBuffer;

public class Melee : RecordInstance<Table_Projectile, Record_Projectile>
{
    public Targeter targeter;

    /* Field & Property */
    public static List<Melee> instances = new List<Melee>(); // 모든 Melee 인스턴스
    [SerializeField] private string[] clashTags; public string[] ClashTags { get { Start(); return clashTags; } set => clashTags = value; } // 충돌 대상 태그
    [SerializeField] private int penetrate; // 최대 피격 수
    [SerializeField] private float radius; // 피격 반지름
    [SerializeField] private float delay; // 공격 전 차징 시간
    [SerializeField] private float damage; // 공격 데미지
    private WaitForSeconds waitForSeconds;

    /* Intializer & Finalizer */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);

        waitForSeconds = new WaitForSeconds(delay);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 지정한 위치에 원형 범위의 공격을 시전합니다.
    /// </summary>
    public IEnumerator Attack(Vector3 worldPos)
    {
        // 에디터용 드로잉 & 차징
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(worldPos, radius);
        yield return waitForSeconds;

        // 범위 내의 모든 타겟 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, radius);
        int currentPenetrate = penetrate; // 현재 남은 관통 수

        // 태그 검사 후 데미지 적용
        foreach (Collider2D collider in colliders)
        {
            if (currentPenetrate < 1) break;
            if (clashTags.Contains(collider.tag)) { HP.FindHP(collider.gameObject).Damage(damage); currentPenetrate--; }
        }
    }
}