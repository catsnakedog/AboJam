using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.LookDev;

[CreateAssetMenu(fileName = "Record_Abocado", menuName = "Record/Record_Abocado", order = int.MaxValue)]
public class Record_Abocado : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id;  public string ID { get => id; set => id = value; }
    public EnumData.Abocado level;
    public int quality;
    public int quality_max;
    public int harvest;
    public int harvestPlus;
}