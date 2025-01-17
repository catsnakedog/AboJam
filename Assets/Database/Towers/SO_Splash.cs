using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Splash", menuName = "Database/SO_Splash", order = int.MaxValue)]
public class SO_Splash : ScriptableObject
{
    public float delay; // 공격 딜레이
    public float detection; // 적 감지 범위
}