using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    public override void InitializeWeapon(Vector2 weaponPlace)
    {
        base.InitializeWeapon(weaponPlace);
        // 추가 초기화 로직이 필요한 경우 여기에 작성
    }
}
