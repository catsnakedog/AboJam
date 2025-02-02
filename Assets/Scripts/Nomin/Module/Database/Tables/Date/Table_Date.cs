using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table_Date : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Date";
    public string ID { get; set; }
    public int secondsPerDay;
    public string startTime;
    public string morningTime;
    public string sunsetTime;
    public string nightTime;

    /* Intializer & Finalizer & Updater */
    public Table_Date(string ID, int secondsPerDay, string startTime, string morningTime, string sunsetTime, string nightTime)
    {
        this.ID = ID;
        this.secondsPerDay = secondsPerDay;
        this.startTime = startTime;
        this.morningTime = morningTime;
        this.sunsetTime = sunsetTime;
        this.nightTime = nightTime;
    }
}