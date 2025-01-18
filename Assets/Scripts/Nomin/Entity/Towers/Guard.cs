using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Tower, IScriptableObject<SO_Guard>, IPoolee
{
    /* Field & Property */
    public static List<Guard> instances = new List<Guard>(); // 모든 방어 타워 인스턴스
    [SerializeField] private SO_Guard so; public SO_Guard SO { get => so; set => so = value; }

    /* Public Method */
    /// <summary>
    /// <br>타워를 증강합니다.</br>
    /// <br>체력이 많아집니다.</br>
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
        hp.SetMaxHP(hp.Hp_max * 1.5f);
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
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save()
    {
        base.Save();
    } // 풀에 집어 넣을 때 자동 실행
}
