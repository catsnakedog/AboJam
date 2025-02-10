using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Upgrade : MonoBehaviour
{
    /* Dependency */
    public Button BTN_Damage;
    public Button BTN_Speed;
    public Button BTN_Range;
    public Button BTN_Knockback;

    public GameObject[] projectiles;
    public Weapon[] weapons;
    public Melee[] meleeWeapons;

    /* Field & Property */
    public static Upgrade instance;

    private float multiplierDamage = 1; // 단리
    private float multiplierRate = 1; // 복리
    private float multiplierRange = 1; // 복리
    private float multiplierKnockback = 1; // 단리

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
    }

    /* Public Method */
    public void Apply()
    {
        ApplyDamage();
        ApplyRate();
        ApplyRange();
        ApplyKnockback();

        void ApplyDamage()
        {
            // 플레이어 총알 프리팹에 데미지 계수 적용
            foreach(Projectile projectile in Projectile.instances)
                if(projectiles.Select(obj => obj.name).ToArray().Contains(projectile.gameObject.name))
                    projectile.multiplierDamage = multiplierDamage;

            // 플레이어 근접 무기에 데미지 계수 적용
            foreach (Melee melee in meleeWeapons) melee.multiplierDamage = multiplierDamage;
        }
        void ApplyRate()
        {
            // 플레이어 모든 무기에 공격 속도 계수 적용
            foreach(Weapon weapon in weapons) weapon.AttackSpeed *= multiplierRate;
        }
        void ApplyRange()
        {
            // 플레이어 원거리 무기에 사거리 계수 적용
            foreach (Weapon weapon in weapons)
                if(weapon is RangedWeapon) ((RangedWeapon)weapon).WeaponDatas[0].Range *= multiplierRange;
        }
        void ApplyKnockback()
        {
            // 플레이어 근접 무기에 넉백 계수 적용
            foreach (Melee melee in meleeWeapons) melee.multiplierKnockback = multiplierKnockback;
        }
    }
}