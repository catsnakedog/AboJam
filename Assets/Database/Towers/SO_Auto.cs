using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Auto", menuName = "Database/SO_Auto", order = int.MaxValue)]
public class SO_Auto : ScriptableObject
{
    public float delay; // 공격 딜레이
    public float detection; // 적 감지 범위
    public float angle; // 서브 탄환 최대 각도
    public int subCount; // 서브 탄환 수
}