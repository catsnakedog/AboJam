using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ITower
{
    public static List<ITower> instances = new();
    public static ITower currentTower;
    public int Level { get; set; } // 현재 레벨
    public int MaxLevel { get; set; } // 최대 레벨
    public int[] ReinforceCost { get; set; } // 레벨업 비용 (개수 = 최대 레벨 결정)
    public static Action eventOnClick;

    public void Reinforce();

    public IEnumerator CorDeath(float time);
}
