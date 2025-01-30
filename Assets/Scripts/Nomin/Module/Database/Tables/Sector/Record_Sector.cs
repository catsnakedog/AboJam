using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Sector", menuName = "Record/Record_Sector", order = int.MaxValue)]
public class Record_Sector : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public float angleStart;
    public float angleEnd;
    public float radiusIn;
    public float radiusOut;
}