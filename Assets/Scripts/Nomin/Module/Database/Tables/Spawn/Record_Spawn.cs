using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Spawn", menuName = "Record /Record_Spawn", order = int.MaxValue)]
public class Record_Spawn : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public string sectorID;
    public string spawneeID;
    public float interval;
    public int count;
}