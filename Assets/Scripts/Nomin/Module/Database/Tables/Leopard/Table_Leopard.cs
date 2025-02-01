using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Leopard : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Leopard";
    public string ID { get; set; }
    public float delay;
    public float detection;

    /* Intializer & Finalizer & Updater */
    public Table_Leopard(string ID, float delay, float detection)
    {
        this.ID = ID;
        this.delay = delay;
        this.detection = detection;
    }
}