using UnityEngine;

[CreateAssetMenu(fileName = "Record_Auto", menuName = "Record/Record_Auto", order = int.MaxValue)]
public class Record_Auto : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public string reinforceCostID;
    public float delay;
    public float detection;
    public float angle;
    public int subCount;
    public int subCountPlus;
}
