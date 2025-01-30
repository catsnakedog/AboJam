using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Wave : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Wave";
    public string ID { get; set; }
    public float delay;
    public string spawnID;

    /* Intializer & Finalizer & Updater */
    public Table_Wave(string ID, float delay, string spawnID)
    {
        this.ID = ID;
        this.delay = delay;
        this.spawnID = spawnID;
    }
}