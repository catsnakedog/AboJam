using System.Data.SqlClient;
using System.Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Rendering;

/// <summary>
/// MSSQL 클라이언트
/// </summary>
public class Database : MonoBehaviour
{
    /* Field & Property */
    public static Database instance;

    [Header("[ 클라이언트 로그인 정보 입력 ]")]
    [SerializeField] private string ip; public string IP { get => ip; private set => ip = value; }
    [SerializeField] private string port; public string PORT { get => port; private set => port = value; }
    [SerializeField] private string database; public string DB { get => database; private set => database = value; }
    [SerializeField] private string id; public string ID { get => id; private set => id = value; }
    [SerializeField] private string password; public string PASSWORD { get => password; private set => password = value; }
    public SqlConnection Connection { get; private set; }

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        try { Connection = new SqlConnection($"Data Source={IP},{PORT};Initial Catalog={DB};User ID={ID};Password={PASSWORD}"); Debug.Log("DB 연결 성공"); }
        catch { Debug.Log("DB 연결 실패"); }
    }

    /* Public Method */
    /// <summary>
    /// DB 로 SO 를 초기화 합니다.
    /// </summary>
    public void Load()
    {
        Connection.Open();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = Connection;
        cmd.CommandText = "SELECT * FROM t1";

        SqlDataAdapter sd = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        sd.Fill(ds, "test");

        Connection.Close();
    }

    /// <summary>
    /// SO 로 인스턴스를 초기화 합니다.
    /// </summary>
    public void Refresh()
    {

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
}