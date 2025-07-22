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
        { TowerType.Guard, "든든한\n바리케이드" },
        { TowerType.Auto, "가까운 적을\n한 명씩\n공격합니다." },
        { TowerType.Splash, "뭉친 적들을\n날려버릴\n폭탄을\n발사합니다." },
        { TowerType.Production, "아보카도를 두 개씩\n수확할 수\n있습니다 !" },
        { TowerType.Heal, "주변 아군을\n치유합니다." },
    };
    /// <summary>
    /// <br>Reinforcement 에 표기되는 설명 입니다.</br>
    /// </summary>
    public static Dictionary<TowerType, string> text_reinforcement = new Dictionary<TowerType, string>(0)
    {
        { TowerType.Guard, "조금 더\n튼튼해집니다." },
        { TowerType.Auto, "한 번에 더\n많은 포탄을\n발사합니다." },
        { TowerType.Splash, "더 강력한\n폭탄을\n발사합니다." },
        { TowerType.Production, "생산 능력 강화" },
        { TowerType.Heal, "치유량이\n증가합니다." },
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
        Garu = 65;
        Abocado = 2;
        Water = 10;
        gameData = new GameData();
    }
}