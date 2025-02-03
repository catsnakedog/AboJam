using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    /* Field & Property */
    public static LocalData instance;
    private string dataPath;
    private string idPath;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        dataPath = Path.Combine(Application.persistentDataPath, "gameData.json");
        idPath = Path.Combine(Application.persistentDataPath, "idData.json");
    }

    /* Public Methdo */
    /// <summary>
    /// GameData 를 로컬에 JSON 으로 저장합니다.
    /// </summary>
    /// <param name="data"></param>
    public void SaveDatas(List<GameData> dataList)
    {
        GameDataList wrapper = new GameDataList { gameDataList = dataList };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(dataPath, json);
        Debug.Log("게임 데이터 리스트 저장 완료!");
    }
    /// <summary>
    /// 로컬에서 JSON 을 GameData 형식으로 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public List<GameData> LoadDatas()
    {
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            GameDataList wrapper = JsonUtility.FromJson<GameDataList>(json);
            return wrapper.gameDataList;
        }
        else
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
            return null;
        }
    }
    /// <summary>
    /// ID 를 로컬에 JSON 으로 저장합니다.
    /// </summary>
    public void SaveID(string ID)
    {
        IDData wrapper = new IDData { ID = ID };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(idPath, json);
        Debug.Log("클라이언트 ID 저장 완료!");
    }
    /// <summary>
    /// 로컬에서 JSON 을 ID 형식으로 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public string LoadID()
    {
        if (File.Exists(idPath))
        {
            string json = File.ReadAllText(idPath);
            IDData wrapper = JsonUtility.FromJson<IDData>(json);
            return wrapper.ID;
        }
        else
        {
            Debug.LogWarning("저장된 ID 가 없습니다.");
            return null;
        }
    }
}