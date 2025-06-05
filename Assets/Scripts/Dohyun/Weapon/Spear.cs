using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spear : MeleeWeapon
{
    public static Action eventAttack;

    [System.Serializable]
    public class SpearEffectData
    {
        public float Time = 1f;
        public float Speed = 1f;
        public StartEndData ShaftWidth = new(0.05f, 0.01f);
        public StartEndData ShaftHeight = new(0.28f, 0.2f);
        public StartEndData HeadHeight = new(0.24f, 0f);
        public StartEndData HeadWidth = new(0.15f, 0f);
        public StartEndData NoiseX = new(0f, 0.18f);
        public StartEndData NoiseY = new(0f, 0.018f);
        public StartEndData OffsetY = new(0.2f, -0.2f);

        public SpearEffectData(SpearEffectData other)
        {
            HeadWidth = other.HeadWidth;
            HeadHeight = other.HeadHeight;
            ShaftWidth = other.ShaftWidth;
            ShaftHeight = other.ShaftHeight;
            NoiseX = other.NoiseX;
            NoiseY = other.NoiseY;
            OffsetY = other.OffsetY;
            Time = other.Time;
            Speed = other.Speed;
        }
    }
    [System.Serializable]
    public class StartEndData
    {
        public float Start;
        public float End;

        public StartEndData(float start, float end)
        {
            Start = start;
            End = end;
        }
    }

    public GameObject SpearObj;
    public SpearEffectData SpearEffect;
    public Transform FireLocation;
    [Range(0f, 1f)]
    public float SpearMotionRate = 0.5f;
    public float FrontMotionLength = 0.2f;

    private Type _spearObjType = typeof(SpearObj);
    private Camera _mainCamera;
    private Coroutine _motionCoroutine = null;

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        IsReloadOnAttack = true;
        _motionCoroutine = null;
        _mainCamera = Camera.main;
        IsImageWidth = false;
        transform.localScale = new Vector3(1, -1, 1);
        HandLogic = new RangedHandLogic();
    }

    public override void InitSetHold()
    {
        base.InitSetHold();
    }

    public override void InitSetMain()
    {
        IsHandFixed = false;
        MouseFlipPrio = false;
        base.InitSetMain();
        ResetMotion();
    }

    public override void InitBeforeChange()
    {
        IsHandFixed = false;
        MouseFlipPrio = false;
        base.InitBeforeChange();
        ResetMotion();
    }

    private void ResetMotion()
    {
        transform.localPosition = Vector3.zero;
        if (_motionCoroutine != null)
            StopCoroutine(_motionCoroutine);
    }

    public override void AttackLogic()
    {
        eventAttack.Invoke();
        IsHandFixed = true;
        MouseFlipPrio = true;
        FixedRot = transform.parent.parent.rotation;
        GameObject spearObj = ObjectPool.Instance.GetObj(_spearObjType, SpearObj, 10);
        spearObj.transform.localPosition = Vector3.zero;
        spearObj.transform.SetParent(FireLocation, false);
        spearObj.GetComponent<SpearObj>().Init(SpearEffect, transform.rotation);
        ResetMotion();
        _motionCoroutine = StartCoroutine(SpearMotion());
    }

    private IEnumerator SpearMotion()
    {
        float time = 0;
        float motionTime = 1 / AttackSpeed;
        float process;

        // 공격 적용
        melee.Attack(melee.transform.position);

        while (time < motionTime * SpearMotionRate)
        {
            process = time / (motionTime * SpearMotionRate);
            transform.localPosition = new Vector3(0, Mathf.Lerp(0, FrontMotionLength, process), 0);
            yield return null;
            time += Time.deltaTime;
        }
        transform.localPosition = new Vector3(0, FrontMotionLength, 0);
        time = 0;

        while (time < motionTime * (1 - SpearMotionRate))
        {
            process = time / (motionTime * (1 - SpearMotionRate));
            transform.localPosition = new Vector3(0, Mathf.Lerp(FrontMotionLength, 0, process), 0);
            yield return null;
            time += Time.deltaTime;
        }
        transform.localPosition = Vector3.zero;
        IsHandFixed = false;
        MouseFlipPrio = false;
    }
}