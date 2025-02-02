using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    /* Field & Property */
    public static LocalData instance;
    private string path;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        path = Path.Combine(Application.persistentDataPath, "gamedata.json");
    }

    /* Public Methdo */
    /// <summary>
    /// GameData 를 로컬에 JSON 으로 저장합니다.
    /// </summary>
    /// <param name="data"></param>
    public void Save(List<GameData> dataList)
    {
        GameDataList wrapper = new GameDataList { gameDataList = dataList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
        Debug.Log("게임 데이터 리스트 저장 완료!");
    }
    /// <summary>
    /// 로컬에서 JSON 을 GameData 형식으로 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public List<GameData> Load()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameDataList wrapper = JsonUtility.FromJson<GameDataList>(json);
            return wrapper.gameDataList;
        }
        else
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
            return null;
        }
    }
}