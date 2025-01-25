using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_ClashTags : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "ClashTags";
    public string ID { get; set; }
    public string clashTags;

    /* Intializer & Finalizer & Updater */
    public Table_ClashTags(string ID, string clashTags)
    {
        this.ID = ID;
        this.clashTags = clashTags;
    }
}