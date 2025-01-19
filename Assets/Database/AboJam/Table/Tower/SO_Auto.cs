using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Auto", menuName = "Database/Tower/SO_Auto", order = int.MaxValue)]
public class SO_Auto : ScriptableObject
{
    private void OnValidate()
    {
        if (delay < 0) Debug.Log($"{name} 의 delay 는 0 보다 작을 수 없습니다.");
        if (detection < 0) Debug.Log($"{name} 의 detection 은 0 보다 작을 수 없습니다.");
        if (angle < 0) Debug.Log($"{name} 의 angle 은 0 보다 작을 수 없습니다.");
        if (subCount < 0) Debug.Log($"{name} 의 subCount 는 0 보다 작을 수 없습니다.");
    }

    public float delay; // 공격 딜레이
    public float detection; // 적 감지 범위
    public float angle; // 서브 탄환 최대 각도
    public int subCount; // 서브 탄환 수
}