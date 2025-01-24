using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Color : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Color";
    public string ID { get; set; }
    public float r;
    public float g;
    public float b;
    public float a;

    /* Intializer & Finalizer & Updater */
    public Table_Color(string ID, float r, float g, float b, float a)
    {
        this.ID = ID;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}