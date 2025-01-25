using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Splash : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Splash";
    public string ID { get; set; }
    public int[] reinforceCost;
    public float delay;
    public float detection;

    /* Intializer & Finalizer & Updater */
    public Table_Splash(string ID, int[] reinforceCost, float delay, float detection)
    {
        this.ID = ID;
        this.reinforceCost = reinforceCost;
        this.delay = delay;
        this.detection = detection;
    }
}