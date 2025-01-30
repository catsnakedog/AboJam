using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Wave", menuName = "Record /Record_Wave", order = int.MaxValue)]
public class Record_Wave : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public float delay;
    public string spawnID;
}