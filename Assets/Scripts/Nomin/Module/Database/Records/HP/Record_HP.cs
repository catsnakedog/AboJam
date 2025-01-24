using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_HP", menuName = "Record/Record_HP", order = int.MaxValue)]
public class Record_HP : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public float hp_max; // 최대 체력 (초기화에만 사용됨)
    public bool hideFullHp;

    /* Intializer & Finalizer & Updater */
    private void OnValidate() { if (hp_max <= 0) Debug.Log($"{name} 의 체력이 0 이하로 설정되어 있습니다."); }
}