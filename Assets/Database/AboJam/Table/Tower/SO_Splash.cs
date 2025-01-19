using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Splash", menuName = "Database/Tower/SO_Splash", order = int.MaxValue)]
public class SO_Splash : ScriptableObject
{
    private void OnValidate()
    {
        if (delay < 0) Debug.Log($"{name} 의 delay 는 0 보다 작을 수 없습니다.");
        if (detection < 0) Debug.Log($"{name} 의 detection 은 0 보다 작을 수 없습니다.");
    }

    public float delay; // 공격 딜레이
    public float detection; // 적 감지 범위
}