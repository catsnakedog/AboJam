using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_HP : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "HP";
    public string ID { get; set; }
    public float Hp_max;
    public bool HideFullHP;

    /* Intializer & Finalizer & Updater */
    public Table_HP(string ID, float Hp_max, bool HideFullHP)
    {
        this.ID = ID;
        this.Hp_max = Hp_max;
        this.HideFullHP = HideFullHP;
    }
}