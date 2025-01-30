using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Spawnee", menuName = "Record/Record_Spawnee", order = int.MaxValue)]
public class Record_Spawnee : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public string prefab;
}