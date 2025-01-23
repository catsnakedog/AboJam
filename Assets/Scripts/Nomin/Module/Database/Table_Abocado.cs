using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Abocado : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Abocado";
    public string ID { get; set; }
    public EnumData.Abocado level;
    public int quality;
    public int quality_max;
    public int harvest;
    public int harvestPlus;

    /* Intializer & Finalizer & Updater */
    public Table_Abocado(string ID, EnumData.Abocado level, int quality, int quality_max, int harvest, int harvestPlus)
    {
        this.ID = ID;
        this.level = level;
        this.quality = quality;
        this.quality_max = quality_max;
        this.harvest = harvest;
        this.harvestPlus = harvestPlus;
    }
}