using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class History : MonoBehaviour
{
    /* Dependency */
    public GameObject gameDataLocal;
    public GameObject record;
    private LocalData localData => LocalData.instance;

    /* Field & Property */
    public static History instance;
    public List<GameData> gameDatas = new();

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        gameDatas = localData.Load();
        SetLocalHistory();
    } // 매 Main 로드 시 실행

    /* Private Method */
    /// <summary>
    /// 게임 데이터를 로컬에 저장하고, 화면에 띄웁니다.
    /// </summary>
    private void SetLocalHistory()
    {
        if(StaticData.gameData.GetDateTime() != DateTime.MinValue) gameDatas.Add(StaticData.gameData); // 이전 게임 데이터 추가
        gameDatas.Sort((a, b) => b.dateTime.CompareTo(a.dateTime)); // 기록 내림차순 정렬
        if (gameDatas.Count > 3) gameDatas.RemoveRange(3, gameDatas.Count - 3); // 3 개만 남기고 제거
        localData.Save(gameDatas); // 로컬에 게임 데이터 저장

        // 각 게임 데이터를 시각화
        for (int i = 0; i < gameDatas.Count; i++) { GameObject instance = Instantiate(record, gameDataLocal.transform); }
        for (int i = 0; i < gameDatas.Count; i++)
        {
            DateTime dateTime = gameDatas[i].GetDateTime();
            gameDataLocal.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{i + 1} 등 : {dateTime:y 년 d 일 m 분} 까지 생존";
        }
    }
}


[Serializable] public class GameData // 반드시 클래스여야만 JSON 직렬화 가능
{
    /* Field & Property */
    public string dateTime; // 게임 종료 시점에 저장 (반드시 string 이어야 직렬화 가능)
    public int kill; // 킬 수
    public int abocado; // 심은 아보카도 수
    public int tower; // 건설한 타워 수
    public int garu; // 사용한 가루 수

    /* Initializer & Finalizer & Updater */
    public GameData() { }
    public GameData(DateTime date, int kill, int abocado, int tower, int garu)
    {
        this.dateTime = date.ToString("o"); // ISO 8601 형식
        this.kill = kill;
        this.abocado = abocado;
        this.tower = tower;
        this.garu = garu;
    }

    /* Public Method */
    /// <summary>
    /// dateTime 을 DateTime 형식으로 변환하여 반환합니다.
    /// </summary>
    /// <returns></returns>
    public DateTime GetDateTime()
    {
        return DateTime.TryParse(dateTime, out DateTime result) ? result : DateTime.MinValue;
    }
}
[Serializable] public class GameDataList { public List<GameData> gameDataList = new(); } // 데이터 래퍼 클래스 (JSON 직렬화 목적, Serializable 해야 함)