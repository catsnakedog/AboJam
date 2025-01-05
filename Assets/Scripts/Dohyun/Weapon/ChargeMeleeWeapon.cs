using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMeleeWeapon : MeleeWeapon
{
    public bool IsCharge = false;
    public float ChargeMaxTime = 2;
    public float ChargeMinTime = 0;
    public float ChargeTime = 0;
    public Coroutine SwingCoroutine;

    private Action _chargeAction = null;

    public override void AttackLogic()
    {
        if (IsCharge)
        {
            if (ChargeTime >= ChargeMaxTime)
            {
                ChargeTime = ChargeMaxTime;
                return;
            }
            ChargeTime += Time.deltaTime;
        }
    }

    public override void AttackStartLogic()
    {
        IsCharge = true;
        _chargeAction = ChargeEffect;
        ChargeTime = 0;
    }

    public override void AttackEndLogic()
    {
        IsCharge = false;
        ChargeAttack(ChargeTime);
        _chargeAction -= ChargeEffect;
        ChargeTime = 0;
    }

    public override void InitSetMain()
    {
        IsCharge = false;
        _chargeAction = null;
        ChargeTime = 0;
        StopAllCoroutines();
    }

    public override void InitSetHold()
    {
        IsCharge = false;
        _chargeAction = null;
        ChargeTime = 0;
        StopAllCoroutines();
    }

    public virtual void SkipSwingEffect()
    {
        if (SwingCoroutine != null)
            StopCoroutine(SwingCoroutine);
    }

    virtual public void ChargeEffect()
    {

    }

    virtual public void ChargeAttack(float chargeTime)
    {

    }

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        AttackType = WeaponAttackType.Charge;
    }

    public void Update()
    {
        _chargeAction?.Invoke();
    }
}
