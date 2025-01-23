using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Projectile : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Projectile";
    public string ID { get; set; }
    public string clashTagsID;
    public float damage = 10f;
    public int penetrate = 1;

    /* Intializer & Finalizer & Updater */
    public Table_Projectile(string ID, string clashTagsID, float damage, int penetrate)
    {
        this.ID = ID;
        this.clashTagsID = clashTagsID;
        this.damage = damage;
        this.penetrate = penetrate;
    }
}