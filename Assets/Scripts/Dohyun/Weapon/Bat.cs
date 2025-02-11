using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static HandUtil;

public class Bat : ChargeMeleeWeapon
{
    public GameObject BatPrefab;
    public float AttackRemainTime;
    public float SwingAngle;
    public float SwingTime;

    private MaterialPropertyBlock _attackProperty;
    private readonly Type _batObjType = typeof(BatObj);
    private GameObject _batAttackEffectObj;
    private SpriteRenderer _batAttackEffectRenderer;
    private Transform _effectRoot;
    private Camera _mainCamera;
    private float _angle;
    private float _radius;
    private bool _standardHand;

    public override void InitSetMain()
    {
        base.InitSetMain();
        ResetValue();
    }

    public override void InitSetHold()
    {
        base.InitSetHold();
        ResetValue();
        transform.localScale = new Vector3(1, 1, 1);
        if (_batAttackEffectObj != null && _batAttackEffectObj.activeSelf)
            ObjectPool.Instance.Return(_batObjType, _batAttackEffectObj);
    }

    public override void InitBeforeChange()
    {
        HandState.StandardHand = _standardHand;
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public override void InitBeforeDisable()
    {
        base.InitBeforeDisable();
        ResetValue();
        transform.localScale = new Vector3(1, 1, 1);
        if (_batAttackEffectObj != null && _batAttackEffectObj.activeSelf)
            ObjectPool.Instance.Return(_batObjType, _batAttackEffectObj);
    }

    public void ResetValue()
    {
        AttackEffectObj.SetActive(false);
        HandState.StandardHand = _standardHand;
        HandPrio = false;
        WeaponScalePrio = false;
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public override void ChargeAttack(float chargeTime)
    {
        if(chargeTime < ChargeMinTime)
        {
            ResetValue();
            transform.localScale = new Vector3(1, 1, 1);
            if (_batAttackEffectObj != null && _batAttackEffectObj.activeSelf)
                ObjectPool.Instance.Return(_batObjType, _batAttackEffectObj);

            return;
        }
        _batAttackEffectObj.transform.SetParent(null, true);
        SkipSwingEffect();
        SwingCoroutine = StartCoroutine(SwingBat());
        _attackProperty.SetFloat("_AttackFlag", 1.0f);
        _batAttackEffectRenderer.SetPropertyBlock(_attackProperty);
        StartCoroutine(SetChargeAttackRemain(_batAttackEffectObj, _attackProperty.GetFloat("_RadiusScale"), _attackProperty.GetFloat("_AttackFlag"), _attackProperty.GetFloat("_LineAngle")));

        // 차징에 따른 공격 적용
        Vector3 worldPos = _batAttackEffectObj.transform.position;
        float radius = melee.radius * _attackProperty.GetFloat("_RadiusScale");
        float damage = melee.damage * _attackProperty.GetFloat("_RadiusScale");
        float knockback = melee.knockback * _attackProperty.GetFloat("_RadiusScale");
        melee.Attack(worldPos, radius, damage, knockback);
    }

    public override void SkipSwingEffect()
    {
        base.SkipSwingEffect();
        HandState.StandardHand = _standardHand;
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    private IEnumerator SwingBat()
    {
        if (transform.localScale.y == -1)
        {
            HandState.StandardHand = !_standardHand;
            transform.localRotation = Quaternion.Euler(0, 0, 180);
            AttackEffectObj.SetActive(true);
        }
        else
        {
            HandState.StandardHand = !_standardHand;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            AttackEffectObj.SetActive(true);
        }

        yield return new WaitForSeconds(SwingTime);
        HandState.StandardHand = _standardHand;
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    private IEnumerator SetChargeAttackRemain(GameObject batEffectObj, float radiusScale, float attackFlag, float lineAngle)
    {
        MaterialPropertyBlock property = new();
        property.SetFloat("_RadiusScale", radiusScale);
        property.SetFloat("_AttackFlag", attackFlag);
        property.SetFloat("_LineAngle", lineAngle);
        _batAttackEffectRenderer.SetPropertyBlock(property);
        yield return new WaitForSeconds(AttackRemainTime);
        batEffectObj.transform.localScale = new Vector3(4, 4, 1);
        ObjectPool.Instance.Return(_batObjType, batEffectObj);
        batEffectObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        property.SetFloat("_RadiusScale", 0.0f);
        property.SetFloat("_AttackFlag", 0.0f);
    }

    public override void ChargeAttackStart()
    {
        SkipSwingEffect();
        _batAttackEffectObj = ObjectPool.Instance.GetObj(_batObjType, BatPrefab, 3);
        _batAttackEffectObj.transform.SetParent(_effectRoot, false);
        _batAttackEffectObj.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * _angle) * _radius, Mathf.Sin(Mathf.Deg2Rad * _angle) * _radius);
        _batAttackEffectRenderer = _batAttackEffectObj.GetComponent<SpriteRenderer>();
        _radius = _batAttackEffectRenderer.material.GetFloat("_LineRadius") * 4;
        _attackProperty.SetFloat("_AttackFlag", 0.0f);
        _batAttackEffectRenderer.SetPropertyBlock(_attackProperty);
    }

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        _attackProperty = new();
        _effectRoot = GameObject.FindWithTag("Player").transform.Find("EffectRoot");
        _mainCamera = Camera.main;
        _standardHand = HandState.StandardHand;
    }

    public override void ChargeEffect()
    {
        _angle = ForwardToMouse(_effectRoot.gameObject, _mainCamera, 0f).eulerAngles.z;
        float process = Mathf.Clamp(ChargeTime, ChargeMinTime, ChargeMaxTime) / ChargeMaxTime;
        if(transform.localScale.y == 1)
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 - process * SwingAngle));
        else
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90 + process * SwingAngle));

        if(_effectRoot.transform.parent.localRotation.eulerAngles.y == 180)
            _effectRoot.transform.localScale = new Vector3(-1, 1, 1);
        else
            _effectRoot.transform.localScale = new Vector3(1, 1, 1);
        _batAttackEffectObj.transform.localPosition =  new Vector3(Mathf.Cos(Mathf.Deg2Rad * _angle) * _radius, Mathf.Sin(Mathf.Deg2Rad * _angle) * _radius);
        _attackProperty.SetFloat("_LineAngle", _angle + 180);
        _attackProperty.SetFloat("_RadiusScale", Mathf.Clamp(process/2 + 0.5f, 0.5f, 1));
        _batAttackEffectRenderer.SetPropertyBlock(_attackProperty);
    }
}
