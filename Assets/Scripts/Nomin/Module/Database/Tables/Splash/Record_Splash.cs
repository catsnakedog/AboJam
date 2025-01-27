using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Splash", menuName = "Record/Record_Splash", order = int.MaxValue)]
public class Record_Splash : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public string reinforceCostID;
    public float delay;
    public float detection;
}
