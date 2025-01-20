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
    public Table_Abocado[] Abocado = { new Table_Abocado("Abocado", EnumData.Abocado.Cultivated, 0, 1, 1, 1) };
    public Table_Gunner[] Gunner = { new Table_Gunner("Gunner", 0.8f, 0.1f, 5f, 2) };
    public Table_Auto[] Auto = { new Table_Auto("Auto", 0.3f, 5f, 60f, 0, 1) };

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
            ImportGunner();
            ImportAuto();

            void ImportHP()
            {
                DataTable dataTable = dataSet.Tables["HP"];
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["HPID"] };
                foreach (Table_HP hp in HP)
                {
                    DataRow dataRow = dataTable.Rows.Find(hp.HPID);
                    hp.Hp_max = Convert.ToSingle(dataRow["Hp_max"]);
                    hp.HideFullHP = Convert.ToBoolean(dataRow["HideFullHP"]);
                }
            }
            void ImportAbocado()
            {
                DataTable dataTable = dataSet.Tables["Abocado"];
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["abocadoID"] };
                foreach (Table_Abocado abocado in Abocado)
                {
                    DataRow dataRow = dataTable.Rows.Find(abocado.abocadoID);
                    abocado.level = Enum.Parse<EnumData.Abocado>(Convert.ToString(dataRow["level"]));
                    abocado.quality = Convert.ToInt32(dataRow["quality"]);
                    abocado.quality_max = Convert.ToInt32(dataRow["quality_max"]);
                    abocado.harvest = Convert.ToInt32(dataRow["harvest"]);
                    abocado.harvestPlus = Convert.ToInt32(dataRow["harvestPlus"]);
                }
            }
            void ImportGunner()
            {
                DataTable dataTable = dataSet.Tables["Gunner"];
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["gunnerID"] };
                foreach (Table_Gunner gunner in Gunner)
                {
                    DataRow dataRow = dataTable.Rows.Find(gunner.gunnerID);
                    gunner.delay = Convert.ToSingle(dataRow["delay"]);
                    gunner.delay_fire = Convert.ToSingle(dataRow["delay_fire"]);
                    gunner.detection = Convert.ToSingle(dataRow["detection"]);
                    gunner.subCount = Convert.ToInt32(dataRow["subCount"]);
                }
            }
            void ImportAuto()
            {
                DataTable dataTable = dataSet.Tables["Auto"];
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["AutoID"] };
                foreach (Table_Auto auto in Auto)
                {
                    DataRow dataRow = dataTable.Rows.Find(auto.autoID);
                    auto.delay = Convert.ToSingle(dataRow["delay"]);
                    auto.detection = Convert.ToSingle(dataRow["detection"]);
                    auto.angle = Convert.ToSingle(dataRow["angle"]);
                    auto.subCount = Convert.ToInt32(dataRow["subCount"]);
                    auto.subCountPlus = Convert.ToInt32(dataRow["subCountPlus"]);
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
    /// <summary>
    /// 런타임 DB / Gunner 테이블 / GunnerID 레코드 => Gunner 인스턴스
    /// </summary>
    public void ExportGunner(string gunnerID, ref float delay, ref float delay_fire, ref float detection, ref int subCount)
    {
        Table_Gunner data = Gunner.FirstOrDefault(gunner => gunner.gunnerID == gunnerID);
        delay = data.delay;
        delay_fire = data.delay_fire;
        detection = data.detection;
        subCount = data.subCount;
    }
    /// <summary>
    /// 런타임 DB / Auto 테이블 / AutoID 레코드 => Auto 인스턴스
    /// </summary>
    public void ExportAuto(string autoID, ref float delay, ref float detection, ref float angle, ref int subCount, ref int subCountPlus)
    {
        Table_Auto data = Auto.FirstOrDefault(auto => auto.autoID == autoID);
        delay = data.delay;
        detection = data.detection;
        angle = data.angle;
        subCount = data.subCount;
        subCountPlus = data.subCountPlus;
    }
}