using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedWeapon : Weapon
{
    public Launcher launcher;
    public Transform FireLocation;
    public GameObject BulletObj;
    public Type Bullet = typeof(Bullet);

    [System.Serializable]
    public class RangedWeaponData
    {
        public float Damage;
        public float BulletSpeed;
        public float Spread;
        public float AttackSpeed;
        public float Range;
        public int bulletPenetration;
    }

    [SerializeField]
    public List<RangedWeaponData> WeaponDatas;

    public override void WeaponSetting()
    {
        AttackType = WeaponAttackType.Ranged;
        AttackSpeed = WeaponDatas[Level-1].AttackSpeed;
        IsReloadOnAttack = true;
        HandLogic = new RangedHandLogic();
    }
}
