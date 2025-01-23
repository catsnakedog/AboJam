using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Heal : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Heal";
    public string ID { get; set; }
    public float delay;
    public float detection;
    public float ratio;

    /* Intializer & Finalizer & Updater */
    public Table_Heal(string ID, float delay, float detection, float ratio)
    {
        this.ID = ID;
        this.delay = delay;
        this.detection = detection;
        this.ratio = ratio;
    }
}