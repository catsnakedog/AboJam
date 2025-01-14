using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MeleeWeapon
{
    public float AttackMotionTime = 0.2f;

    private Coroutine _attackMotionCoroutine;

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
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void InitBeforeChange()
    {
        base.InitBeforeChange();
        if (_attackMotionCoroutine != null)
            StopCoroutine(_attackMotionCoroutine);
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    private IEnumerator AttackMotion()
    {
        float motionTime = 1 / AttackSpeed;
        if (motionTime >= AttackMotionTime)
            motionTime = AttackMotionTime;
        float time = 0;

        AttackEffectObj.SetActive(true);
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
        AttackEffectObj.SetActive(false);
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void AttackLogic()
    {
        StartCoroutine(AttackMotion());
    }
}