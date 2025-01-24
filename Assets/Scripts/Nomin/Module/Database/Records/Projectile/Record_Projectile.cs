using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Projectile", menuName = "Record/Record_Projectile", order = int.MaxValue)]
public class Record_Projectile : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public string clashTagsID;
    public float damage;
    public int penetrate;
}