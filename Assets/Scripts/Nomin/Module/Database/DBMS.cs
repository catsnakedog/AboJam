using System.Data.SqlClient;
using System.Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Rendering;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using System.IO;

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
    private SqlConnection Connection { get; set; }
    private object Lock { get; } = new(); // 멀티 스레딩 DB 접근 동기화

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        try { Connection = new ($"Data Source={IP},{PORT};Initial Catalog={DB};User ID={ID};Password={PASSWORD}"); }
        catch { Debug.Log("DB 연결 실패"); }
    }

    /* Public Method */
    /// <summary>
    /// DB 에서 데이터를 가져옵니다.
    /// </summary>
    public DataTable Get(string SQL)
    {
        lock (Lock)
        {
            Connection.Open();

            DataTable dataTable = new();
            using (SqlDataAdapter adapter = new(new(SQL, Connection))) adapter.Fill(dataTable);

            Connection.Close();

            return dataTable;
        }
    }
    /// <summary>
    /// DB 에 데이터를 작성합니다.
    /// </summary>
    public void Set(string SQL)
    {
        lock (Lock)
        {
            Connection.Open();

            using (SqlCommand command = new(SQL, Connection)) command.ExecuteNonQuery();

            Connection.Close();
        }
    }
    /// <summary>
    /// 서버에서 DataSet 을 가져옵니다.
    /// </summary>
    public DataSet GetDataSet()
    {
        lock (Lock)
        {
            Connection.Open();

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
}


    /*
    [ 게임 시작 파이프라인 ]
    Database.Load() // DBMS >> SO
    인스턴스 Awake 시 Init() // SO >> 인스턴스화

    [ 런타임 쿼리 파이프라인 ]
    SSMS // QUERY >> DBMS
    Database.Load() // DBMS >> SO
    인스턴스.Init() // SO >> 기존 인스턴스
    */