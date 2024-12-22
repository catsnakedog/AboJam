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

    [System.Serializable]
    public class WeaponHandState
    {
        public bool StandardHand;
        public bool LeftUse;
        public bool RightUse;
    }

    [System.Serializable]
    public class WeaponData
    {
        public float Damage;
        public float BulletSpeed;
        public float Spread;
        public float AttackSpeed;
        public float Range;
        public int bulletPenetration;
    }

    [SerializeField]
    public List<WeaponData> WeaponDatas;
    public int Level;
    public Transform SecondHandLocation;
    public HoldingLocationIndex HoldingIndex;
    public HoldingLocationOrder HoldingOrder;
    public SpriteRenderer Renderer;
    public WeaponHandState HandState;
    public EnumData.Weapon WeaponType;

    private bool isReload = false;

    public void Init()
    {
        isReload = false;
        StartCoroutine(Reload());
    }

    public void Attack()
    {
        if (isReload)
            return;
        AttackLogic();
        StartCoroutine(Reload());
    }

    public virtual void AttackLogic()
    {
    }

    private IEnumerator Reload()
    {
        isReload = true;
        yield return new WaitForSeconds(1 / WeaponDatas[Level-1].AttackSpeed);
        isReload = false;
    }

    public void OnDisable()
    {
        isReload = false;
    }
}