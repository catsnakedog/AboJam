using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    /* Dependency */
    public HP hp;

    /* Field & Property */
    public static List<Tower> instances = new List<Tower>(); // 모든 타워 인스턴스
    public static Tower currentTower; // 최근 선택된 타워

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
        Debug.Log($"{name} 증강");
        hp.Heal(hp.HP_max);
    }

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
