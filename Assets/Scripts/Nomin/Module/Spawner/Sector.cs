using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>두 원과 각도로 정의된 영역입니다.</br>
/// <br>HDD 의 Sector 와 매우 유사합니다.</br>
/// </summary>
public struct Sector
{
    public static List<Sector> instances = new();
    public string ID;
    public float angleStart;
    public float angleEnd;
    public float radiusIn;
    public float radiusOut;

    /* Intializer & Finalizer & Updater */
    public Sector(string ID, float angleStart, float angleEnd, float radiusIn, float radiusOut)
    {
        this.ID = ID;
        this.angleStart = angleStart;
        this.angleEnd = angleEnd;
        this.radiusIn = radiusIn;
        this.radiusOut = radiusOut;
    }
}