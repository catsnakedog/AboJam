using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Light : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Light";
    public string ID { get; set; }
    public string colorID;
    public float radius;
    public float intensity;
    public float onTime;
    public float keepTime;
    public float offTime;
    public int frame;

    /* Intializer & Finalizer & Updater */
    public Table_Light(string ID, string colorID, float radius, float intensity, float onTime, float keepTime, float offTime, int frame)
    {
        this.ID = ID;
        this.colorID = colorID;
        this.radius = radius;
        this.intensity = intensity;
        this.onTime = onTime;
        this.keepTime = keepTime;
        this.offTime = offTime;
        this.frame = frame;
    }
}