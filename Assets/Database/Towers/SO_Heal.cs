using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Heal", menuName = "Database/SO_Heal", order = int.MaxValue)]
public class SO_Heal : ScriptableObject
{
    public float delay; // 회복 딜레이
    public float detection; // 아군 감지 범위
    public float ratio; // 회복 대상 체력 비율 조건
}