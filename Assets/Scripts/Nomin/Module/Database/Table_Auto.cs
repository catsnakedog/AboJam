using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Auto
{
    /* Field & Property */
    public string autoID;
    public float delay;
    public float detection;
    public float angle;
    public int subCount;
    public int subCountPlus;

    /* Intializer & Finalizer & Updater */
    public Table_Auto(string autoID, float delay, float detection, float angle, int subCount, int subCountPlus)
    {
        this.autoID = autoID;
        this.delay = delay;
        this.detection = detection;
        this.angle = angle;
        this.subCount = subCount;
        this.subCountPlus = subCountPlus;
    }
}