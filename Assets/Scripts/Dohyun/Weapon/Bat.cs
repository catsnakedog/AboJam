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
    private Type _batObjType = typeof(BatObj);
    private GameObject _batEffectObj;
    private BatObj _batObj;
    private SpriteRenderer _batEffectRenderer;
    private Transform _effectRoot;
    private Camera _mainCamera;
    private float _angle;
    private float _radius;
    private bool _standardHand;

    public override void InitSetMain()
    {
        base.InitSetMain();
        AttackEffectObj.SetActive(false);
        HandState.StandardHand = _standardHand;
        HandPrio = false;
        WeaponScalePrio = false;
        transform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public override void InitSetHold()
    {
        base.InitSetHold();
        AttackEffectObj.SetActive(false);
        HandState.StandardHand = _standardHand;
        HandPrio = false;
        WeaponScalePrio = false;
        transform.localRotation = Quaternion.Euler(0, 0, 90);
        transform.localScale = new Vector3(1, 1, 1);
        if (_batEffectObj != null && _batEffectObj.activeSelf)
            ObjectPool.Instance.Return(_batObjType, _batEffectObj);
    }

    public override void ChargeAttack(float chargeTime)
    {
        StartCoroutine(Reload());
        _batEffectObj.transform.SetParent(null, true);
        SkipSwingEffect();
        SwingCoroutine = StartCoroutine(SwingBat());
        _attackProperty.SetFloat("_AttackFlag", 1.0f);
        _batEffectRenderer.SetPropertyBlock(_attackProperty);
        _batObj.Attack();
        StartCoroutine(SetChargeAttackEnd(_batEffectObj, _attackProperty));
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

    private IEnumerator SetChargeAttackEnd(GameObject batEffectObj, MaterialPropertyBlock attackProperty)
    {
        yield return new WaitForSeconds(AttackRemainTime);
        batEffectObj.transform.localScale = new Vector3(4, 4, 1);
        ObjectPool.Instance.Return(_batObjType, batEffectObj);
        batEffectObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        attackProperty.SetFloat("_RadiusScale", 0.0f);
        attackProperty.SetFloat("_AttackFlag", 0.0f);
    }

    public override void AttackStartLogic()
    {
        base.AttackStartLogic();
        _batEffectObj = ObjectPool.Instance.GetObj(_batObjType, BatPrefab, 3);
        _batEffectObj.transform.SetParent(_effectRoot, false);
        _batEffectObj.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * _angle) * _radius, Mathf.Sin(Mathf.Deg2Rad * _angle) * _radius);
        _batEffectRenderer = _batEffectObj.GetComponent<SpriteRenderer>();
        _batObj = _batEffectObj.GetComponent<BatObj>();
        _radius = _batEffectRenderer.material.GetFloat("_LineRadius") * 4;
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
        _batEffectObj.transform.localPosition =  new Vector3(Mathf.Cos(Mathf.Deg2Rad * _angle) * _radius, Mathf.Sin(Mathf.Deg2Rad * _angle) * _radius);
        _attackProperty.SetFloat("_LineAngle", _angle + 180);
        _attackProperty.SetFloat("_RadiusScale", Mathf.Clamp(process/2 + 0.5f, 0.5f, 1));
        _batEffectRenderer.SetPropertyBlock(_attackProperty);
    }
}
