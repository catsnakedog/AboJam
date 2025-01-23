using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Light : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Light";
    public string ID { get; set; }
    public string colorID;
    public float radius;
    public float intensity;
    public float onTime;
    public float keepTime;
    public float offTime;
    public int frame;

    /* Intializer & Finalizer & Updater */
    public Table_Light(string ID, string colorID, float radius, float intensity, float onTime, float keepTime, float offTime, int frame)
    {
        this.ID = ID;
        this.colorID = colorID;
        this.radius = radius;
        this.intensity = intensity;
        this.onTime = onTime;
        this.keepTime = keepTime;
        this.offTime = offTime;
        this.frame = frame;
    }
}