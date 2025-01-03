using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeWeapon : Weapon
{
    public GameObject AttackEffectObj;
    public Type AttackEffectType = typeof(Type);

    [System.Serializable]
    public class RangedWeaponData
    {
        public float Damage;
        public float AttackSpeed;
    }

    [SerializeField]
    public List<RangedWeaponData> WeaponDatas;

    public override void WeaponSetting()
    {
        AttackType = WeaponAttackType.Melee;
        AttackSpeed = WeaponDatas[Level].AttackSpeed;
        HandLogic = new MeleeHandLogic();
    }
}