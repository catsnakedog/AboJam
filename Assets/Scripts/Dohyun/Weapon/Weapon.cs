using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum HoldingLocationIndex
    {
        Place1 = 0,
        Place2 = 1,
        Place3 = 2,
        Place4 = 3,
    }

    public enum HoldingLocationOrder
    {
        Place1 = 0,
        Place2 = -2,
        Place3 = -2,
        Place4 = -2,
    }

    public enum WeaponAttackType
    {
        Melee = 0,
        Ranged = 1,
        Charge = 2,
        Gage = 3
    }


    [System.Serializable]
    public class WeaponHandState
    {
        public bool StandardHand;
        public bool LeftUse;
        public bool RightUse;
    }

    public int Level;
    public float ClothesShake;
    public Transform SecondHandLocation;
    public HoldingLocationIndex HoldingIndex;
    public HoldingLocationOrder HoldingOrder;
    public SpriteRenderer Renderer;
    public WeaponHandState HandState;
    public EnumData.Weapon WeaponType;
    public WeaponAttackType AttackType;
    public IHandLogic HandLogic;

    public float AttackSpeed = 1;
    public bool IsReload = false;
    public bool IsStartAttack = false;
    public bool IsReloadOnAttack = true;
    public bool HandPrio = false;
    public bool WeaponScalePrio = false;
    public bool MouseFlipPrio = false;
    public bool IsHandFixed = false;
    public Quaternion FixedRot = default;
    public bool IsImageWidth = true;

    public void Init()
    {
        WeaponSetting();
        HandPrio = false;
        WeaponScalePrio = false;
        IsHandFixed = false;
        IsReload = false;
        IsStartAttack = false;
        StartCoroutine(Reload());
    }

    public void SetMain()
    {
        InitSetMain();
        IsReload = false;
        IsStartAttack = false;
        StartCoroutine(Reload());
    }

    public virtual void InitSetMain()
    {

    }

    public virtual void InitSetHold()
    {

    }

    public virtual void InitBeforeChange()
    {

    }

    public virtual void InitBeforeDisable()
    {

    }

    public virtual void WeaponSetting()
    {

    }

    public void AttackStart()
    {
        if (IsReload)
            return;
        IsStartAttack = true;
        AttackStartLogic();
    }

    public void Attack()
    {
        if (IsReload)
            return;
        AttackLogic();
        if (IsReloadOnAttack)
            StartCoroutine(Reload());
    }

    public void AttackEnd()
    {
        if (!IsStartAttack)
            return;
        IsStartAttack = false;
        AttackEndLogic();
    }


    public virtual void AttackStartLogic()
    {
    }

    public virtual void AttackLogic()
    {
    }

    public virtual void AttackEndLogic()
    {
    }

    public IEnumerator Reload()
    {
        IsReload = true;
        yield return new WaitForSeconds(1 / AttackSpeed);
        IsReload = false;
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        InitBeforeDisable();
    }
}