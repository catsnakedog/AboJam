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
    public Table_Light[] Light =
    {
        new Table_Light("Light_Explosion_Projectile_Auto0", "Color_Light_Explosion_Projectile_Auto0", 1f, 3f, 0.1f, 0.1f, 0.1f, 60),
        new Table_Light("Light_Explosion_Projectile_Gunner0", "Color_Light_Explosion_Projectile_Gunner0", 1f, 3f, 0.1f, 0.1f, 0.1f, 60),
        new Table_Light("Light_Explosion_Projectile_Heal0", "Color_Light_Explosion_Projectile_Heal0", 1f, 3f, 0.1f, 0.1f, 0.1f, 60),
        new Table_Light("Light_Explosion_Projectile_Heal1", "Color_Light_Explosion_Projectile_Heal1", 1f, 5f, 0.1f, 0.1f, 0.1f, 60),
        new Table_Light("Light_Explosion_Projectile_Splash0", "Color_Light_Explosion_Projectile_Splash0", 3f, 5f, 0.2f, 0.3f, 0.2f, 60),
        new Table_Light("Light_Explosion_Projectile_Splash1", "Color_Light_Explosion_Projectile_Splash1", 5f, 3f, 0.1f, 0.1f, 0.1f, 60),
        new Table_Light("Light_Tower", "Color_Light_Tower", 1.5f, 5f, 0.1f, 99999f, 0.1f, 60)
    };
    public Table_Color[] Color =
    {
        new Table_Color("Color_Light_Explosion_Projectile_Auto0", 253f, 255f, 0f, 1f),
        new Table_Color("Color_Light_Explosion_Projectile_Gunner0", 253f, 255f, 0f, 1f),
        new Table_Color("Color_Light_Explosion_Projectile_Heal0", 0f, 156f, 255f, 1f),
        new Table_Color("Color_Light_Explosion_Projectile_Heal1", 94f, 255f, 0, 1f),
        new Table_Color("Color_Light_Explosion_Projectile_Splash0", 255f, 45f, 0f, 1f),
        new Table_Color("Color_Light_Explosion_Projectile_Splash1", 255f, 0f, 255f, 1f),
        new Table_Color("Color_Light_Tower", 255f, 255f, 255f, 1f),
    };
    public Table_Explosion[] Explosion =
    {
        new Table_Explosion("Explosion_Projectile_Auto0", 2f, 0f, 0f, 1f),
        new Table_Explosion("Explosion_Projectile_Gunner0", 2f, 0f, 0f, 1f),
        new Table_Explosion("Explosion_Projectile_Heal0", 5f, 0f, 0f, 0.8f),
        new Table_Explosion("Explosion_Projectile_Heal1", 3f, 0f, 0f, 0.2f),
        new Table_Explosion("Explosion_Projectile_Splash0", 5f, 1.5f, 20f, 0.5f),
        new Table_Explosion("Explosion_Projectile_Splash1", 6f, 3f, 40f, 0.5f),
    };
    public Table_Projectile[] Projectile =
    {
        new Table_Projectile("Projectile_Player", "PlayerTargetTags", 3f, 1),
        new Table_Projectile("Projectile_Auto0", "AutoTargetTags", 3f, 1),
        new Table_Projectile("Projectile_Gunner0", "EnemyTargetTags", 3f, 1),
        new Table_Projectile("Projectile_Heal0", "HealTargetTags", -7f, 2),
        new Table_Projectile("Projectile_Heal1", "HealTargetTags", -11f, 2),
        new Table_Projectile("Projectile_Splash0", "SplashTargetTags", 10f, 1),
        new Table_Projectile("Projectile_Splash1", "SplashTargetTags", 15f, 1),
    };
    public Table_ClashTags[] ClashTags =
    {
        new Table_ClashTags("PlayerTargetTags", "Enemies"),
        new Table_ClashTags("EnemyTargetTags", "Player,Towers,Abocados"),
        new Table_ClashTags("AutoTargetTags", "Enemies"),
        new Table_ClashTags("HealTargetTags","Player,Towers,Abocados"),
        new Table_ClashTags("SplashTargetTags", "Enemies"),
    }; // 제 1 형 정규화를 위한 수정이 너무 많아 테이블 분리만 해뒀습니다.

    /* Public Method */
    // Import : 서버 DB >> 런타임 DB
    // Export : 런타임 DB >> 인스턴스
    public void ImportDatabase()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            ImportTable(dataSet, ref Abocado);
            ImportTable(dataSet, ref Gunner);
            ImportTable(dataSet, ref Auto);
            ImportTable(dataSet, ref Guard);
            ImportTable(dataSet, ref Splash);
            ImportTable(dataSet, ref Heal);
            ImportTable(dataSet, ref HP);
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
    public void ImportTable<T>(DataSet dataSet, ref T[] runtimeTable) where T : ITable
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

            // 레코드를 생성한 뒤
            T record = (T)(Activator.CreateInstance(typeof(T), fields));

            // 런타임 데이터베이스에서 해당 ID 와 매칭되는 레코드 특정
            T runtimeRecord = (T)runtimeTable.OfType<ITable>().FirstOrDefault(item => item.ID == record.ID);
            if (runtimeRecord == null) { Debug.Log($"{record.TableName} 에 {record.ID} 와 매칭되는 레코드가 없습니다."); return; }

            // 런타임 데이터베이스 레코드에 생성한 레코드 할당
            foreach (var property in typeof(T).GetProperties()) property.SetValue(runtimeRecord, property.GetValue(record));
            foreach (var field in typeof(T).GetFields()) field.SetValue(runtimeRecord, field.GetValue(record));
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
    public void ExportLight(string ID, ref Color color, ref float radius, ref float intensity, ref float onTime, ref float keepTime, ref float offTime, ref float frame)
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