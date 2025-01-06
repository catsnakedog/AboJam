using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /* Dependency */
    public HP hp;

    /* Field & Property */
    public static List<Tower> instances = new List<Tower>(); // 모든 타워 인스턴스
    public static Tower currentTower; // 최근 선택된 타워
    public int Level { get; private set; } = 0; // 현재 레벨
    public int MaxLevel { get; private set; } // 최대 레벨
    public int[] ReinforceCost { get { return reinforceCost; } private set { reinforceCost = value; } } // 레벨업 비용 (개수 = 최대 레벨 결정)

    /* Backing Field */
    [SerializeField] private int[] reinforceCost; 

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        MaxLevel = reinforceCost.Length - 1;
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 타워 클릭 시 상호작용 입니다.
    /// </summary>
    public void OnClick()
    {
        currentTower = this;
        Reinforcement.instance.On();
    }
    /// <summary>
    /// 타워를 증강합니다.
    /// </summary>
    public virtual void Reinforce()
    {
        // 초기화
        Debug.Log($"{name} 증강");
        hp.Heal(hp.HP_max);
        Level++;
    }
}
