using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
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
    public Table_Guard[] Guard = { new Table_Guard("Guard", 1.5f) };
    public Table_Splash[] Splash = { new Table_Splash("Splash", 1f, 5f) };
    public Table_Heal[] Heal = { new Table_Heal("Heal", 0.4f, 10f, 0.9999f) };

    /* Public Method */
    /// <summary>
    /// 서버 데이터베이스의 모든 데이터를 런타임 데이터베이스로 받아옵니다.
    /// </summary>
    public void Import()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            ImportHP(dataSet);
            ImportAbocado(dataSet);
            ImportGunner(dataSet);
            ImportAuto(dataSet);
            ImportGuard(dataSet);
            ImportSplash(dataSet);
            ImportHeal(dataSet);
        }
        catch (Exception) { Debug.Log("서버와의 연결이 원활하지 않거나, 잘못된 데이터가 존재합니다."); throw; }
    }

    /// <summary>
    /// 런타임 데이터베이스의 모든 데이터를 인스턴스에 덮어씁니다.
    /// </summary>
    public void Export()
    {
        foreach (HP item in global::HP.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Abocado item in global::Abocado.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Gunner item in global::Gunner.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Auto item in global::Auto.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Guard item in global::Guard.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Splash item in global::Splash.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Heal item in global::Heal.instances) if (item.isActiveAndEnabled) item.Load();
    }

    // 서버 DB / 각 테이블 / ID 레코드 => 런타임 DB
    // 기획자가 인게임에서 Import 누를 시 호출
    public void ImportHP(DataSet dataSet)
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
    public void ImportAbocado(DataSet dataSet)
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
    public void ImportGunner(DataSet dataSet)
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
    public void ImportAuto(DataSet dataSet)
    {
        DataTable dataTable = dataSet.Tables["Auto"];
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["autoID"] };
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
    public void ImportGuard(DataSet dataSet)
    {
        DataTable dataTable = dataSet.Tables["Guard"];
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["guardID"] };
        foreach (Table_Guard guard in Guard)
        {
            DataRow dataRow = dataTable.Rows.Find(guard.guardID);
            guard.hpMultiply = Convert.ToSingle(dataRow["hpMultiply"]);
        }
    }
    public void ImportSplash(DataSet dataSet)
    {
        DataTable dataTable = dataSet.Tables["Splash"];
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["splashID"] };
        foreach (Table_Splash splash in Splash)
        {
            DataRow dataRow = dataTable.Rows.Find(splash.splashID);
            splash.delay = Convert.ToSingle(dataRow["delay"]);
            splash.detection = Convert.ToSingle(dataRow["detection"]);
        }
    }
    public void ImportHeal(DataSet dataSet)
    {
        DataTable dataTable = dataSet.Tables["Heal"];
        dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["healID"] };
        foreach (Table_Heal heal in Heal)
        {
            DataRow dataRow = dataTable.Rows.Find(heal.healID);
            heal.delay = Convert.ToSingle(dataRow["delay"]);
            heal.detection = Convert.ToSingle(dataRow["detection"]);
            heal.ratio = Convert.ToSingle(dataRow["ratio"]);
        }
    }

    // 런타임 DB / 각 테이블 / ID 레코드 => 인스턴스
    // 기획자가 인게임에서 Export 누를 시 호출
    // 인스턴스가 Load (풀에서 꺼내지거나, 처음 생성) 될 때 마다 호출
    public void ExportHP(string HPID, ref float hp_max, ref bool hideFullHp)
    {
        Table_HP data = HP.FirstOrDefault(hp => hp.HPID == HPID);
        hp_max = data.Hp_max;
        hideFullHp = data.HideFullHP;
    }
    public void ExportAbocado(string abocadoID, ref EnumData.Abocado level, ref int quality, ref int quality_max, ref int harvest, ref int harvestPlus)
    {
        Table_Abocado data = Abocado.FirstOrDefault(abocado => abocado.abocadoID == abocadoID);
        level = data.level;
        quality = data.quality;
        quality_max = data.quality_max;
        harvest = data.harvest;
        harvestPlus = data.harvestPlus;
    }
    public void ExportGunner(string gunnerID, ref float delay, ref float delay_fire, ref float detection, ref int subCount)
    {
        Table_Gunner data = Gunner.FirstOrDefault(gunner => gunner.gunnerID == gunnerID);
        delay = data.delay;
        delay_fire = data.delay_fire;
        detection = data.detection;
        subCount = data.subCount;
    }
    public void ExportAuto(string autoID, ref float delay, ref float detection, ref float angle, ref int subCount, ref int subCountPlus)
    {
        Table_Auto data = Auto.FirstOrDefault(auto => auto.autoID == autoID);
        delay = data.delay;
        detection = data.detection;
        angle = data.angle;
        subCount = data.subCount;
        subCountPlus = data.subCountPlus;
    }
    public void ExportGuard(string guardID, ref float hpMultiply)
    {
        Table_Guard data = Guard.FirstOrDefault(guard => guard.guardID == guardID);
        hpMultiply = data.hpMultiply;
    }
    public void ExportSplash(string splashID, ref float delay, ref float detection)
    {
        Table_Splash data = Splash.FirstOrDefault(splash => splash.splashID == splashID);
        delay = data.delay;
        detection = data.detection;
    }
    public void ExportHeal(string healID, ref float delay, ref float detection, ref float ratio)
    {
        Table_Heal data = Heal.FirstOrDefault(heal => heal.healID == healID);
        delay = data.delay;
        detection = data.detection;
        ratio = data.ratio;
    }
}