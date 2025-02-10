using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Melee", menuName = "Record/Record_Melee", order = int.MaxValue)]
public class Record_Melee : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id;  public string ID { get => id; set => id = value; }
    public string clashTagsID;
    public int penetrate;
    public float radius;
    public float damage;
    public float effectTime;
    public float knockback;
}