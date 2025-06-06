using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using UnityEngine;

public class Swap : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private Player player;
    EnumData.Weapon[] ranges = { EnumData.Weapon.Gun, EnumData.Weapon.ShotGun, EnumData.Weapon.Riple, EnumData.Weapon.Sniper };
    EnumData.Weapon[] melees = { EnumData.Weapon.Bat, EnumData.Weapon.Knife, EnumData.Weapon.Spear, EnumData.Weapon.ChainSaw };

    /* Field & Property */
    public static Swap instance;

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        SetSlot(EnumData.Weapon.Gun);
        SetSlot(EnumData.Weapon.Knife);
    }

    /* Public Method */
    /// <summary>
    /// <br>슬롯에 무기를 장착시킵니다.</br>
    /// <br>원거리 무기는 1 번 슬롯에 장착됩니다.</br>
    /// <br>근접 무기는 2 번 슬롯에 장착됩니다.</br>
    /// </summary>
    public void SetSlot(EnumData.Weapon weapon)
    {
        if (ranges.Contains(weapon) && player.Hand.FirstSlot != weapon)
        {
            player.Hand.FirstSlot = weapon;
            player.Hand.SetSlotWeapons();
        }
        if (melees.Contains(weapon) && player.Hand.SecondSlot != weapon)
        {
            player.Hand.SecondSlot = weapon;
            player.Hand.SetSlotWeapons();
        }
    }
    /// <summary>
    /// 근거리 / 원거리 무기를 교체합니다.
    /// </summary>
    public void SwapSlot(bool force = false)
    {
        if(force == false) if (player.Hand.IsSwitchWeapon) return; // 교체 중에는 리턴
        if (player.Hand.CurrentSlotIndex == Hand.WeaponSlot.FirstRanged) player.Hand.CurrentSlotIndex = Hand.WeaponSlot.SecondRanged;
        else if (player.Hand.CurrentSlotIndex == Hand.WeaponSlot.SecondRanged) player.Hand.CurrentSlotIndex = Hand.WeaponSlot.FirstRanged;

        player.Hand.SwitchWeapon();
    }
    /// <summary>
    /// 지정한 무기로 스왑 후 장착합니다.
    /// </summary>
    public void Equip(EnumData.Weapon weapon)
    {
        SetSlot(weapon);

        if (ranges.Contains(weapon)) { if (player.Hand.CurrentSlotIndex != Hand.WeaponSlot.FirstRanged) SwapSlot(true); }
        else { if (player.Hand.CurrentSlotIndex != Hand.WeaponSlot.SecondRanged) SwapSlot(true); }
    }
}
