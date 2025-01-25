using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Auto", menuName = "Record/Record_Auto", order = int.MaxValue)]
public class Record_Auto : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public int[] reinforceCost;
    public float delay;
    public float detection;
    public float angle;
    public int subCount;
    public int subCountPlus;
}
