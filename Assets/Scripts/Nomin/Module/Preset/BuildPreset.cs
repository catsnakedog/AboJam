using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using static EnumData;
using static ObjectPool;
using static UnityEngine.GraphicsBuffer;

[Serializable] internal struct AbocadoData { public EnumData.Abocado level; public int quality; public Vector3 pos; }
[Serializable] internal struct AutoData { public int level; public Vector3 pos; }
[Serializable] internal struct GuardData { public int level; public Vector3 pos; }
[Serializable] internal struct SplashData { public int level; public Vector3 pos; }
[Serializable] internal struct HealData { public int level; public Vector3 pos; }
[Serializable] internal class AbocadoWrapper { public List<AbocadoData> list; }
[Serializable] internal class AutoWrapper { public List<AutoData> list; }
[Serializable] internal class GuardWrapper { public List<GuardData> list; }
[Serializable] internal class SplashWrapper { public List<SplashData> list; }
[Serializable] internal class HealWrapper { public List<HealData> list; }
[Serializable] internal class CountWrapper { public int count; }

internal class BuildPreset : MonoBehaviour
{
    /* Field */
    [SerializeField] private int index; public int Index { get => index; }
    [SerializeField] private TextMeshProUGUI tmp;
    private string folderpath => Path.Combine(Application.persistentDataPath, @"preset");
    private string abocadoPath => Path.Combine(folderpath, $"abocados{index}.json");
    private string autoPath => Path.Combine(folderpath, $"autos{index}.json");
    private string guardPath => Path.Combine(folderpath, $"guards{index}.json");
    private string splashPath => Path.Combine(folderpath, $"splashes{index}.json");
    private string healPath => Path.Combine(folderpath, $"heals{index}.json");
    private string countPath => Path.Combine(folderpath, $"count{index}.json");

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        DrawCount(LoadCount());
    }

    /* Public Method */
    /// <summary>
    /// 모든 건물을 저장합니다.
    /// </summary>
    public void Save()
    {
        if (!Directory.Exists(folderpath)) Directory.CreateDirectory(folderpath);
        SaveAllAbocado(out int savedAbocadoCount);
        SaveAllAuto(out int savedAutoCount);
        SaveAllGuard(out int savedGuardCount);
        SaveAllSplash(out int savedSplashCount);
        SaveAllHeal(out int savedHealCount);

        // UI 에 총 건물 숫자를 표기하고, 로컬에 저장합니다.
        int totalSavedCount = savedAbocadoCount + savedAutoCount + savedGuardCount + savedSplashCount + savedHealCount;
        SaveCount(totalSavedCount);
        DrawCount(totalSavedCount);
    }
    /// <summary>
    /// 모든 건물을 불러옵니다.
    /// </summary>
    public void Load()
    {
        KillAllAbocado(0);
        KillAllTower(0);

        RestoreAbocado();
        RestoreAuto();
        RestoreGuard();
        RestoreSplash();
        RestoreHeal();
    }

    /* Private Method */
    /// <summary>
    /// 저장된 아보카도를 불러와 실체화합니다.
    /// </summary>
    private void RestoreAbocado()
    {
        List<AbocadoData> abocadoDatas = LoadAllAbocado();
        foreach (AbocadoData abocadoData in abocadoDatas)
        {
            // 신규 아보카도 생성
            GameObject newObject = Pool.instance.Get("Abocado");
            Abocado newAbocado = newObject.GetComponent<Abocado>();

            // 타일과 바인딩
            Grid.instance
                .GetNearestTile(abocadoData.pos)
                .Bind(newObject, EnumData.TileIndex.AboCado);

            OverrideAbocado(newAbocado, abocadoData);
        }
    }
    /// <summary>
    /// 저장된 Auto 타워를 불러와 실체화합니다.
    /// </summary>
    private void RestoreAuto()
    {
        List<AutoData> autoDatas = LoadAllAuto();
        foreach (AutoData autoData in autoDatas)
        {
            // 신규 Auto 타워 생성
            GameObject newObject = Pool.instance.Get("Auto");
            Auto newAuto = newObject.GetComponent<Auto>();

            // 타일과 바인딩
            Grid.instance
                .GetNearestTile(autoData.pos)
                .Bind(newObject, EnumData.TileIndex.Auto);

            OverrideAuto(newAuto, autoData);
        }
    }
    /// <summary>
    /// 저장된 Guard 타워를 불러와 실체화합니다.
    /// </summary>
    private void RestoreGuard()
    {
        List<GuardData> guardDatas = LoadAllGuard();
        foreach (GuardData guardData in guardDatas)
        {
            // 신규 Guard 타워 생성
            GameObject newObject = Pool.instance.Get("Guard");
            Guard newGuard = newObject.GetComponent<Guard>();

            // 타일과 바인딩
            Grid.instance
                .GetNearestTile(guardData.pos)
                .Bind(newObject, EnumData.TileIndex.Guard);

            OverrideGuard(newGuard, guardData);
        }
    }
    /// <summary>
    /// 저장된 Splash 타워를 불러와 실체화합니다.
    /// </summary>
    private void RestoreSplash()
    {
        List<SplashData> splashDatas = LoadAllSplash();
        foreach (SplashData splashData in splashDatas)
        {
            // 신규 Splash 타워 생성
            GameObject newObject = Pool.instance.Get("Splash");
            Splash newSplash = newObject.GetComponent<Splash>();

            // 타일과 바인딩
            Grid.instance
                .GetNearestTile(splashData.pos)
                .Bind(newObject, EnumData.TileIndex.Splash);

            OverrideSplash(newSplash, splashData);
        }
    }
    /// <summary>
    /// 저장된 Heal 타워를 불러와 실체화합니다.
    /// </summary>
    private void RestoreHeal()
    {
        List<HealData> healDatas = LoadAllHeal();
        foreach (HealData healData in healDatas)
        {
            // 신규 Heal 타워 생성
            GameObject newObject = Pool.instance.Get("Heal");
            Heal newHeal = newObject.GetComponent<Heal>();

            // 타일과 바인딩
            Grid.instance
                .GetNearestTile(healData.pos)
                .Bind(newObject, EnumData.TileIndex.Heal);

            OverrideHeal(newHeal, healData);
        }
    }

    /// <summary>
    /// 모든 아보카도를 비활성화합니다.
    /// </summary>
    private void KillAllAbocado(float seconds)
    {
        foreach (Abocado abocado in Abocado.instances)
            if (abocado.gameObject.activeSelf)
                abocado.StartCoroutine(abocado.CorDeath(seconds));
    }
    /// <summary>
    /// 모든 타워를 비활성화합니다.
    /// </summary>
    private void KillAllTower(float seconds)
    {
        foreach (ITower tower in ITower.instances)
            if (tower is MonoBehaviour mono)
                if (mono.gameObject.activeSelf)
                    mono.StartCoroutine(tower.CorDeath(seconds));
    }

    /// <summary>
    /// 현재 필드의 모든 아보카도 데이터를 저장합니다.
    /// </summary>
    private void SaveAllAbocado(out int count)
    {
        List<AbocadoData> datas = new List<AbocadoData>();
        count = 0;

        foreach (Abocado abocado in Abocado.instances)
        {
            if (abocado.gameObject.activeSelf)
            {
                count++;
                AbocadoData data = new AbocadoData();
                data.level = abocado.Level;
                data.quality = abocado.Quality;
                data.pos = abocado.transform.position;
                datas.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new AbocadoWrapper { list = datas });
        File.WriteAllText(abocadoPath, json);
    }
    /// <summary>
    /// 현재 필드의 모든 Auto 타워 데이터를 저장합니다.
    /// </summary>
    private void SaveAllAuto(out int count)
    {
        List<AutoData> datas = new List<AutoData>();
        count = 0;

        foreach (Auto auto in Auto.instances)
        {
            if (auto.gameObject.activeSelf)
            {
                count++;
                AutoData data = new AutoData();
                data.level = auto.Level;
                data.pos = auto.transform.position;
                datas.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new AutoWrapper { list = datas });
        File.WriteAllText(autoPath, json);
    }
    /// <summary>
    /// 현재 필드의 모든 Guard 타워 데이터를 저장합니다.
    /// </summary>
    private void SaveAllGuard(out int count)
    {
        List<GuardData> datas = new List<GuardData>();
        count = 0;

        foreach (Guard guard in Guard.instances)
        {
            if (guard.gameObject.activeSelf)
            {
                count++;
                GuardData data = new GuardData();
                data.level = guard.Level;
                data.pos = guard.transform.position;
                datas.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new GuardWrapper { list = datas });
        File.WriteAllText(guardPath, json);
    }
    /// <summary>
    /// 현재 필드의 모든 Splash 타워 데이터를 저장합니다.
    /// </summary>
    private void SaveAllSplash(out int count)
    {
        List<SplashData> datas = new List<SplashData>();
        count = 0;

        foreach (Splash splash in Splash.instances)
        {
            if (splash.gameObject.activeSelf)
            {
                count++;
                SplashData data = new SplashData();
                data.level = splash.Level;
                data.pos = splash.transform.position;
                datas.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new SplashWrapper { list = datas });
        File.WriteAllText(splashPath, json);
    }
    /// <summary>
    /// 현재 필드의 모든 Heal 타워 데이터를 저장합니다.
    /// </summary>
    private void SaveAllHeal(out int count)
    {
        List<HealData> datas = new List<HealData>();
        count = 0;

        foreach (Heal heal in Heal.instances)
        {
            if (heal.gameObject.activeSelf)
            {
                count++;
                HealData data = new HealData();
                data.level = heal.Level;
                data.pos = heal.transform.position;
                datas.Add(data);
            }
        }

        string json = JsonUtility.ToJson(new HealWrapper { list = datas });
        File.WriteAllText(healPath, json);
    }

    /// <summary>
    /// 로컬에서 아보카도 데이터를 불러옵니다.
    /// </summary>
    private List<AbocadoData> LoadAllAbocado()
    {
        if (!File.Exists(abocadoPath))
        {
            Debug.Log("로컬에서 아보카도 데이터를 불러오지 못했습니다.");
            return new List<AbocadoData>();
        }

        string json = File.ReadAllText(abocadoPath);
        return JsonUtility.FromJson<AbocadoWrapper>(json).list;
    }
    /// <summary>
    /// 로컬에서 Auto 타워 데이터를 불러옵니다.
    /// </summary>
    private List<AutoData> LoadAllAuto()
    {
        if (!File.Exists(autoPath))
        {
            Debug.Log("로컬에서 Auto 타워 데이터를 불러오지 못했습니다.");
            return new List<AutoData>();
        }

        string json = File.ReadAllText(autoPath);
        return JsonUtility.FromJson<AutoWrapper>(json).list;
    }
    /// <summary>
    /// 로컬에서 Guard 타워 데이터를 불러옵니다.
    /// </summary>
    private List<GuardData> LoadAllGuard()
    {
        if (!File.Exists(guardPath))
        {
            Debug.Log("로컬에서 Guard 타워 데이터를 불러오지 못했습니다.");
            return new List<GuardData>();
        }

        string json = File.ReadAllText(guardPath);
        return JsonUtility.FromJson<GuardWrapper>(json).list;
    }
    /// <summary>
    /// 로컬에서 Splash 타워 데이터를 불러옵니다.
    /// </summary>
    private List<SplashData> LoadAllSplash()
    {
        if (!File.Exists(splashPath))
        {
            Debug.Log("로컬에서 Splash 타워 데이터를 불러오지 못했습니다.");
            return new List<SplashData>();
        }

        string json = File.ReadAllText(splashPath);
        return JsonUtility.FromJson<SplashWrapper>(json).list;
    }
    /// <summary>
    /// 로컬에서 Heal 타워 데이터를 불러옵니다.
    /// </summary>
    private List<HealData> LoadAllHeal()
    {
        if (!File.Exists(healPath))
        {
            Debug.Log("로컬에서 Heal 타워 데이터를 불러오지 못했습니다.");
            return new List<HealData>();
        }

        string json = File.ReadAllText(healPath);
        return JsonUtility.FromJson<HealWrapper>(json).list;
    }

    /// <summary>
    /// 아보카도에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideAbocado(Abocado abocado, AbocadoData abocadoData)
    {
        int repeatCount = 0;

        while (abocado.Level < abocadoData.level)
        {
            abocado.GrowUp(true);
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideAbocado)}");
                break;
            }
        }

        while (abocado.Quality < abocadoData.quality)
        {
            abocado.Promote();
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideAbocado)}");
                break;
            }
        }
    }
    /// <summary>
    /// Auto 타워에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideAuto(Auto auto, AutoData autoData)
    {
        int repeatCount = 0;

        while (auto.Level < autoData.level)
        {
            auto.Reinforce();
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideAuto)}");
                break;
            }
        }
    }
    /// <summary>
    /// Guard 타워에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideGuard(Guard guard, GuardData guardData)
    {
        int repeatCount = 0;

        while (guard.Level < guardData.level)
        {
            guard.Reinforce();
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideGuard)}");
                break;
            }
        }
    }
    /// <summary>
    /// Splash 타워에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideSplash(Splash splash, SplashData splashData)
    {
        int repeatCount = 0;

        while (splash.Level < splashData.level)
        {
            splash.Reinforce();
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideSplash)}");
                break;
            }
        }
    }
    /// <summary>
    /// Heal 타워에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideHeal(Heal heal, HealData healData)
    {
        int repeatCount = 0;

        while (heal.Level < healData.level)
        {
            repeatCount++;
            if (repeatCount > 100)
            {
                Debug.Log($"무한 재귀 버그 : {nameof(OverrideHeal)}");
                break;
            }
            heal.Reinforce();
        }
    }

    /// <summary>
    /// 로컬에 프리셋의 건물 숫자를 저장합니다.
    /// </summary>
    private void SaveCount(int count)
    {
        string json = JsonUtility.ToJson(new CountWrapper { count = count });
        File.WriteAllText(countPath, json);
    }
    /// <summary>
    /// 로컬에서 프리셋의 건물 숫자를 불러옵니다.
    /// </summary>
    private int LoadCount()
    {
        if (!File.Exists(countPath)) return 0;

        string json = File.ReadAllText(countPath);
        return JsonUtility.FromJson<CountWrapper>(json).count;
    }
    /// <summary>
    /// UI 에 프리셋의 건물 수를 표기합니다.
    /// </summary>
    private void DrawCount(int count)
    {
        tmp.text =  "총 건물 수 : "+ count.ToString();
    }
}
