using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Spawnee : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Spawnee";
    public string ID { get; set; }
    public string prefab;
    public int level;

    /* Intializer & Finalizer & Updater */
    public Table_Spawnee(string ID, string prefab, int level)
    {
        this.ID = ID;
        this.prefab = prefab;
        this.level = level;
    }
}