using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// 런타임 데이터베이스입니다.
/// CONSTRAINT : 런타임 테이블 이름은 반드시 서버 테이블 이름과 같아야 합니다.
/// </summary>
public class Database_AboJam : MonoBehaviour
{
    /* Dependency */
    private DBMS dbms => DBMS.instance;

    /* Field & Property */
    public static Database_AboJam instance;

    /* Initializer */
    private void Awake()
    {
        instance = this;
    }

    /* Runtime Table */
    public Table_HP[] HP =
    {
        new Table_HP("Player", 100, false),
        new Table_HP("Abocado", 50, true),
        new Table_HP("Auto", 100, true),
        new Table_HP("Guard", 100, true),
        new Table_HP("Heal", 100, true),
        new Table_HP("Splash", 100, true),
        new Table_HP("Gunner", 100, true),
    };

    /* Public Method */
    /// <summary>
    /// 서버 데이터를 런타임 데이터에 덮어씁니다.
    /// </summary>
    public void Load()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            // HP 테이블 덮어쓰기
            DataTable dataTableHP = dataSet.Tables["HP"];
            dataTableHP.PrimaryKey = new DataColumn[] { dataTableHP.Columns["HPID"] };
            foreach (Table_HP hp in HP)
            {
                DataRow dataRow = dataTableHP.Rows.Find(hp.HPID);
                hp.Hp_max = Convert.ToSingle(dataRow["Hp_max"]);
                hp.HideFullHP = Convert.ToBoolean(dataRow["HideFullHP"]);
            }
        }
        catch (Exception)
        {
            Debug.Log("서버와의 연결이 원활하지 않거나, 잘못된 데이터가 존재합니다.");
            throw;
        }

    }
}