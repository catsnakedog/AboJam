using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Splash : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Splash";
    public string ID { get; set; }
    public float delay;
    public float detection;

    /* Intializer & Finalizer & Updater */
    public Table_Splash(string ID, float delay, float detection)
    {
        this.ID = ID;
        this.delay = delay;
        this.detection = detection;
    }
}