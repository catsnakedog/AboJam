using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Light", menuName = "Record/Record_Light", order = int.MaxValue)]
public class Record_Light : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public string colorID;
    public float radius;
    public float intensity;
    public float onTime;
    public float keepTime;
    public float offTime;
    public int frame;
}