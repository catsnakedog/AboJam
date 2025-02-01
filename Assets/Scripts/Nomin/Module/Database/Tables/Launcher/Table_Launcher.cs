using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Launcher : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Launcher";
    public string ID { get; set; }

    [Header("[ Launcher ]")]
    public bool align;
    public float turnTime;
    public float angleOffset;
    public float frame;

    [Header("[ Launch ]")]
    public float speed;
    public float range;

    /* Intializer & Finalizer & Updater */
    public Table_Launcher(string ID, bool align, float turnTime, float angleOffset, float frame, float speed, float range)
    {
        this.ID = ID;
        this.align = align;
        this.turnTime = turnTime;
        this.angleOffset = angleOffset;
        this.frame = frame;
        this.speed = speed;
        this.range = range;
    }
}