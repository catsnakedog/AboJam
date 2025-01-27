using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Guard : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Guard";
    public string ID { get; set; }
    public string reinforceCostID;
    public float hpMultiply;

    /* Intializer & Finalizer & Updater */
    public Table_Guard(string ID, string reinforceCostID, float hpMultiply)
    {
        this.ID = ID;
        this.reinforceCostID = reinforceCostID;
        this.hpMultiply = hpMultiply;
    }
}