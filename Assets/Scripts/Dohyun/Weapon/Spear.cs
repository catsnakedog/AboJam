using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spear : MeleeWeapon
{
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

    private Type _spearObjType = typeof(SpearObj);
    private Camera _mainCamera;

    public override void WeaponSetting()
    {
        base.WeaponSetting();
        IsReloadOnAttack = true;
        _mainCamera = Camera.main;
        IsImageWidth = false;
        transform.localScale = new Vector3(1, -1, 1);
        HandLogic = new RangedHandLogic();
    }

    public override void AttackStartLogic()
    {   
    }

    public override void AttackLogic()
    {
        GameObject spearObj = ObjectPool.Instance.GetObj(_spearObjType, SpearObj, 10);
        spearObj.transform.localPosition = Vector3.zero;
        spearObj.transform.SetParent(FireLocation, false);
        spearObj.GetComponent<SpearObj>().Init(SpearEffect, transform.rotation);
    }

    public override void AttackEndLogic()
    {

    }
}