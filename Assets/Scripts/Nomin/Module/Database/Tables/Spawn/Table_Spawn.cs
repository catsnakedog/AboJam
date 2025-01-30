using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Spawn : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Spawn";
    public string ID { get; set; }
    public string sectorID;
    public string spawneeID;
    public float interval;
    public int count;

    /* Intializer & Finalizer & Updater */
    public Table_Spawn(string ID, string sectorID, string spawneeID, float interval, int count)
    {
        this.ID = ID;
        this.sectorID = sectorID;
        this.spawneeID = spawneeID;
        this.interval = interval;
        this.count = count;
    }
}