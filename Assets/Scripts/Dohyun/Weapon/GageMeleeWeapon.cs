using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GageMeleeWeapon : MeleeWeapon
{
    public float Gage = 0;
    public float GageMax = 5;
    public float GageTriggerMin = 1;
    public float GageHealValue = 1;
    public float GageUseValue = 1;

    private Action _gageAction = null;
    private bool _trigger = false;

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        AttackType = WeaponAttackType.Gage;
        Gage = 0;
        _trigger = false;
        _gageAction += HealGage;
    }

    public override void AttackStartLogic()
    {
        if (_trigger)
            return;

        base.AttackStartLogic();
        if(CheckTrigger(Gage))
        {
            _gageAction = null;
            _trigger = true;
        }
    }

    private bool CheckTrigger(float gage)
    {
        if (gage >= GageTriggerMin) return true;

        return false;
    }

    private void HealGage()
    {
        Gage += Time.deltaTime * GageHealValue;
    }

    public override void AttackLogic()
    {
        base.AttackLogic();
        if(_trigger)
        {
            Gage -= Time.deltaTime * GageUseValue;
        }
    }

    public override void AttackEndLogic()
    {
        base.AttackEndLogic();
        _gageAction += HealGage;
    }

    public void Update()
    {
        _gageAction?.Invoke();
    }
}