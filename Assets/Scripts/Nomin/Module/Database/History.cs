using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;

public class History : MonoBehaviour
{
    /* Dependency */
    public GameObject gameDataLocal;
    public GameObject record;
    private LocalData localData => LocalData.instance;
    private DBMS dbms => DBMS.instance;
    private Database_AboJam database_AboJam => Database_AboJam.instance;

    /* Field & Property */
    public static History instance;
    private List<GameData> gameDatas = new();
    private string ID; // 클라이언트 고유 ID

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        gameDatas = localData.LoadDatas();
        if (gameDatas == null) gameDatas = new List<GameData>();
        SetLocalHistory();
        SetServerHistory();
    } // 매 Main 로드 시 실행

    /* Private Method */
    /// <summary>
    /// 게임 데이터를 로컬에 저장하고, 화면에 띄웁니다.
    /// </summary>
    private void SetLocalHistory()
    {
        if (StaticData.gameData != null) gameDatas.Add(StaticData.gameData); // 이전 게임 데이터 추가
        gameDatas.Sort((a, b) => b.dateTime.CompareTo(a.dateTime)); // 기록 내림차순 정렬
        if (gameDatas.Count > 3) gameDatas.RemoveRange(3, gameDatas.Count - 3); // 3 개만 남기고 제거
        localData.SaveDatas(gameDatas); // 로컬에 게임 데이터 저장

        // 각 게임 데이터를 시각화
        for (int i = 0; i < gameDatas.Count; i++) { GameObject instance = Instantiate(record, gameDataLocal.transform); }
        for (int i = 0; i < gameDatas.Count; i++)
        {
            DateTime dateTime = gameDatas[i].GetDateTime();
            gameDataLocal.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"[{i + 1} 등] {dateTime:y년 d일 hh:mm} 까지 생존";
        }
    }
    /// <summary>
    /// 게임 데이터를 서버에 저장하고, 화면에 띄웁니다.
    /// </summary>
    private void SetServerHistory()
    {
        // 연결 검증
        if (dbms.CheckConnection() == false) return;
        DataSet dataSet = dbms.GetDataSet();

        // 고유 ID 검사
        CheckID(dataSet);

        // 로컬 최고기록을 서버 고유 ID 기록에 생성 | 덮어쓰기
        if (gameDatas.Count > 0)
        {
            gameDatas[0].ID = ID;
            InsertHistory(gameDatas[0], dataSet);
        }

        // 데이타셋 새로고침
        dataSet = dbms.GetDataSet();

        // 서버 History 테이블 정렬해서 가져오기
        List<GameData> histories = GetHistory(dataSet);

        // 내 기록 최상단에 띄우기
        GameData myData = histories.Find(game => game.ID == ID);

        // 서버 기록 Top 100 까지로 줄이기
        if (histories.Count > 100) histories.RemoveRange(100, histories.Count - 100);

        // 출력

        /* Local Method */
        void InsertHistory(GameData gameData, DataSet dataSet)
        {
            // 만약 기존 기록이 더 높으면 리턴
            DataTable historyTable = dataSet.Tables["History"];
            DataRow[] historyRow = historyTable.Select($"ID = '{ID}'");
            if (historyRow.Length > 0)
            {
                DateTime localDateTime, serverDateTime;
                bool isServerValid = DateTime.TryParse(historyRow[0]["dateTime"].ToString(), out serverDateTime);
                bool isLocalValid = DateTime.TryParse(gameData.dateTime, out localDateTime);
                if (isServerValid && isLocalValid && serverDateTime >= localDateTime) return;
            }

            lock (dbms.Lock)
            {
                dbms.Connection.Open();

                string query = @"
                IF EXISTS (SELECT 1 FROM History WHERE ID = @id)
                    UPDATE History
                    SET dateTime = @dateTime, 
                        [kill] = @kill, 
                        abocado = @abocado, 
                        tower = @tower, 
                        garu = @garu
                    WHERE ID = @id;
                ELSE
                    INSERT INTO History
                    VALUES (@id, @dateTime, @kill, @abocado, @tower, @garu);";

                using (SqlCommand cmd = new SqlCommand(query, dbms.Connection))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@dateTime", gameData.dateTime);
                    cmd.Parameters.AddWithValue("@kill", gameData.kill);
                    cmd.Parameters.AddWithValue("@abocado", gameData.abocado);
                    cmd.Parameters.AddWithValue("@tower", gameData.tower);
                    cmd.Parameters.AddWithValue("@garu", gameData.garu);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    Console.WriteLine(rowsAffected > 0 ? "History 추가 성공!" : "History 추가 실패!");
                }

                dbms.Connection.Close();
            }
        }
        List<GameData> GetHistory(DataSet dataSet)
        {
            List<GameData> histories = new List<GameData>();
            DataTable historyTable = dataSet.Tables["History"];

            foreach (DataRow row in historyTable.Rows)
            {
                GameData gameData = new GameData
                {
                    ID = row["ID"].ToString(),
                    dateTime = row["dateTime"].ToString(),
                    kill = Convert.ToInt32(row["kill"]),
                    abocado = Convert.ToInt32(row["abocado"]),
                    tower = Convert.ToInt32(row["tower"]),
                    garu = Convert.ToInt32(row["garu"])
                };

                histories.Add(gameData);
            }

            // 날짜순 정렬 (최신 기록이 먼저 오도록)
            histories.Sort((a, b) => b.GetDateTime().CompareTo(a.GetDateTime()));

            return histories;
        }

    }
    /// <summary>
    /// <br>현재 클라이언트 고유 ID 를 업데이트 하고 반환합니다.</br>
    /// <br>서버 연결 검증이 우선되어야 합니다.</br>
    /// </summary>
    private void CheckID(DataSet dataSet)
    {
        // 아이디 불러오기
        ID = localData.LoadID();

        // 없으면 생성
        if (ID == null) { CreateID(dataSet); localData.SaveID(ID); InsertID(ID); }

        /* Local Method */
        string CreateID(DataSet dataSet)
        {
            // DB 에서 무작위 동물 이름 가져오기
            DataTable animalTable = dataSet.Tables["Animal"];
            string animalName = animalTable.AsEnumerable()
                                            .OrderBy(row => Guid.NewGuid()) // 무작위 정렬
                                            .Select(row => row.Field<string>("name"))
                                            .FirstOrDefault();

            // DB 에서 ID 테이블의 다음 인덱스 구하기
            int index = dataSet.Tables.Contains("ID") ? dataSet.Tables["ID"].AsEnumerable().Count() : 0;

            // 클라이언트 ID = 동물 이름 + 인덱스
            return animalName + (index + 1).ToString();
        }
        void InsertID(string ID)
        {
            lock (dbms.Lock)
            {
                dbms.Connection.Open();

                string query = "INSERT INTO ID VALUES (@id)";

                using (SqlCommand cmd = new SqlCommand(query, dbms.Connection))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    Console.WriteLine(rowsAffected > 0 ? "ID 추가 성공!" : "ID 추가 실패!");
                }

                dbms.Connection.Close();
            }
        }
    }
}

/* JSON Serializable Data */
// CONSTRAINT : Class
// CONSTRAINT : Serializable 어트리뷰트
[Serializable] public class IDData { public string ID; } // 아이디 래퍼 클래스
[Serializable] public class GameData
{
    /* Field & Property */
    public string ID;
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
} // 데이터 클래스
[Serializable] public class GameDataList { public List<GameData> gameDataList = new(); } // 데이터 래퍼 클래스