using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSaw : GageMeleeWeapon
{
    public float AttackAngleCorretion;
    public GameObject ChainSawPrefab;
    public float AngleRotateSpeed;

    private GameObject _chainSawEffectObj;
    private SpriteRenderer _chainSawEffectRenderer;
    private Transform _effectRoot;
    private MaterialPropertyBlock _attackProperty;
    private float _angle = 0;

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        _attackProperty = new();
        _effectRoot = GameObject.FindWithTag("Player").transform.Find("EffectRoot");
        _angle = 0;
    }

    public override void InitSetMain()
    {
        base.InitSetMain();
        _chainSawEffectObj = Instantiate(ChainSawPrefab, _effectRoot.transform, false);
        _chainSawEffectRenderer = _chainSawEffectObj.GetComponent<SpriteRenderer>();
        _attackProperty.SetFloat("_AttackFlag", 0.0f);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);
        _angle = 0;
    }

    public override void InitSetHold()
    {
        base.InitSetHold();
        if(_chainSawEffectObj != null)
            Destroy(_chainSawEffectObj);
    }

    public override void InitBeforeDisable()
    {
        base.InitBeforeDisable();
        if (_chainSawEffectObj != null)
            Destroy(_chainSawEffectObj);
    }

    public override void Trigger()
    {
        base.Trigger();
        _angle = 0;

        AngleCorrection = AttackAngleCorretion;
        WeaponAngleCorrection();
        _attackProperty.SetFloat("_AttackFlag", 1.0f);
        _attackProperty.SetFloat("_EffectAngle", _angle);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);
    }

    public override void TriggerLogic()
    {
        _angle += Time.deltaTime * AngleRotateSpeed;
        if (_angle > 360) _angle -= 360;
        _attackProperty.SetFloat("_EffectAngle", _angle);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);
    }

    public override void TriggerEnd()
    {
        base.TriggerEnd();
        _angle = 0;
        AngleCorrection = DefaultAngleCorrection;
        WeaponAngleCorrection();
        _attackProperty.SetFloat("_AttackFlag", 0.0f);
        _attackProperty.SetFloat("_EffectAngle", _angle);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);
    }
}
