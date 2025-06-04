using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumData
{
    public enum Weapon
    {
        Gun,
        ShotGun,
        Riple,
        Sniper,
        Bat,
        Knife,
        Spear,
        ChainSaw,
        None,
    }

    /// <summary>
    /// <br>아보카도 성장 단계 입니다.</br>
    /// <br>Resources/Images/Abocado 에 타입에 맞는 이미지가 필요합니다.</br>
    /// </summary>
    public enum Abocado : sbyte
    {
        Cultivated,
        Seed,
        Tree,
        Fruited,
    }

    /// <summary>
    /// <br>아보카도 타워 종류 입니다.</br> 
    /// <br>StaticData.text_promotion 에 타입에 맞는 설명이 필요합니다.</br>
    /// <br>Resources/Prefabs/Tower 에 타입에 맞는 프리팹이 필요합니다.</br>
    /// <br>Resources/UI/Promotion/Tower 에 타입에 맞는 이미지가 필요합니다.</br>
    /// </summary>
    public enum TowerType : sbyte
    {
        Guard,
        Auto,
        Splash,
        Production,
        Heal,
    }

    /// <summary>
    /// 구분용 인덱스
    /// </summary>
    public enum TileIndex
    {
        Empty = 0,
        AboCado = 1,
        Guard = 2,
        Auto = 3,
        Splash = 4,
        Heal = 5,
    }

    /// <summary>
    /// 특수 능력이 적용될 몬스터의 특수 레벨
    /// </summary>
    public enum SpecialLevel
    {
        FirstBoss = 999,
        SecondsBoss = 9999,
        ThirdBoss = 99999,
    }
}
