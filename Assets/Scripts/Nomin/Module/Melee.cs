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

public class Melee : RecordInstance<Table_Melee, Record_Melee>
{
    public Targeter targeter;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Melee> instances = new List<Melee>(); // 모든 Melee 인스턴스
    [SerializeField] private string[] clashTags; public string[] ClashTags { get { Start(); return clashTags; } set => clashTags = value; } // 충돌 대상 태그
    [SerializeField] private int penetrate; // 최대 피격 수
    [SerializeField] private float radius; // 피격 반지름
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

        Load();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportMelee(initialRecords[0].ID, ref clashTags, ref penetrate, ref radius, ref damage);
    } // Import 시 자동 실행

    /* Public Method */
    /// <summary>
    /// 지정한 위치에 원형 범위의 공격을 시전합니다.
    /// </summary>
    public void Attack(Vector3 worldPos)
    {
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