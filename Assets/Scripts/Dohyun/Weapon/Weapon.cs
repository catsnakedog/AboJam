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
    }

    public enum HoldingLocationOrder
    {
        Place1 = 0,
        Place2 = -2,
    }

    public enum WeaponAttackType
    {
        Melee = 0,
        Range = 1,
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

    public void Init()
    {
        WeaponSetting();
        IsReload = false;
        IsStartAttack = false;
        StartCoroutine(Reload());
    }

    public void SetMain()
    {
        IsReload = false;
        IsStartAttack = false;
        StartCoroutine(Reload());
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

    private IEnumerator Reload()
    {
        IsReload = true;
        yield return new WaitForSeconds(1 / AttackSpeed);
        IsReload = false;
    }

    private void OnEnable()
    {
        Init();
    }
}