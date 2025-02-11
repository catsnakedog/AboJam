using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Upgrade", menuName = "Record/Record_Upgrade", order = int.MaxValue)]
public class Record_Upgrade : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public string reinforceCostID;
    public float coefficient;
}
