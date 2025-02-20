using System.Data.SqlClient;
using System.Data;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// MSSQL 클라이언트
/// </summary>
public class DBMS : MonoBehaviour
{
    /* Field & Property */
    public static DBMS instance;

    [Header("[ 클라이언트 로그인 정보 입력 ]")]
    [SerializeField] private string ip; public string IP { get => ip; private set => ip = value; }
    [SerializeField] private string port; public string PORT { get => port; private set => port = value; }
    [SerializeField] private string database; public string DB { get => database; private set => database = value; }
    [SerializeField] private string id; public string ID { get => id; private set => id = value; }
    [SerializeField] private string password; public string PASSWORD { get => password; private set => password = value; }
    public SqlConnection Connection { get; set; }
    public object Lock { get; } = new(); // 멀티 스레딩 DB 접근 동기화

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // UTF-8 강제 적용
        try { Connection = new($"Data Source={IP},{PORT};Initial Catalog={DB};User ID={ID};Password={PASSWORD};TrustServerCertificate=True;Connection Timeout=5;"); }
        catch { Debug.Log("DB 연결 실패"); }
    }

    /* Public Method */
    /// <summary>
    /// 서버에서 DataSet 을 가져옵니다.
    /// </summary>
    public DataSet GetDataSet()
    {
        lock (Lock)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();

            DataSet dataSet = new();

            // 모든 테이블 이름 가져오기
            List<string> tableNames = new();
            using (SqlCommand command = new("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", Connection))
            using (SqlDataReader reader = command.ExecuteReader()) while (reader.Read()) tableNames.Add(reader.GetString(0));

            // 각 테이블 마다 >> 테이블 데이터를 조회하여 >> dataSet 에 추가
            foreach (string tableName in tableNames)
                using (SqlDataAdapter adapter = new($"SELECT * FROM [{tableName}]", Connection))
                    adapter.Fill(dataSet, tableName);

            Connection.Close();

            return dataSet;
        }
    }
    /// <summary>
    /// 서버 연결을 검증합니다.
    /// </summary>
    /// <returns></returns>
    public bool CheckConnection()
    {
        try
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
            Console.WriteLine("✅ MSSQL 서버 연결 성공!");
            Connection.Close();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 연결 실패: {ex.Message}");
            return false;
        }
    }
}