using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GageMeleeWeapon : MeleeWeapon
{
    public float Gage = 0;
    public float GageMax = 5;
    public float GageTriggerMin = 1;
    public float GageHealValue = 1;
    public float GageUseValue = 1;
    public GameObject GageBarPrefab;
    public float DefaultAngleCorrection = 45;
    public float AngleCorrection;
    public float ShakeValue = 0.5f;

    private Action _gageAction = null;
    private bool _trigger = false;
    private WeaponGageBar _gageBar;
    private Vector3 _parentOriPos;
    private GameObject _shakeTarget;

    public override void InitSetMain()
    {
        _trigger = false;
        _gageBar = Instantiate(GageBarPrefab).GetComponent<WeaponGageBar>();
        _gageBar.Init();
        _gageBar.gameObject.SetActive(true);
        _gageAction += HealGage;
        AngleCorrection = DefaultAngleCorrection;
        WeaponAngleCorrection();
    }

    public override void InitSetHold()
    {
        _gageAction = null;
        if(_gageBar != null && _gageBar.gameObject != null)
            Destroy(_gageBar.gameObject);
    }

    public override void InitBeforeChange()
    {
        TriggerEnd();
    }

    public override void InitBeforeDisable()
    {
        base.InitBeforeDisable();
        _gageAction = null;
        if (_gageBar != null && _gageBar.gameObject != null)
            Destroy(_gageBar.gameObject);
    }

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        AttackType = WeaponAttackType.Gage;
        Gage = 0;
        _trigger = false;
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
            _gageBar.gameObject.SetActive(true);
            Trigger();
        }
    }

    private bool CheckTrigger(float gage)
    {
        if (gage >= GageTriggerMin) return true;

        return false;
    }

    private bool CheckGage()
    {
        if (Gage <= 0) return false;

        return true;
    }

    virtual public void Trigger()
    {
        _shakeTarget = transform.parent.parent.gameObject;
        _parentOriPos = _shakeTarget.transform.localPosition;
    }

    virtual public void TriggerEnd()
    {
        _gageAction = HealGage;
        _trigger = false;
        _shakeTarget.transform.localPosition = _parentOriPos;
    }

    virtual public void TriggerLogic()
    {

    }

    private void HealGage()
    {
        Gage += Time.deltaTime * GageHealValue;
        if(Gage > GageMax) Gage = GageMax;
        _gageBar.SetGageValue(Gage / GageMax);
    }

    public void WeaponAngleCorrection()
    {
        if (transform.localScale.y == 1)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 - AngleCorrection));
        else
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + AngleCorrection));
    }

    public void ShakeWeaponCorrection()
    {
        if(_shakeTarget != null)
            _shakeTarget.transform.localPosition = _parentOriPos;
        _shakeTarget = transform.parent.parent.gameObject;
        _parentOriPos = _shakeTarget.transform.localPosition;
    }

    public void ShakeWeapon()
    {
        float offsetX = UnityEngine.Random.Range(-ShakeValue, ShakeValue) / 1000.0f;
        float offsetY = UnityEngine.Random.Range(-ShakeValue, ShakeValue) / 2000.0f;

        _shakeTarget.transform.localPosition = _parentOriPos + new Vector3(offsetX, offsetY, 0);
    }

    public override void AttackLogic()
    {
        base.AttackLogic();
        if(_trigger)
        {
            Gage -= Time.deltaTime * GageUseValue;
            if (Gage < 0) Gage = 0;
            _gageBar.SetGageValue(Gage / GageMax);
            ShakeWeapon();
            if(CheckGage())
                TriggerLogic();
            else
                TriggerEnd();
        }
    }

    public override void AttackEndLogic()
    {
        base.AttackEndLogic();
        if (!_trigger) return;
        TriggerEnd();
    }

    public void Update()
    {
        _gageAction?.Invoke();
    }
}