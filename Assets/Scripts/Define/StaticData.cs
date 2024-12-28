using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumData;

public static class StaticData
{
    // (i, j) GameObject 접근 : Grid.instance.GetObject(i, j);
    // (i, j) Tile 접근 : Grid.instance.GetObject(i, j).GetComponant<Tile>();
    public static int Garu;
    public static int Abocado = 5;
    public static int Water = 10;
    public static int Date_Day = 0;
    public static int Date_Time = 0;
    public static int Date_TimeThreshold = 5;
    public static bool Date_isMorning = true;

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
}