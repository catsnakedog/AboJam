using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Record_Date", menuName = "Record/Record_Date", order = int.MaxValue)]
public class Record_Date : ScriptableObject, IRecord
{
    /* Field & Property */
    [SerializeField] private string id; public string ID { get => id; set => id = value; }
    public int secondsPerDay;
    public string startTime;
    public string morningTime;
    public string sunsetTime;
    public string nightTime;
}