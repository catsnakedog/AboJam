using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Sector : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Sector";
    public string ID { get; set; }
    public float angleStart;
    public float angleEnd;
    public float radiusIn;
    public float radiusOut;

    /* Intializer & Finalizer & Updater */
    public Table_Sector(string ID, float angleStart, float angleEnd, float radiusIn, float radiusOut)
    {
        this.ID = ID;
        this.angleStart = angleStart;
        this.angleEnd = angleEnd;
        this.radiusIn = radiusIn;
        this.radiusOut = radiusOut;
    }
}