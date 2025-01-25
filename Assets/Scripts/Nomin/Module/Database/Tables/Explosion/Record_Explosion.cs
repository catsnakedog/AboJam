using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Explosion", menuName = "Record/Record_Explosion", order = int.MaxValue)]
public class Record_Explosion : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public float scale;
    public float radius;
    public float damage;
    public float time;
}