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
    private float _lastChargeTime = 0;

    private Action _chargeAction = null;
    private Action _chargeLateAction = null;

    public override void AttackLogic()
    {
        if (!IsCharge)
        {
            IsCharge = true;
            _chargeLateAction = CheckCharge;
            _chargeAction = ChargeEffect;
            ChargeTime = 0;
            _lastChargeTime = -1;
        }
        else
        {
            ChargeTime += Time.deltaTime;
        }
    }

    private void CheckCharge()
    {
        if (_lastChargeTime == ChargeTime)
        {
            ChargeEnd();
        }
        else
        {
            _lastChargeTime = ChargeTime;
        }
    }

    virtual public void ChargeEnd()
    {
        Debug.Log(ChargeTime);

    }

    virtual public void ChargeEffect()
    {

    }

    public void Update()
    {
        _chargeAction?.Invoke();
    }

    public void LateUpdate()
    {
        _chargeLateAction?.Invoke();
    }
}
