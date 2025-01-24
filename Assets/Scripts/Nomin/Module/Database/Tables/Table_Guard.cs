using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Guard : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Guard";
    public string ID { get; set; }
    public float hpMultiply;

    /* Intializer & Finalizer & Updater */
    public Table_Guard(string ID, float hpMultiply)
    {
        this.ID = ID;
        this.hpMultiply = hpMultiply;
    }
}