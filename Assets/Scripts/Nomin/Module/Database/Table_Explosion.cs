using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 PK 입니다.</br>
/// <br>CONSTRAINT : 컬럼 이름이 서버 테이블과 정확히 일치해야 합니다.</br>
/// </summary>
public class Table_Explosion
{
    /* Field & Property */
    public string explosionID;
    public float scale;
    public float radius;
    public float damage;
    public float time;

    /* Intializer & Finalizer & Updater */
    public Table_Explosion(string explosionID, float scale, float radius, float damage, float time)
    {
        this.explosionID = explosionID;
        this.scale = scale;
        this.radius = radius;
        this.damage = damage;
        this.time = time;
    }
}