using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class History : MonoBehaviour, IPointerEnterHandler
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
    [SerializeField] private Sprite myLog;
    [SerializeField] private GameObject trophy;
    private Coroutine corLocalLast;
    private Coroutine corServerLast;
    private Coroutine corWaitLast;
    private DateTime last = DateTime.MinValue;

    /* Field & Property */
    public static History instance;
    public float trophyOffset;
    private string ID; // 클라이언트 고유 ID
    private List<GameData> gameDatas = new();
    private float[] retryDelay = { 10, 15, 25, 45 }; // 연결 재시도 대기 시간
    private int retryIndex = 0; // 연결 재시도 횟수
    private float waitTime; // 현재 남은 대기 시간
    private bool isWait = false;
    public static Action eventHover;
    public static Action eventNetwork;

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
        gameDatas.RemoveAll(gameData => string.IsNullOrEmpty(gameData.dateTime));// 덤프 값 제거
        gameDatas.Sort((a, b) => b.dateTime.CompareTo(a.dateTime)); // 기록 내림차순 정렬
        if (gameDatas.Count > 3) gameDatas.RemoveRange(3, gameDatas.Count - 3); // 3 개만 남기고 제거
        localData.SaveDatas(gameDatas); // 로컬에 게임 데이터 저장

        // 각 게임 데이터를 시각화
        for (int i = 0; i < gameDatas.Count; i++) { GameObject instance = Instantiate(record, gameDataLocal.transform); }
        for (int i = 0; i < gameDatas.Count; i++)
        {
            DateTime dateTime = gameDatas[i].GetDateTime();
            gameDataLocal.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text =
                $"[{i + 1}] 나이 : {dateTime:d일}";
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
        eventNetwork.Invoke();
        if (corServerLast == null) corServerLast = StartCoroutine(CorSetServerHistory());
    }
    private IEnumerator CorSetServerHistory()
    {
        // 연결 실패 후 대기 상태가 아니면
        GameObject loading;
        if(!isWait)
        {
            // 연결 검증 (비동기)
            message.On("서버 연결 중 입니다. 잠시만 기다려주세요.", 2f);
            loading = Instantiate(this.loading, new Vector3(4.78f, 2.69f, 1f), Quaternion.identity);
            Task<bool> checkTask = Task.Run(() => dbms.CheckConnection());
            yield return new WaitUntil(() => checkTask.IsCompleted); // 완료될 때까지 기다리기

            // 연결 실패 시 대기 상태 시작
            if (checkTask.Result == false)
            {
                // 대기 시간 조정
                waitTime = retryDelay[retryIndex];
                retryIndex++; if (retryIndex >= retryDelay.Length) waitTime = 120;

                // 대기 시작
                if (corWaitLast == null) corWaitLast = StartCoroutine(CorWait());
                message.On($"서버 연결에 실패했습니다. {(int)Math.Round(waitTime, MidpointRounding.AwayFromZero)} 초 후에 다시 시도해주세요.\n공용 인터넷일 경우 LTE / 데이터로 연결해주세요.", 4f);
                Destroy(loading);
                corServerLast = null;
                yield break;
            }
        }
        // 대기 상태 시 안내 메시지 출력
        else
        {
            message.On($"서버 연결에 실패했습니다. {(int)Math.Round(waitTime, MidpointRounding.AwayFromZero)} 초 후에 다시 시도해주세요.\n공용 인터넷일 경우 LTE / 데이터로 연결해주세요.", 4f);
            corServerLast = null;
            yield break;
        }

        // 연결 성공 시 대기 시간 초기화
        retryIndex = 0;

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
            foreach (Image img in obj.GetComponentsInChildren<Image>()) if (img.color.a != 0) img.sprite = myLog;
            DateTime myDateTime = myData.GetDateTime();
            string myID = AdjustWidth($"[{histories.FindIndex(game => game.ID == ID) + 1} 등] " + myData.ID, 15);
            gameDataMy.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"{myID}({myDateTime:d일})";
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
            gameDataServer.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = $"{id}({dateTime:d일})";
            gameDataServer.transform.GetChild(i).GetComponentInChildren<Record>().WriteLog(histories[i]);

            // 트로피를 인스턴스화 후 위치시킵니다.
            if(i == 0)
            {
                RectTransform parentRectTransform = gameDataServer.transform.GetChild(i).GetComponent<RectTransform>();
                GameObject instance = Instantiate(trophy, parentRectTransform);
                RectTransform childRectTransform = instance.GetComponent<RectTransform>();
                Vector2 parentSize = parentRectTransform.rect.size;
                Vector2 localPos = new Vector2(parentSize.x * 0.5f, 0);
                localPos += new Vector2(trophyOffset, 0);
                childRectTransform.anchoredPosition = localPos;
                childRectTransform.localScale = Vector3.one;
            }
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

                        try
                        {
                            int rowsAffected = cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            Debug.Log("ID : " + ID);
                            Debug.Log("SQL 오류 발생:");
                            Debug.Log(ex.Message);               // 오류 메시지
                            Debug.Log($"오류 코드: {ex.Number}"); // SQL 오류 코드
                            Debug.Log($"라인 번호: {ex.LineNumber}"); // 오류 발생 위치
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("기타 오류 발생:");
                            Debug.Log(ex.Message);
                        }
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
    /// <summary>
    /// 마우스 호버링 이벤트입니다.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventHover.Invoke();
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

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Debug.Log("ID : " + ID);
                        Debug.Log("SQL 오류 발생:");
                        Debug.Log(ex.Message);               // 오류 메시지
                        Debug.Log($"오류 코드: {ex.Number}"); // SQL 오류 코드
                        Debug.Log($"라인 번호: {ex.LineNumber}"); // 오류 발생 위치
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("기타 오류 발생:");
                        Debug.Log(ex.Message);
                    }
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
        isWait = true;
        float startTime = Time.time;
        float endTime = startTime + waitTime;

        while (Time.time < endTime)
        {
            waitTime = endTime - Time.time;
            yield return null;
        }

        corWaitLast = null;
        isWait = false;
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