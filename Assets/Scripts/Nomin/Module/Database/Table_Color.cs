using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Color : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Color";
    public string ID { get; set; }
    public float r;
    public float g;
    public float b;
    public float a;

    /* Intializer & Finalizer & Updater */
    public Table_Color(string ID, float r, float g, float b, float a)
    {
        this.ID = ID;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}