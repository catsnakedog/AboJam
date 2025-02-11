using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Upgrade : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Upgrade";
    public string ID { get; set; }
    public string reinforceCostID;
    public float coefficient;

    /* Intializer & Finalizer & Updater */
    public Table_Upgrade(string ID, string reinforceCostID, float coefficient)
    {
        this.ID = ID;
        this.reinforceCostID = reinforceCostID;
        this.coefficient = coefficient;
    }
}