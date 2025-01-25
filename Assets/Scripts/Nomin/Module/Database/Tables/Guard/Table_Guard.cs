using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Guard : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Guard";
    public string ID { get; set; }
    public int[] reinforceCost;
    public float hpMultiply;

    /* Intializer & Finalizer & Updater */
    public Table_Guard(string ID, int[] reinforceCost, float hpMultiply)
    {
        this.ID = ID;
        this.reinforceCost = reinforceCost;
        this.hpMultiply = hpMultiply;
    }
}