using UnityEngine;
using UnityEngine.UI;
using System;

public class Upgrade : MonoBehaviour
{
    /* Dependency */
    public Button BTN_HP;
    public Button BTN_Damage;
    public Button BTN_Speed;
    public Button BTN_Range;
    public Button BTN_Knockback;

    public Projectile[] projectiles;
    public Weapon[] weapons;
    public Melee[] melees;
    public Launcher[] launchers;

    /* Field & Property */
    public static Upgrade instance;
    public float multiplierDamage = 1;    // 단리
    public float multiplierRate = 1;      // 복리
    public float multiplierRange = 1;     // 복리
    public float multiplierKnockback = 1; // 단리

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
    }

    /* Public Method */
    public void Apply()
    {
        ApplyMelee();
        ApplyRate();
        ApplyRange();
        ApplyKnockback();
    }
    public void ApplyRanged(Projectile projectile)
    {
        if (projectile.gameObject.name.Contains("Player")) projectile.multiplierDamage = multiplierDamage;
    }
    public void ApplyMelee()
    {
        foreach (Melee melee in melees) melee.multiplierDamage = multiplierDamage;
    }
    public void ApplyRate()
    {
        foreach (Weapon weapon in weapons) { weapon.AttackSpeed *= multiplierRate; }
    }
    public void ApplyRange()
    {
        foreach (Launcher launcher in launchers) launcher.MultiplierRange = multiplierRange;
    }
    public void ApplyKnockback()
    {
        foreach (Melee melee in melees) melee.multiplierKnockback = multiplierKnockback;
    }
}