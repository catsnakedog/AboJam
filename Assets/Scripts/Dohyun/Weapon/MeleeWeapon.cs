using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeWeapon : Weapon
{
    public GameObject AttackEffectObj;
    public Type AttackEffectType = typeof(Type);

    public override void WeaponSetting()
    {
        AttackType = WeaponAttackType.Melee;
    }
}