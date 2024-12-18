using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWeapon : Weapon
{
    public override void InitializeWeapon(Vector2 weaponPlace)
    {
        base.InitializeWeapon(weaponPlace);
        // 몬스터 무기 초기화 로직 추가 가능
    }
}