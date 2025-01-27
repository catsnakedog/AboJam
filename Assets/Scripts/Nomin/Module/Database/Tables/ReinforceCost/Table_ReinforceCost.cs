using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_ReinforceCost : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "ReinforceCost";
    public string ID { get; set; }
    public string reinforceCost;

    /* Intializer & Finalizer & Updater */
    public Table_ReinforceCost(string ID, string reinforceCost)
    {
        this.ID = ID;
        this.reinforceCost = reinforceCost;
    }
}