using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Color", menuName = "Record/Record_Color", order = int.MaxValue)]
public class Record_Color : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public float r;
    public float g;
    public float b;
    public float a;
}