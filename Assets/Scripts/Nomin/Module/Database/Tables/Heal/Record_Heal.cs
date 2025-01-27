using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Heal", menuName = "Record/Record_Heal", order = int.MaxValue)]
public class Record_Heal : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public string reinforceCostID;
    public float delay;
    public float detection;
    public float ratio;
}
