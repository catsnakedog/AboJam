using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_Abocado", menuName = "Database/Abocado/SO_Abocado", order = int.MaxValue)]
public class SO_Abocado : ScriptableObject
{
    private void OnValidate()
    {
        if (level < 0) Debug.Log($"{name} 의 level 은 0 보다 작을 수 없습니다.");
        if (quality < 0) Debug.Log($"{name} 의 quality 는 0 보다 작을 수 없습니다.");
        if (quality_max < 0) Debug.Log($"{name} 의 quality_max 는 0 보다 작을 수 없습니다.");
        if (harvest < 0) Debug.Log($"{name} 의 harvest 는 0 보다 작을 수 없습니다.");
    }

    public EnumData.Abocado level; // 아보카도 레벨
    public int quality; // 아보카도 품질 (Promotion)
    public int quality_max; // 아보카도 최고 품질 (Promotion)
    public int harvest; // 수확량
}