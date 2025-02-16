using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using static EnumData;

public static class StaticData
{
    /* Field & Property */
    /// <summary>
    /// <br>Promotion 에 표기되는 TowerType 의 설명 입니다.</br>
    /// </summary>
    public static Dictionary<TowerType, string> text_promotion = new Dictionary<TowerType, string>(0)
    {
        { TowerType.Guard, "체력이 많은 아보카도, 생산능력 x 공격능력 x" },
        { TowerType.Auto, "적에게 준수한 연사속도로 한발씩 공격" },
        { TowerType.Splash, "적에게 느린 연사속도로 광역공격" },
        { TowerType.Production, "생산능력이 강화" },
        { TowerType.Heal, "체력이 가장 낮은 아보카도 나무의 체력을 회복" },
    };
    /// <summary>
    /// <br>Reinforcement 에 표기되는 설명 입니다.</br>
    /// </summary>
    public static Dictionary<TowerType, string> text_reinforcement = new Dictionary<TowerType, string>(0)
    {
        { TowerType.Guard, "더 많은 체력" },
        { TowerType.Auto, "세 발씩 공격" },
        { TowerType.Splash, "넓은 범위 광역공격" },
        { TowerType.Production, "생산 능력 강화" },
        { TowerType.Heal, "체력 회복량 증가" },
    };
    public static GameData gameData = new ();
    private static int garu; public static int Garu
    {
        get
        {
            return garu;
        }
        set
        {
            if (garu > value) StaticData.gameData.garu += garu - value; // 사용량 만큼 게임 데이터에 추가
            garu = value;
            foreach (var item in Inventory.instances) if (item != null) item.UpdateGaru();
        }
    }
    private static int abocado; public static int Abocado
    {
        get
        {
            return abocado;
        }
        set
        {
            abocado = value;
            foreach (var item in Inventory.instances) if (item != null) item.UpdateAbocado();
        }
    }
    private static int water; public static int Water
    {
        get
        {
            return water;
        }
        set
        {
            water = value;
            foreach (var item in Inventory.instances) if (item != null) item.UpdateWater();
        }
    }

    /* Intializer & Finalizer & Updater */
    static StaticData()
    {
        Abocado = 5;
    }
    public static void Init()
    {
        Garu = 0;
        Abocado = 1;
        Water = 10;
        gameData = new GameData();
    }
}