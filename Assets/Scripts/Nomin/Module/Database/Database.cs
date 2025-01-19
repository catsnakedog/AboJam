using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임에 사용되는 데이터베이스입니다.
/// </summary>
public class Database_AboJam
{
    /* Dependency */
    public struct Table_HP
    {
        public string HPID;
        public float Hp_max;
        public bool HideFullHP;
    }

    /* Field & Property */
    public static Database_AboJam instance;
    public Table_HP[] HP;

    /* Intializer & Finalizer & Updater */
    public Database_AboJam()
    {
        instance = this;
    }
}

public class Loader
{

}
