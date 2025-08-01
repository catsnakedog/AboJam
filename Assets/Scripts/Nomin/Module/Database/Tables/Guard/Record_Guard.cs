using UnityEngine;

[CreateAssetMenu(fileName = "Record_Guard", menuName = "Record/Record_Guard", order = int.MaxValue)]
public class Record_Guard : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] public string id; public string ID { get => id; set => id = value; }
    public string reinforceCostID;
    public float hpMultiply;
}
