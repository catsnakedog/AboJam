using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MeleeWeapon
{
    public float AttackMotionTime = 0.2f;
    public float AttackEffectTime = 1f;
    public float AttackEffectSpeed = 1f;
    public GameObject SlashEffectObj;
    public Transform FireLocation;
    public static Action<Vector3> eventAttack;

    private Coroutine _attackMotionCoroutine;
    private Type _knifeObjType = typeof(KnifeObj);

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        IsReloadOnAttack = true;
        _attackMotionCoroutine = null;
        HandLogic = new RangedHandLogic();
    }

    public override void InitSetHold()
    {
        base.InitSetHold();
        if(_attackMotionCoroutine != null)
            StopCoroutine(_attackMotionCoroutine);
    }

    public override void InitSetMain()
    {
        base.InitSetMain();
        if (_attackMotionCoroutine != null)
            StopCoroutine(_attackMotionCoroutine);
        SlashEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void InitBeforeChange()
    {
        base.InitBeforeChange();
        if (_attackMotionCoroutine != null)
            StopCoroutine(_attackMotionCoroutine);
        SlashEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void InitBeforeDisable()
    {
        base.InitBeforeDisable();
        if (_attackMotionCoroutine != null)
            StopCoroutine(_attackMotionCoroutine);
        SlashEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    private IEnumerator AttackMotion()
    {
        // 공격 적용
        melee.Attack(melee.transform.position);

        GameObject knifeObj = ObjectPool.Instance.GetObj(_knifeObjType, AttackEffectObj, 10);
        knifeObj.transform.position = FireLocation.position;
        knifeObj.transform.SetParent(null, true);
        knifeObj.GetComponent<KnifeObj>().Init(AttackEffectTime, AttackEffectSpeed, transform.rotation);

        float motionTime = 1 / AttackSpeed;
        if (motionTime >= AttackMotionTime)
            motionTime = AttackMotionTime;
        float time = 0;

        SlashEffectObj.SetActive(true);
        while (time < motionTime - 0.05f)
        {
            float process = time / (motionTime - 0.05f);
            if (transform.localScale.y == 1)
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(90f, 180f, process));
            else
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(90f, 0f, process));
            yield return null;
            time += Time.deltaTime;
        }
        SlashEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void AttackLogic()
    {
        eventAttack.Invoke(gameObject.transform.position);
        StartCoroutine(AttackMotion());
    }
}