using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_HP", menuName = "Database/HP/SO_HP", order = int.MaxValue)]
public class SO_HP : ScriptableObject
{
    private void OnValidate()
    {
        if (hp_max <= 0) Debug.Log($"{name} 의 체력이 0 이하로 설정되어 있습니다.");
    }

    public float hp_max; // 최대 체력 (초기화에만 사용됨)
    public bool hideFullHp;
}