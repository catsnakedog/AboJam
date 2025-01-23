using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Guard : Tower, IPoolee
{
    /* Field & Property */
    public static List<Guard> instances = new List<Guard>(); // 모든 방어 타워 인스턴스
    public float hpMultiply = 1.5f;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    [SerializeField] private string ID; // Primary Key

    /* Public Method */
    /// <summary>
    /// <br>타워를 증강합니다.</br>
    /// <br>체력이 많아집니다.</br>
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
        hp.SetMaxHP(hp.Hp_max * hpMultiply);
        hp.Heal(hp.Hp_max);
    }

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        base.Start();
        instances.Add(this);
        Load();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        base.Load();

        database_abojam.ExportGuard(ID, ref hpMultiply);
    } // 풀에서 꺼낼 때 / Import 시 자동 실행
    public void Save()
    {
        base.Save();
    } // 풀에 집어 넣을 때 자동 실행
}