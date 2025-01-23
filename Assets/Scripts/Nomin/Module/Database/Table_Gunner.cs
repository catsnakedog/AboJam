using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Gunner : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Gunner";
    public string ID { get; set; }
    public float delay;
    public float delay_fire;
    public float detection;
    public int subCount;

    /* Intializer & Finalizer & Updater */
    public Table_Gunner(string ID, float delay, float delay_fire, float detection, int subCount)
    {
        this.ID = ID;
        this.delay = delay;
        this.delay_fire = delay_fire;
        this.detection = detection;
        this.subCount = subCount;
    }
}