using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangedWeapon : Weapon
{
    public Transform FireLocation;
    public GameObject BulletObj;
    public Type Bullet = typeof(Bullet);
}
