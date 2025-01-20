using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.HID;
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

    /* Runtime Table : 아래는 초기 데이터이며, ImportAll 시 서버 데이터로 덮어씌워집니다. */
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
    public Table_Abocado[] Abocado =
    {
        new Table_Abocado("Abocado", EnumData.Abocado.Cultivated, 0, 1, 1, 1)
    };

    /* Public Method */
    /// <summary>
    /// 서버 데이터베이스의 모든 데이터를 런타임 데이터베이스로 받아옵니다.
    /// </summary>
    public void Import()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            ImportHP();
            ImportAbocado();

            void ImportHP()
            {
                DataTable dataTableHP = dataSet.Tables["HP"];
                dataTableHP.PrimaryKey = new DataColumn[] { dataTableHP.Columns["HPID"] };
                foreach (Table_HP hp in HP)
                {
                    DataRow dataRow = dataTableHP.Rows.Find(hp.HPID);
                    hp.Hp_max = Convert.ToSingle(dataRow["Hp_max"]);
                    hp.HideFullHP = Convert.ToBoolean(dataRow["HideFullHP"]);
                }
            }
            void ImportAbocado()
            {

                DataTable dataTableAbocado = dataSet.Tables["Abocado"];
                dataTableAbocado.PrimaryKey = new DataColumn[] { dataTableAbocado.Columns["AbocadoID"] };
                foreach (Table_Abocado abocado in Abocado)
                {
                    DataRow dataRow = dataTableAbocado.Rows.Find(abocado.abocadoID);
                    abocado.level = Enum.Parse<EnumData.Abocado>(Convert.ToString(dataRow["level"]));
                    abocado.quality = Convert.ToInt32(dataRow["quality"]);
                    abocado.quality_max = Convert.ToInt32(dataRow["quality_max"]);
                    abocado.harvest = Convert.ToInt32(dataRow["harvest"]);
                    abocado.harvestPlus = Convert.ToInt32(dataRow["harvestPlus"]);
                }
            }
        }
        catch (Exception) { Debug.Log("서버와의 연결이 원활하지 않거나, 잘못된 데이터가 존재합니다."); throw; }
    }
    /// <summary>
    /// 런타임 DB / HP 테이블 / HPID 레코드 => HP 인스턴스
    /// </summary>
    public void ExportHP(string HPID, ref float hp_max, ref bool hideFullHp)
    {
        Table_HP data = HP.FirstOrDefault(hp => hp.HPID == HPID);
        hp_max = data.Hp_max;
        hideFullHp = data.HideFullHP;
    }
    /// <summary>
    /// 런타임 DB / Abocado 테이블 / AbocadoID 레코드 => Abocado 인스턴스
    /// </summary>
    public void ExportAbocado(string abocadoID, ref EnumData.Abocado level, ref int quality, ref int quality_max, ref int harvest, ref int harvestPlus)
    {
        Table_Abocado data = Abocado.FirstOrDefault(abocado => abocado.abocadoID == abocadoID);
        level = data.level;
        quality = data.quality;
        quality_max = data.quality_max;
        harvest = data.harvest;
        harvestPlus = data.harvestPlus;
    }
}