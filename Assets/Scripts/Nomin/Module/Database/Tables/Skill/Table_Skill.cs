using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Skill : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Skill";
    public string ID { get; set; }

    public float cooldown;
    public int range;
    public int count;
    public float seconds;
    public float speed;

    /* Intializer & Finalizer & Updater */
    public Table_Skill(string ID, float cooldown, int range, int count, float seconds, float speed)
    {
        this.ID = ID;
        this.cooldown = cooldown;
        this.range = range;
        this.count = count;
        this.seconds = seconds;
        this.speed = speed;
    }
}