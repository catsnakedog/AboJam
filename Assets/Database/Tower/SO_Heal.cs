using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Heal", menuName = "Database/Tower/SO_Heal", order = int.MaxValue)]
public class SO_Heal : ScriptableObject
{
    private void OnValidate()
    {
        if (delay < 0) Debug.Log($"{name} 의 delay 는 0 보다 작을 수 없습니다.");
        if (detection < 0) Debug.Log($"{name} 의 detection 은 0 보다 작을 수 없습니다.");
        if (ratio <= 0) Debug.Log($"{name} 의 ratio 는 0 이하일 수 없습니다.");
    }

    public float delay; // 회복 딜레이
    public float detection; // 아군 감지 범위
    public float ratio; // 회복 대상 체력 비율 조건
}