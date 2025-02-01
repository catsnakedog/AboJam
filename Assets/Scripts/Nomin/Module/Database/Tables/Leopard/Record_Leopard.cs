using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Leopard", menuName = "Record/Record_Leopard", order = int.MaxValue)]
public class Record_Leopard : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public float delay;
    public float detection;
}