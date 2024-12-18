using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
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
    public Transform FireLocation;
    public Transform SecondHandLocation;
    public GameObject BulletObj;

    public Type Bullet = typeof(Bullet);
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