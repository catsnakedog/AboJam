using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <br>런타임 데이터베이스의 테이블 입니다.</br>
/// <br>CONSTRAINT : 첫 번째 필드는 정확한 서버 테이블 이름입니다.</br>
/// <br>CONSTRAINT : 두 번째 필드는 PK 입니다.</br>
/// </summary>
public class Table_Explosion : ITable
{
    /* Field & Property */
    public string TableName { get; set; } = "Explosion";
    public string ID { get; set; }
    public float scale;
    public float radius;
    public float damage;
    public float time;

    /* Intializer & Finalizer & Updater */
    public Table_Explosion(string ID, float scale, float radius, float damage, float time)
    {
        this.ID = ID;
        this.scale = scale;
        this.radius = radius;
        this.damage = damage;
        this.time = time;
    }
}

public interface ITable
{
    public static List<ITable[]> instances = new List<ITable[]>();

    /// <summary>
    /// 반드시 서버 테이블 이름과 동일해야 합니다.
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 반드시 UNIQUE 해야 합니다.
    /// </summary>
    public string ID { get; set; }
}