using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
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
    private Coroutine corLast;
    public static Action eventStart;
    public static Action eventEnd;
    public bool GetTrigger { get => _trigger; }

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
        Debug.Log("전기톱 스타트");
        eventStart.Invoke();
        base.Trigger();
        _angle = 0;

        AngleCorrection = AttackAngleCorretion;
        WeaponAngleCorrection();
        _attackProperty.SetFloat("_AttackFlag", 1.0f);
        _attackProperty.SetFloat("_EffectAngle", _angle);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);

        // 공격 적용
        corLast = StartCoroutine(CorAttack());
        IEnumerator CorAttack()
        {
            while (true)
            {
                melee.Attack(melee.transform.position);
                yield return new WaitForSeconds(0.1f);
            }
        }
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
        Debug.Log("전기톱 끝");
        eventEnd.Invoke();
        base.TriggerEnd();
        _angle = 0;
        AngleCorrection = DefaultAngleCorrection;
        WeaponAngleCorrection();
        _attackProperty.SetFloat("_AttackFlag", 0.0f);
        _attackProperty.SetFloat("_EffectAngle", _angle);
        _chainSawEffectRenderer.SetPropertyBlock(_attackProperty);

        // 공격 종료
        if(corLast != null) StopCoroutine(corLast);
    }
}
