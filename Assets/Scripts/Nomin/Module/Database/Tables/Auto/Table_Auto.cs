using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Auto : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Auto";
    public string ID { get; set; }
    public string reinforceCostID;
    public float delay;
    public float detection;
    public float angle;
    public int subCount;
    public int subCountPlus;

    /* Intializer & Finalizer & Updater */
    public Table_Auto(string ID, string reinforceCostID, float delay, float detection, float angle, int subCount, int subCountPlus)
    {
        this.ID = ID;
        this.reinforceCostID = reinforceCostID;
        this.delay = delay;
        this.detection = detection;
        this.angle = angle;
        this.subCount = subCount;
        this.subCountPlus = subCountPlus;
    }
}