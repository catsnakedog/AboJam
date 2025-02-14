using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Skill", menuName = "Record/Record_Skill", order = int.MaxValue)]
public class Record_Skill : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public float cooldown;
    public int range;
    public int count;
    public float seconds;
    public float speed;
}