using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Gunner", menuName = "Record/Record_Gunner", order = int.MaxValue)]
public class Record_Gunner : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public float delay; // 공격 딜레이
    public float delay_fire; // 사격 간격
    public float detection; // 적 감지 범위
    public int subCount; // 서브 탄환 수

    /* Intializer & Finalizer & Updater */
    private void OnValidate()
    {
        if (delay < 0) Debug.Log($"{name} 의 delay 는 0 보다 작을 수 없습니다.");
        if (delay_fire < 0) Debug.Log($"{name} 의 delay_fire 은 0 보다 작을 수 없습니다.");
        if (detection < 0) Debug.Log($"{name} 의 detection 은 0 보다 작을 수 없습니다.");
        if (subCount < 0) Debug.Log($"{name} 의 subCount 는 0 보다 작을 수 없습니다.");
    }
}