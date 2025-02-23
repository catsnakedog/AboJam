using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Pool;

/// <summary>
/// <br>런타임 데이터베이스입니다. (현재 Script Execution Order : 100)</br>
/// <br>CONSTRAINT : 반드시 모든 레코드 인스턴스보다 먼저 Awake 가 호출되어야 합니다.</br>
/// <br>CONSTRAINT : 런타임 테이블 이름은 반드시 서버 테이블 이름과 같아야 합니다.</br>
/// </summary>
public class Database_AboJam : MonoBehaviour
{
    /* Dependency */
    private DBMS dbms => DBMS.instance;
    private Pool pool => Pool.instance;

    /* Field & Property */
    public static Database_AboJam instance;

    /* Initializer */
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private void Awake() { instance = this; }

    /* Runtime Table : ImportTable 시 서버 데이터로 덮어씌워집니다. */
    public List<Table_HP> HP = new() { new Table_HP("ID", Hp_max: 1f, HideFullHP: true) };
    public List<Table_Launcher> Launcher = new() { new Table_Launcher("ID", align: true, turnTime: 1f, angleOffset: 1f, frame: 1f, speed: 1f, range: 1f) };
    public List<Table_Melee> Melee = new() { new Table_Melee("ID", "clashTagsID", penetrate: 1, radius: 1f, damage: 1f, effectTime: 1f, knockback: 1f) };
    public List<Table_Abocado> Abocado = new() { new Table_Abocado("ID", level: EnumData.Abocado.Cultivated, quality: 1, quality_max: 1, harvest: 1, harvestPlus: 1) };
    public List<Table_Gunner> Gunner = new() { new Table_Gunner("ID", delay: 1f, delay_fire: 1f, detection: 1f, subCount: 1) };
    public List<Table_Leopard> Leopard = new() { new Table_Leopard("ID", delay: 1f, detection: 1f) };
    public List<Table_Thug> Thug = new() { new Table_Thug("ID", delay: 1f, detection: 1f) };
    public List<Table_Auto> Auto = new() { new Table_Auto("ID", "reinforceCostID", delay: 1f, detection: 1f, angle: 1f, subCount: 1, subCountPlus: 1) };
    public List<Table_Guard> Guard = new() { new Table_Guard("ID", "reinforceCostID", hpMultiply: 1f) };
    public List<Table_Splash> Splash = new() { new Table_Splash("ID", "reinforceCostID", delay: 1f, detection: 1f) };
    public List<Table_Heal> Heal = new() { new Table_Heal("ID", "reinforceCostID", delay: 1f, detection: 1f, ratio: 1f) };
    public List<Table_ReinforceCost> ReinforceCost = new() { new Table_ReinforceCost("ID", reinforceCost: 1) };
    public List<Table_Light> Light = new() { new Table_Light("ID", "colorID", radius: 1f, intensity: 1f, onTime: 1f, keepTime: 1f, offTime: 1f, frame: 1) };
    public List<Table_Color> Color = new() { new Table_Color("ID", r: 1f, g: 1f, b: 1f, a: 1f) };
    public List<Table_Explosion> Explosion = new() { new Table_Explosion("ID", scale: 1f, radius: 1f, damage: 1f, time: 1f) };
    public List<Table_Projectile> Projectile = new() { new Table_Projectile("ID", "clashTagsID", damage: 1f, penetrate: 1) };
    public List<Table_ClashTags> ClashTags = new() { new Table_ClashTags("ID", "clashTags") };
    public List<Table_Spawnee> Spawnee = new() { new Table_Spawnee("ID", "prefab", level: 0) };
    public List<Table_Sector> Sector = new() { new Table_Sector("ID", angleStart: 1f, angleEnd: 1f, radiusIn: 1f, radiusOut: 1f) };
    public List<Table_Spawn> Spawn = new() { new Table_Spawn("ID", "sectorID", "spawneeID", interval: 1f, count: 1) };
    public List<Table_Wave> Wave = new() { new Table_Wave("ID", delay: 1f, "spawnID") };
    public List<Table_Date> Date = new() { new Table_Date("ID", secondsPerDay: 1, "startTime", "morningTime", "sunsetTime", "nightTime") };
    public List<Table_Upgrade> Upgrade = new() { new Table_Upgrade("ID", "reinforceCostID", coefficient: 0.1f) };
    public List<Table_Skill> Skill = new() { new Table_Skill("ID", cooldown: 1f, range: 1, count: 1, seconds: 1f, speed: 1) };

    /* Public Method */
    // Import : 서버 DB >> 런타임 DB
    // Export : 런타임 DB >> 인스턴스
    public void ImportDatabase()
    {
        try
        {
            DataSet dataSet = dbms.GetDataSet();

            ImportTable(dataSet, ref HP);
            ImportTable(dataSet, ref Launcher);
            ImportTable(dataSet, ref Melee);
            ImportTable(dataSet, ref Abocado);
            ImportTable(dataSet, ref Gunner);
            ImportTable(dataSet, ref Leopard);
            ImportTable(dataSet, ref Thug);
            ImportTable(dataSet, ref Auto);
            ImportTable(dataSet, ref Guard);
            ImportTable(dataSet, ref Splash);
            ImportTable(dataSet, ref Heal);
            ImportTable(dataSet, ref ReinforceCost);
            ImportTable(dataSet, ref Light);
            ImportTable(dataSet, ref Color);
            ImportTable(dataSet, ref Explosion);
            ImportTable(dataSet, ref Projectile);
            ImportTable(dataSet, ref ClashTags);
            ImportTable(dataSet, ref Spawnee);
            ImportTable(dataSet, ref Sector);
            ImportTable(dataSet, ref Spawn);
            ImportTable(dataSet, ref Wave);
            ImportTable(dataSet, ref Date);
            ImportTable(dataSet, ref Upgrade);
            ImportTable(dataSet, ref Skill);
        }
        catch (Exception) { Debug.Log("서버와의 연결이 원활하지 않거나, 잘못된 데이터가 존재합니다."); throw; }
    }
    public void ExportDatabase()
    {
        foreach (HP item in global::HP.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Launcher item in global::Launcher.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Melee item in global::Melee.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Abocado item in global::Abocado.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Gunner item in global::Gunner.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Leopard item in global::Leopard.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Thug item in global::Thug.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Auto item in global::Auto.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Guard item in global::Guard.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Splash item in global::Splash.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Heal item in global::Heal.instances) if (item.isActiveAndEnabled) item.Load();
        // Readonly : ReinforceCost
        foreach (Light item in global::Light.instances) if (item.isActiveAndEnabled) item.Load();
        // Readonly : Color
        foreach (Explosion item in global::Explosion.instances) if (item.isActiveAndEnabled) item.Load();
        foreach (Projectile item in global::Projectile.instances) if (item.isActiveAndEnabled) item.Load();
        // Readonly : ClashTags
        // Readonly : Spawnee
        // Readonly : Sector
        // Readonly : Spawn
        // Readonly : Wave
        if (global::Date.instance.isActiveAndEnabled) global::Date.instance.Load();
        foreach (BTN_Upgrade item in global::BTN_Upgrade.instances) if (item.isActiveAndEnabled) item.Load();
        if (global::Skill.instance.isActiveAndEnabled) global::Skill.instance.Load();
    }
    public void ImportTable<T>(DataSet dataSet, ref List<T> runtimeTable) where T : ITable
    {
        // runtimeTable.TableName 으로 서버에서 테이블을 특정
        DataTable dataTable = dataSet.Tables[runtimeTable[0].TableName];

        // 런타임 테이블 초기화
        runtimeTable.Clear();

        // 모든 런타임 테이블 필드 정보 조회
        FieldInfo[] runtimeTableFieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // 서버 테이블의 레코드 마다
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            // 모든 필드를 런타임 필드 타입에 맞게 컨버팅 후
            System.Object[] fields = new System.Object[dataTable.Columns.Count];
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                try
                {
                    // if Enum / Else 나머지 컨버팅
                    if (runtimeTableFieldInfos[j + 1].FieldType.IsEnum) fields[j] = Enum.Parse(runtimeTableFieldInfos[j + 1].FieldType, dataTable.Rows[i][j].ToString());
                    else fields[j] = Convert.ChangeType(dataTable.Rows[i][j], runtimeTableFieldInfos[j + 1].FieldType);
                }
                catch (Exception)
                {
                    Debug.Log($"서버 테이블 {dataTable.TableName} 의 컬럼 {dataTable.Columns[j].ColumnName} 를 {runtimeTableFieldInfos[j + 1].FieldType} 로의 타입 변환이 실패했습니다.");
                    Debug.Log("서버 테이블과 런타임 테이블의 컬럼 개수, 타입이 정확히 일치하는지 확인해주세요.");
                    throw;
                }
            }

            // 런타임 테이블에 생성한 레코드 추가
            runtimeTable.Add((T)Activator.CreateInstance(typeof(T), fields));
        }
    }
    public void ExportHP(string ID, ref float hp_max, ref bool hideFullHp)
    {
        Table_HP data = HP.FirstOrDefault(hp => hp.ID == ID);
        hp_max = data.Hp_max;
        hideFullHp = data.HideFullHP;
    }
    public void ExportLauncher(string ID, ref bool align, ref float turnTime, ref float angleOffset, ref float frame, ref float speed, ref float range)
    {
        Table_Launcher data = Launcher.FirstOrDefault(launcher => launcher.ID == ID);

        align = data.align;
        turnTime = data.turnTime;
        angleOffset = data.angleOffset;
        frame = data.frame;
        speed = data.speed;
        range = data.range;
    }
    public void ExportMelee(string ID, ref string[] clashTags, ref int penetrate, ref float radius, ref float damage, ref float effectTime, ref float knockback)
    {
        Table_Melee data = Melee.FirstOrDefault(melee => melee.ID == ID);
        ExportClashTags(data.clashTagsID, ref clashTags);
        penetrate = data.penetrate;
        radius = data.radius;
        damage = data.damage;
        effectTime = data.effectTime;
        knockback = data.knockback;
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
    public void ExportLeopard(string ID, ref float delay, ref float detection)
    {
        Table_Leopard data = Leopard.FirstOrDefault(Leopard => Leopard.ID == ID);
        delay = data.delay;
        detection = data.detection;
    }
    public void ExportThug(string ID, ref float delay, ref float detection)
    {
        Table_Thug data = Thug.FirstOrDefault(Thug => Thug.ID == ID);
        delay = data.delay;
        detection = data.detection;
    }
    public void ExportAuto(string ID, ref int[] reinforceCost, ref float delay, ref float detection, ref float angle, ref int subCount, ref int subCountPlus)
    {
        Table_Auto data = Auto.FirstOrDefault(auto => auto.ID == ID);
        ExportReinforceCost(data.reinforceCostID, ref reinforceCost);
        delay = data.delay;
        detection = data.detection;
        angle = data.angle;
        subCount = data.subCount;
        subCountPlus = data.subCountPlus;
    }
    public void ExportGuard(string ID, ref int[] reinforceCost, ref float hpMultiply)
    {
        Table_Guard data = Guard.FirstOrDefault(guard => guard.ID == ID);
        ExportReinforceCost(data.reinforceCostID, ref reinforceCost);
        hpMultiply = data.hpMultiply;
    }
    public void ExportSplash(string ID, ref int[] reinforceCost, ref float delay, ref float detection)
    {
        Table_Splash data = Splash.FirstOrDefault(splash => splash.ID == ID);
        ExportReinforceCost(data.reinforceCostID, ref reinforceCost);
        delay = data.delay;
        detection = data.detection;
    }
    public void ExportHeal(string ID, ref int[] reinforceCost, ref float delay, ref float detection, ref float ratio)
    {
        Table_Heal data = Heal.FirstOrDefault(heal => heal.ID == ID);
        ExportReinforceCost(data.reinforceCostID, ref reinforceCost);
        delay = data.delay;
        detection = data.detection;
        ratio = data.ratio;
    }
    public void ExportReinforceCost(string ID, ref int[] reinforceCost)
    {
        reinforceCost = ReinforceCost
            .Where(reinforceCost => reinforceCost.ID == ID)
            .Select(data => data.reinforceCost)
            .ToArray();
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
        clashTags = ClashTags
            .Where(clashTags => clashTags.ID == ID)
            .Select(data => data.clashTags)
            .ToArray();
    }
    public void ExportWave(string ID, ref float[] delay, ref Spawn[] spawn)
    {
        // 런타임 테이블 Wave 중 ID 가 같은 모든 레코드 선택
        Table_Wave[] datas = Wave.Where(wave => wave.ID == ID).ToArray();

        // 레코드들에서 delay[] 와 spawn[] 추출하여 할당
        delay = datas.Select(data => data.delay).ToArray();
        spawn = datas.Select(data =>
        {
            Spawn record = new Spawn();
            ExportSpawn(data.spawnID, ref record.sector, ref record.spawnee, ref record.interval, ref record.count);
            return record;
        }).ToArray();
    }
    public void ExportSpawn(string ID, ref Sector sector, ref Spawnee spawnee, ref float interval, ref int count)
    {
        Table_Spawn data = Spawn.FirstOrDefault(spawn => spawn.ID == ID);

        Sector record_sector = new();
        ExportSector(data.sectorID, ref record_sector.angleStart, ref record_sector.angleEnd, ref record_sector.radiusIn, ref record_sector.radiusOut);
        sector = record_sector;

        Spawnee record_spawnee = new();
        ExportSpawnee(data.spawneeID, ref record_spawnee.prefabs, ref record_spawnee.levels);
        spawnee = record_spawnee;

        interval = data.interval;
        count = data.count;
    }
    public void ExportSector(string ID, ref float angleStart, ref float angleEnd, ref float radiusIn, ref float radiusOut)
    {
        Table_Sector data = Sector.FirstOrDefault(sector => sector.ID == ID);

        angleStart = data.angleStart;
        angleEnd = data.angleEnd;
        radiusIn = data.radiusIn;
        radiusOut = data.radiusOut;
    }
    public void ExportSpawnee(string ID, ref GameObject[] prefabs, ref int[] levels)
    {
        List<GameObject> temp = new();

        foreach (string prefabName in Spawnee.Where(spawnee => spawnee.ID == ID).Select(data => data.prefab).ToArray())
        {
            foreach (ObjectGroup group in pool.objectGroups)
            {
                GameObject prefab = group.objects.FirstOrDefault(obj => obj.name == prefabName);
                if (prefab != null) { temp.Add(prefab); break; }
            }
        }

        prefabs = temp.ToArray();
        levels = Spawnee.Where(spawnee => spawnee.ID == ID).Select(data => data.level).ToArray();
    }
    public void ExportDate(string ID, ref int secondsPerDay, ref string startTime, ref string morningTime, ref string sunsetTime, ref string nightTime)
    {
        Table_Date data = Date.FirstOrDefault(date => date.ID == ID);
        secondsPerDay = data.secondsPerDay;
        startTime = data.startTime;
        morningTime = data.morningTime;
        sunsetTime = data.sunsetTime;
        nightTime = data.nightTime;
    }
    public void ExportUpgrade(string ID, ref int[] reinforceCost, ref float coefficient)
    {
        Table_Upgrade data = Upgrade.FirstOrDefault(upgrade => upgrade.ID == ID);
        ExportReinforceCost(data.reinforceCostID, ref reinforceCost);
        coefficient = data.coefficient;
    }
    public void ExportSkill(string ID, ref float cooldown, ref int range, ref int count, ref float seconds, ref float speed)
    {
        Table_Skill data = Skill.FirstOrDefault(skill => skill.ID == ID);
        cooldown = data.cooldown;
        range = data.range;
        count = data.count;
        seconds = data.seconds;
        speed = data.speed;
    }
}