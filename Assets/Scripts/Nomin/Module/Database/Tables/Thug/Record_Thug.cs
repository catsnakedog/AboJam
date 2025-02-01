using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Thug", menuName = "Record/Record_Thug", order = int.MaxValue)]
public class Record_Thug : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public float delay;
    public float detection;
}