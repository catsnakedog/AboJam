using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEditor.Rendering;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    /* Dependency */
    public GameObject gameDataLocal;
    public GameObject gameDataServer;
    public GameObject gameDataMy;
    public GameObject record;
    private LocalData localData => LocalData.instance;
    private DBMS dbms => DBMS.instance;
    private Message message => Message.instance;
    [SerializeField] private GameObject loading;
    private Coroutine corLocalLast;
    private Coroutine corServerLast;
    private Coroutine corWaitLast;
    private float waitTime = 10;
    private DateTime last = DateTime.MinValue;

    /* Field & Property */
    public static History instance;
    private string ID; // 클라이언트 고유 ID
    private List<GameData> gameDatas = new();

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        gameDatas = localData.LoadDatas();
        if (gameDatas == null) gameDatas = new List<GameData>();
        SetLocalHistory();
    } // 매 Main 로드 시 실행

    /* Public Method */
    /// <summary>
    /// 게임 데이터를 로컬에 저장하고, 화면에 띄웁니다.
    /// </summary>
    public void SetLocalHistory()
    {
        if (corLocalLast == null) corLocalLast = StartCoroutine(CorSetLocalHistory());
    }
    private IEnumerator CorSetLocalHistory()
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
            gameDataLocal.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text =
                $"[{i + 1}] 나이 : {dateTime:y살 d일}";
            gameDataLocal.transform.GetChild(i).GetComponentInChildren<Record>().WriteLog(gameDatas[i]);
        }

        ID = localData.LoadID();
        if (ID != null) message.On($"환영합니다, {ID} 님", 3f);

        corLocalLast = null;
        yield return null;
    }
    /// <summary>
    /// 게임 데이터를 서버에 저장하고, 화면에 띄웁니다.
    /// </summary>
    public void SetServerHistory()
    {
        if (corServerLast == null) corServerLast = StartCoroutine(CorSetServerHistory());
    }
    private IEnumerator CorSetServerHistory()
    {
        // 연결 검증 (비동기)
        message.On("서버 연결 중 입니다. 잠시만 기다려주세요.", 2f);
        GameObject loading = Instantiate(this.loading, new Vector3(4.78f, 2.69f, 1f), Quaternion.identity);
        Task<bool> checkTask = Task.Run(() => dbms.CheckConnection());
        yield return new WaitUntil(() => checkTask.IsCompleted); // 완료될 때까지 기다리기
        if (checkTask.Result == false)
        {
            if (corWaitLast == null) corWaitLast = StartCoroutine(CorWait());
            message.On($"서버 연결에 실패했습니다. {(int)Math.Round(waitTime, MidpointRounding.AwayFromZero)} 초 후에 다시 시도해주세요.\n학교 등 공공기관 와이파이는 연결이 실패할 수 있습니다.", 4f);
            Destroy(loading);
            corServerLast = null;
            yield break;
        }

        // DataSet 가져오기 (비동기)
        Task<DataSet> dataSetTask = Task.Run(() => dbms.GetDataSet());
        yield return new WaitUntil(() => dataSetTask.IsCompleted); // 완료될 때까지 기다리기
        DataSet dataSet = dataSetTask.Result; // 비동기 작업이 끝난 후 결과 가져오기

        // 고유 ID 검사
        CheckIDAsync(dataSet);

        // 로컬 최고기록을 서버에 저장
        if (gameDatas.Count > 0)
        {
            gameDatas[0].ID = ID;
            Task insertTask = InsertHistoryAsync(gameDatas[0], dataSet);
            yield return new WaitUntil(() => insertTask.IsCompleted);
        }

        // DataSet 새로고침 (비동기)
        dataSetTask = Task.Run(() => dbms.GetDataSet());
        yield return new WaitUntil(() => dataSetTask.IsCompleted);
        dataSet = dataSetTask.Result;

        // 서버 History 테이블 가져오기
        List<GameData> histories = GetHistory(dataSet);

        // 클라이언트 뷰 초기화
        while (gameDataServer.transform.childCount > 0) DestroyImmediate(gameDataServer.transform.GetChild(0).gameObject);
        while (gameDataMy.transform.childCount > 0) DestroyImmediate(gameDataMy.transform.GetChild(0).gameObject);

        // 내 기록 시각화
        GameData myData = histories.Find(game => game.ID == ID);
        if (myData != null)
        {
            GameObject obj = Instantiate(record, gameDataMy.transform);
            foreach (Image img in obj.GetComponentsInChildren<Image>()) if (img.color.a != 0) img.color = UnityEngine.Color.red;
            DateTime myDateTime = myData.GetDateTime();
            string myID = AdjustWidth($"[{histories.FindIndex(game => game.ID == ID) + 1} 등] " + myData.ID, 15);
            string myDate = AdjustWidth($"나이 : {myDateTime: y살 d일}", 20);
            gameDataMy.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"{myID} {myDate}";
            gameDataMy.transform.GetChild(0).GetComponentInChildren<Record>().WriteLog(myData);
        }

        // 서버 기록 Top 100 유지
        if (histories.Count > 100) histories.RemoveRange(100, histories.Count - 100);

        // 서버 데이터를 최대 100개까지 시각화
        int index = Math.Min(histories.Count, 100);
        for (int i = 0; i < index; i++) Instantiate(record, gameDataServer.transform);
        for (int i = 0; i < index; i++)
        {
            DateTime dateTime = histories[i].GetDateTime();
            string id = AdjustWidth($"[{i + 1} 등] " + histories[i].ID, 15);
            string date = AdjustWidth($"나이 : {dateTime: y살 d일}", 20);
            gameDataServer.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = $"{id} {date}";
            gameDataServer.transform.GetChild(i).GetComponentInChildren<Record>().WriteLog(histories[i]);
        }

        if (loading != null) Destroy(loading);
        corServerLast = null;

        /* Local Method */
        async Task InsertHistoryAsync(GameData gameData, DataSet dataSet)
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
            await Task.Run(() =>
            {
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
            });
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
        string AdjustWidth(string input, int totalWidth)
        {
            int currentWidth = 0;
            foreach (char c in input)
            {
                currentWidth += Regex.IsMatch(c.ToString(), @"[\uAC00-\uD7A3]") ? 2 : 1; // 한글이면 2칸, 아니면 1칸
            }

            int spaceToAdd = totalWidth - currentWidth; // 필요한 공백 개수 계산
            return spaceToAdd > 0 ? input + new string(' ', spaceToAdd) : input; // 부족한 만큼 공백 추가
        }
    }

    /* Private Method */
    /// <summary>
    /// <br>현재 클라이언트 고유 ID 를 업데이트 하고 반환합니다.</br>
    /// <br>서버 연결 검증이 우선되어야 합니다.</br>
    /// </summary>
    private async void CheckIDAsync(DataSet dataSet)
    {
        // 아이디 불러오기
        ID = localData.LoadID();

        // 없으면 생성
        if (ID == null)
        {
            localData.SaveID(CreateID(dataSet));
            ID = localData.LoadID();
            await InsertIDAsync(ID);
        }

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
        async Task InsertIDAsync(string ID)
        {
            string query = "INSERT INTO ID VALUES (@id)";

            lock (dbms.Lock)
            {
                dbms.Connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, dbms.Connection))
                {
                    cmd.Parameters.AddWithValue("@id", ID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                }

                dbms.Connection.Close();
            }
        }
    }
    /// <summary>
    /// 대기 시간을 감소시킵니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorWait()
    {
        while (waitTime > 0)
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - last; // 1)
            if (elapsed.TotalSeconds > 0.1f * 5) { elapsed = TimeSpan.FromSeconds(0.1f); }
            last = now;
            waitTime -= (float)elapsed.TotalSeconds;
            if (waitTime < 0) waitTime = 0;

            yield return new WaitForSeconds(0.1f);
        }

        waitTime = 10;
        corWaitLast = null;
    }
}

/* JSON Serializable Data */
// CONSTRAINT : Class
// CONSTRAINT : Serializable 어트리뷰트
[Serializable] public class IDData { public string ID; } // 아이디 래퍼 클래스
[Serializable] public class ScenarioData { public int scenarioNumber; } // 시나리오 래퍼 클래스
[Serializable]
public class GameData
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
}// 데이터 클래스
[Serializable] public class GameDataList { public List<GameData> gameDataList = new(); } // 데이터 래퍼 클래스