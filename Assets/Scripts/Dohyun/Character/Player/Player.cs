using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerMovement playerMovement;
    public Hand Hand;
    public Head Head;

    void Awake()
    {
        Movement = playerMovement;
        Hand?.Init();
        Head?.Init();
        Init();
    }

    private void Update()
    {
        playerMovement.MoveAction?.Invoke();
        Hand.HandAction?.Invoke();
        Head.HeadAction?.Invoke();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if ((int)Hand.WeaponType == 0)
                return;
            Hand.WeaponType = (EnumData.Weapon)((int)Hand.WeaponType - 1);
            Hand.SetWeapon();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if ((int)Hand.WeaponType >= 8)
                return;
            Hand.WeaponType = (EnumData.Weapon)((int)Hand.WeaponType + 1);
            Hand.SetWeapon();
        }
    }
}