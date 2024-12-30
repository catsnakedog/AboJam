using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeWeapon : Weapon
{
    public bool IsCharged = true;
    public float ChargedMaxTime = 2;
    public float ChargedMinTime = 0;

    virtual public void ChargeEffect()
    {

    }
}
