using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Table_Explosion : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Explosion";
    public string ID { get; set; }
    public float scale;
    public float radius;
    public float damage;
    public float time;

    /* Intializer & Finalizer & Updater */
    public Table_Explosion(string ID, float scale, float radius, float damage, float time)
    {
        this.ID = ID;
        this.scale = scale;
        this.radius = radius;
        this.damage = damage;
        this.time = time;
    }
}