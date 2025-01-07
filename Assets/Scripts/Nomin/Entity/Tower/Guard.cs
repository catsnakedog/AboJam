using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Tower
{
    /* Field & Property */
    public static List<Guard> instances = new List<Guard>(); // 모든 방어 타워 인스턴스

    /* Public Method */
    /// <summary>
    /// <br>타워를 증강합니다.</br>
    /// <br>체력이 많아집니다.</br>
    /// </summary>
    public override void Reinforce()
    {
        base.Reinforce();
        hp.SetMaxHP(hp.HP_max * 1.5f);
        hp.Heal(hp.HP_max);
    }

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        base.Start();
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
