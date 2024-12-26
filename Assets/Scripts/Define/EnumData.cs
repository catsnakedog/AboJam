using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumData
{
    public enum TileType
    {
        Empty,
        BreakWall,
        Wall,
        River,
        Turret,
    }

    public enum Weapon
    {
        Gun,
        ShotGun,
        Riple,
        Sniper,
        knife,
        Spear,
        ChainSaw,
        Bat,
        None,
    }

    /// <summary>
    /// <br>아보카도 성장 단계 입니다.</br>
    /// <br>Resources/Images/Abocado 에 이름에 맞는 이미지가 필요합니다.</br>
    /// <br>이미지가 없으면 Default 이미지로 설정됩니다.</br>
    /// </summary>
    public enum Abocado : sbyte
    {
        Cultivated = 0,
        Seed = 1,
        Tree = 2,
        Fruited = 3
    }

    /// <summary>
    /// <br>아보카도 타워 종류 입니다.</br> 
    /// </summary>
    public enum Tower : sbyte
    {
        Guard_I,
        Guard_II,
        Auto_I,
        Auto_II,
        Splash_I,
        Splash_II,
        Production_I,
        Production_II,
        Heal_I,
        Heal_II,
    }
}
