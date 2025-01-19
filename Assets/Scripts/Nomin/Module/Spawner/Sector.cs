using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>두 원과 각도로 정의된 영역입니다.</br>
/// <br>HDD 의 Sector 와 매우 유사합니다.</br>
/// </summary>
public struct Sector
{
    /* Field & Property */
    public static List<Sector> sectors = new();
    public string sectorID;
    public float angleStart;
    public float angleEnd;
    public float radiusIn;
    public float radiusOut;

    /* Intializer & Finalizer & Updater */
    public Sector(string sectorID, float angleStart, float angleEnd, float radiusIn, float radiusOut)
    {
        this.sectorID = sectorID;
        this.angleStart = angleStart;
        this.angleEnd = angleEnd;
        this.radiusIn = radiusIn;
        this.radiusOut = radiusOut;
    }
}