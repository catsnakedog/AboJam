using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Gunner
{
    /* Field & Property */
    public string gunnerID;
    public float delay;
    public float delay_fire;
    public float detection;
    public int subCount;

    /* Intializer & Finalizer & Updater */
    public Table_Gunner(string gunnerID, float delay, float delay_fire, float detection, int subCount)
    {
        this.gunnerID = gunnerID;
        this.delay = delay;
        this.delay_fire = delay_fire;
        this.detection = detection;
        this.subCount = subCount;
    }
}