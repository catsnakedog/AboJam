using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static EnumData;
using static ObjectPool;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public struct AbocadoData
{
    public EnumData.Abocado level;
    public int quality;
    public Vector3 pos;
}

[Serializable]
public class AbocadoWrapper
{
    public List<AbocadoData> list;
}

internal class BuildPreset : MonoBehaviour
{
    private string abocadoPath => Path.Combine(Application.persistentDataPath, "abocados.json");
    private string towerPath => Path.Combine(Application.persistentDataPath, "towers.json");

    /// <summary>
    /// 모든 건물을 저장합니다.
    /// </summary>
    public void Save()
    {
        SaveAllAbocado();
    }

    /// <summary>
    /// 모든 건물을 불러옵니다.
    /// </summary>
    public void Load()
    {
        KillAllAbocado(0);
        KillAllTower(0);

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
                mono.StartCoroutine(tower.CorDeath(seconds));
    }
    /// <summary>
    /// 현재 필드의 모든 아보카도 데이터를 저장합니다.
    /// </summary>
    private void SaveAllAbocado()
    {
        List<AbocadoData> datas = new List<AbocadoData>();

        foreach (Abocado abocado in Abocado.instances)
        {
            if(abocado.gameObject.activeSelf)
            {
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
    /// 로컬에서 아보카도 데이터를 불러옵니다.
    /// </summary>
    public List<AbocadoData> LoadAllAbocado()
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
    /// 아보카도에 데이터를 덮어씁니다.
    /// </summary>
    private void OverrideAbocado(Abocado abocado, AbocadoData abocadoData)
    {
        while (abocado.Level < abocadoData.level) abocado.GrowUp(true);
        while (abocado.Quality < abocadoData.quality) abocado.Promote();
    }
}
