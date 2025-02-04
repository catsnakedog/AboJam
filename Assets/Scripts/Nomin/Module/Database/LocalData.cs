using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    /* Dependency */
    private Message message => Message.instance;

    /* Field & Property */
    public static LocalData instance;
    private static readonly string Key = "MySecretKey42158"; // 16, 24, 32 바이트 키 사용
    private string dataPath;
    private string idPath;
    private string scenarioPath;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        dataPath = Path.Combine(Application.persistentDataPath, "gameData.json");
        idPath = Path.Combine(Application.persistentDataPath, "idData.json");
        scenarioPath = Path.Combine(Application.persistentDataPath, "scenarioData.json");
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
        json = EncryptJson(json);
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
            json = DecryptJson(json); if (json == null) { Debug.LogWarning("JSON 이 위변조 되었습니다."); return null; }
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
        json = EncryptJson(json);
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
            json = DecryptJson(json); if (json == null) { Debug.LogWarning("JSON 이 위변조 되었습니다."); return null; }
            IDData wrapper = JsonUtility.FromJson<IDData>(json);
            return wrapper.ID;
        }
        else
        {
            Debug.LogWarning("저장된 ID 가 없습니다.");
            return null;
        }
    }
    /// <summary>
    /// 시나리오 클리어 정보를 로컬에 JSON 으로 저장합니다.
    /// </summary>
    public void SaveScenario(int scenarioNumber)
    {
        ScenarioData wrapper = new ScenarioData { scenarioNumber = scenarioNumber };
        string json = JsonUtility.ToJson(wrapper, true);
        json = EncryptJson(json);
        File.WriteAllText(scenarioPath, json);
        Debug.Log("클라이언트 ScenarioNumber 저장 완료!");
    }
    /// <summary>
    /// 로컬에서 JSON 을 시나리오 형식으로 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public int LoadScenario()
    {
        if (File.Exists(scenarioPath))
        {
            string json = File.ReadAllText(scenarioPath);
            json = DecryptJson(json); if (json == null) { Debug.LogWarning("JSON 이 위변조 되었습니다."); return 0; }
            ScenarioData wrapper = JsonUtility.FromJson<ScenarioData>(json);
            return wrapper.scenarioNumber;
        }
        else
        {
            Debug.LogWarning("저장된 시나리오 정보가 없습니다.");
            return 0;
        }
    }

    /* private Method */
    /// <summary>
    /// JSON 을 암호화합니다.
    /// </summary>
    private string EncryptJson(string jsonData)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[16]; // IV는 기본값 사용

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(jsonBytes, 0, jsonBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }
    /// <summary>
    /// JSON 을 복호화합니다.
    /// </summary>
    private string DecryptJson(string encryptedJson)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedJson);
                    byte[] jsonBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(jsonBytes);
                }
            }
        }
        catch { message.On("JSON 데이터 위조 감지 : JSON 파일을 삭제해 주세요.", 3f); return null; }
    }
}