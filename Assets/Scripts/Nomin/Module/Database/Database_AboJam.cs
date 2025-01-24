using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// <br>런타임 데이터베이스입니다. (현재 Script Execution Order : 100)</br>
/// <br>CONSTRAINT : 반드시 모든 레코드 인스턴스보다 먼저 Awake 가 호출되어야 합니다.</br>
/// <br>CONSTRAINT : 런타임 테이블 이름은 반드시 서버 테이블 이름과 같아야 합니다.</br>
/// </summary>
public class Database_AboJam : MonoBehaviour
{
    /* Dependency */
    private DBMS dbms => DBMS.instance;

    /* Field & Property */
    public static Database_AboJam instance;

    /* Initializer */
    private void Awake() { instance = this; }

    /* Runtime Table : ImportTable 시 서버 데이터로 덮어씌워집니다. */
    public List<Table_HP> HP = new() { new Table_HP("ID", Hp_max: 1f, HideFullHP: true) };
    public List<Table_Abocado> Abocado = new() { new Table_Abocado("ID", level: EnumData.Abocado.Cultivated, quality: 1, quality_max: 1, harvest: 1, harvestPlus: 1) };
    public List<Table_Gunner> Gunner = new() { new Table_Gunner("ID", delay: 1f, delay_fire: 1f, detection: 1f, subCount: 1) };
    public List<Table_Auto> Auto = new() { new Table_Auto("ID", delay: 1f, detection: 1f, angle: 1f, subCount: 1, subCountPlus: 1) };
    public List<Table_Guard> Guard = new() { new Table_Guard("ID", hpMultiply: 1f) };
    public List<Table_Splash> Splash = new() { new Table_Splash("ID", delay: 1f, detection: 1f) };
    public List<Table_Heal> Heal = new() { new Table_Heal("ID", delay: 1f, detection: 1f, ratio: 1f) };
    public List<Table_Light> Light = new() { new Table_Light("ID", "colorID", radius: 1f, intensity: 1f, onTime: 1f, keepTime: 1f, offTime: 1f, frame: 1) };
    public List<Table_Color> Color = new() { new Table_Color("ID", r: 1f, g: 1f, b: 1f, a: 1f) };
    public List<Table_Explosion> Explosion = new() { new Table_Explosion("ID", scale: 1f, radius: 1f, damage: 1f, time: 1f) };
    public List<Table_Projectile> Projectile = new() { new Table_Projectile("ID", "clashTagsID", damage: 1f, penetrate: 1) };
    public List<Table_ClashTags> ClashTags = new() { new Table_ClashTags("ID", "clashTags") };

    /* Public Method */
    // Import : 서버 DB >> 런타임 DB
    // Export : 런타임 DB >> 인스턴스
    public void ImportDatabase()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            ImportTable(dataSet, ref HP);
            ImportTable(dataSet, ref Abocado);
            ImportTable(dataSet, ref Gunner);
            ImportTable(dataSet, ref Auto);
            ImportTable(dataSet, ref Guard);
            ImportTable(dataSet, ref Splash);
            ImportTable(dataSet, ref Heal);
            ImportTable(dataSet, ref Light);
            ImportTable(dataSet, ref Color);
            ImportTable(dataSet, ref Explosion);
            ImportTable(dataSet, ref Projectile);
            ImportTable(dataSet, ref ClashTags);
        }
        catch (Exception) { Debug.Log("서버와의 연결이 원활하지 않거나, 잘못된 데이터가 존재합니다."); throw; }
    }
    public void ExportDatabase()
    {
        foreach (HP item in global::HP.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Abocado item in global::Abocado.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Gunner item in global::Gunner.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Auto item in global::Auto.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Guard item in global::Guard.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Splash item in global::Splash.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Heal item in global::Heal.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Light item in global::Light.instances) if (item.isActiveAndEnabled) item.Load();
        // Color 는 데이터 상으로만 존재하여 Export 가 불가능합니다.
        foreach (Explosion item in global::Explosion.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Projectile item in global::Projectile.instances) if (item.isActiveAndEnabled) item.Load();
        // ClashTags 는 데이터 상으로만 존재하여 Export 가 불가능합니다.
    }
    public void ImportTable<T>(DataSet dataSet, ref List<T> runtimeTable) where T : ITable
    {
        // runtimeTable.TableName 으로 서버에서 테이블을 특정합니다.
        DataTable dataTable = dataSet.Tables[runtimeTable[0].TableName];

        // 모든 런타임 테이블 필드 정보 조회
        FieldInfo[] runtimeTableFieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // 서버 테이블의 레코드 마다
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            // 모든 필드를 런타임 필드 타입에 맞게 컨버팅 후
            System.Object[] fields = new System.Object[dataTable.Columns.Count];
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                // if Enum / Else 나머지 컨버팅
                if (runtimeTableFieldInfos[j + 1].FieldType.IsEnum) fields[j] = Enum.Parse(runtimeTableFieldInfos[j + 1].FieldType, dataTable.Rows[i][j].ToString());
                else fields[j] = Convert.ChangeType(dataTable.Rows[i][j], runtimeTableFieldInfos[j + 1].FieldType);
            }

            // 레코드를 생성한 뒤 런타임 데이터베이스에서 해당 ID 와 매칭되는 레코드 특정
            T record = (T)(Activator.CreateInstance(typeof(T), fields));
            T runtimeRecord = (T)runtimeTable.OfType<ITable>().FirstOrDefault(item => item.ID == record.ID);

            // 기존 레코드가 있으면 수정, 없으면 생성
            if (runtimeRecord != null)
            {
                foreach (var property in typeof(T).GetProperties()) property.SetValue(runtimeRecord, property.GetValue(record));
                foreach (var field in typeof(T).GetFields()) field.SetValue(runtimeRecord, field.GetValue(record));
            }
            if (runtimeRecord == null) runtimeTable.Add(record);
        }
    }
    public void ExportHP(string ID, ref float hp_max, ref bool hideFullHp)
    {
        Table_HP data = HP.FirstOrDefault(hp => hp.ID == ID);
        hp_max = data.Hp_max;
        hideFullHp = data.HideFullHP;
    }
    public void ExportAbocado(string ID, ref EnumData.Abocado level, ref int quality, ref int quality_max, ref int harvest, ref int harvestPlus)
    {
        Table_Abocado data = Abocado.FirstOrDefault(abocado => abocado.ID == ID);
        level = data.level;
        quality = data.quality;
        quality_max = data.quality_max;
        harvest = data.harvest;
        harvestPlus = data.harvestPlus;
    }
    public void ExportGunner(string ID, ref float delay, ref float delay_fire, ref float detection, ref int subCount)
    {
        Table_Gunner data = Gunner.FirstOrDefault(gunner => gunner.ID == ID);
        delay = data.delay;
        delay_fire = data.delay_fire;
        detection = data.detection;
        subCount = data.subCount;
    }
    public void ExportAuto(string ID, ref float delay, ref float detection, ref float angle, ref int subCount, ref int subCountPlus)
    {
        Table_Auto data = Auto.FirstOrDefault(auto => auto.ID == ID);
        delay = data.delay;
        detection = data.detection;
        angle = data.angle;
        subCount = data.subCount;
        subCountPlus = data.subCountPlus;
    }
    public void ExportGuard(string ID, ref float hpMultiply)
    {
        Table_Guard data = Guard.FirstOrDefault(guard => guard.ID == ID);
        hpMultiply = data.hpMultiply;
    }
    public void ExportSplash(string ID, ref float delay, ref float detection)
    {
        Table_Splash data = Splash.FirstOrDefault(splash => splash.ID == ID);
        delay = data.delay;
        detection = data.detection;
    }
    public void ExportHeal(string ID, ref float delay, ref float detection, ref float ratio)
    {
        Table_Heal data = Heal.FirstOrDefault(heal => heal.ID == ID);
        delay = data.delay;
        detection = data.detection;
        ratio = data.ratio;
    }
    public void ExportLight(string ID, ref UnityEngine.Color color, ref float radius, ref float intensity, ref float onTime, ref float keepTime, ref float offTime, ref float frame)
    {
        Table_Light data = Light.FirstOrDefault(light => light.ID == ID);
        ExportColor(data.colorID, ref color.r, ref color.g, ref color.b, ref color.a);
        radius = data.radius;
        intensity = data.intensity;
        onTime = data.onTime;
        keepTime = data.keepTime;
        offTime = data.offTime;
        frame = data.frame;
    }
    public void ExportColor(string ID, ref float r, ref float g, ref float b, ref float a)
    {
        Table_Color data = Color.FirstOrDefault(color => color.ID == ID);
        r = data.r;
        g = data.g;
        b = data.b;
        a = data.a;
    }
    public void ExportExplosion(string ID, out Vector3 scale, ref float radius, ref float damage, ref float time)
    {
        Table_Explosion data = Explosion.FirstOrDefault(explosion => explosion.ID == ID);
        scale = new Vector3(data.scale, data.scale);
        radius = data.radius;
        damage = data.damage;
        time = data.time;
    }
    public void ExportProjectile(string ID, ref string[] clashTags, ref float damage, ref int penetrate)
    {
        Table_Projectile data = Projectile.FirstOrDefault(projectile => projectile.ID == ID);
        ExportClashTags(data.clashTagsID, ref clashTags);
        damage = data.damage;
        penetrate = data.penetrate;
    }
    public void ExportClashTags(string ID, ref string[] clashTags)
    {
        Table_ClashTags data = ClashTags.FirstOrDefault(clashTags => clashTags.ID == ID);
        clashTags = data.clashTags.Split(',');
    }
}