using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Melee : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Melee";
    public string ID { get; set; }
    public string clashTagsID;
    public int penetrate;
    public float radius;
    public float delay;
    public float damage;
    public float effectTime;
    public float knockback;

    /* Intializer & Finalizer & Updater */
    public Table_Melee(string ID, string clashTagsID, int penetrate, float radius, float damage, float effectTime, float knockback)
    {
        this.ID = ID;
        this.clashTagsID = clashTagsID;
        this.penetrate = penetrate;
        this.radius = radius;
        this.damage = damage;
        this.effectTime = effectTime;
        this.knockback = knockback;
    }
}