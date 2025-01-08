using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeWeapon : Weapon
{
    public GameObject AttackEffectObj;
    public Type AttackEffectType = typeof(Type);

    [System.Serializable]
    public class MeleeWeaponData
    {
        public float Damage;
        public float AttackSpeed;
    }

    [SerializeField]
    public List<MeleeWeaponData> WeaponDatas;

    public override void WeaponSetting()
    {
        AttackType = WeaponAttackType.Melee;
        AttackSpeed = WeaponDatas[Level-1].AttackSpeed;
        IsReloadOnAttack = false;
        HandLogic = new MeleeHandLogic();
    }
}