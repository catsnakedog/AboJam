using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /* Dependency */
    public HP hp;
    public Movement movement;

    /* Field & Property */
    public static List<Enemy> instances = new List<Enemy>(); // 모든 적 인스턴스
    public static Enemy currentEnemy; // 최근 선택된 적
    public int Level { get; private set; } = 0; // 현재 레벨
    public int MaxLevel { get; private set; } // 최대 레벨

    /* Intializer & Finalizer & Updater */
    public void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 적 클릭 시 상호작용 입니다.
    /// </summary>
    public void OnClick()
    {
        currentEnemy = this;
    }
    /// <summary>
    /// <br>적을 증강합니다.</br>
    /// <br>공통 증강만 정의되어 있으며, 개별 증강은 자식 스크립트에서 구현됩니다.</br>
    /// </summary>
    public virtual void Reinforce()
    {
        // 적 공통 증강
        Debug.Log($"{name} 증강");
        hp.Heal(hp.HP_max);
        Level++;
    }
}