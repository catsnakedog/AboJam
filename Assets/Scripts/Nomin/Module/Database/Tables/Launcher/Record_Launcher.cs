using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Launcher", menuName = "Record/Record_Launcher", order = int.MaxValue)]
public class Record_Launcher : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }

    [Header("[ Launcher ]")]
    public bool align;
    public float turnTime;
    public float angleOffset;
    public float frame;

    [Header("[ Launch ]")]
    public float speed;
    public float range;
}