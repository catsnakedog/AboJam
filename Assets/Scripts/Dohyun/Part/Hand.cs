using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Weapon;

[System.Serializable]
public class Hand
{
    public GameObject LeftArm;
    public GameObject RightArm;
    [HideInInspector]
    public Weapon Weapon;
    public Weapon[] Weapons;

    public Action HandAction;
    public Quaternion LeftRot;
    public Quaternion RightRot;
    public SpriteRenderer LeftRenderer;
    public SpriteRenderer RightRenderer;
    public Transform LeftGunLocation;
    public Transform RightGunLocation;
    public EnumData.Weapon WeaponType = EnumData.Weapon.Default;
    public GameObject Player;
    public EnumData.Weapon Default;

    private bool _extraFlip;

    public void Init()
    {
        HandAction = CheckAttack;
        HandAction += SetFlip;
        HandAction += LeftSet;
        HandAction += RightSet;
        HandAction += SetArm;
        SetWeapon();
    }

    private void SetFlip()
    {
        if (Player.transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            _extraFlip = false;
        else
            _extraFlip = true;
    }

    private void SetArm()
    {
        if(CheckFlip(weapons[(int)WeaponType], 0, _extraFlip) == false)
            Weapon.transform.SetParent(LeftGunLocation, false);
        else
            Weapon.transform.SetParent(RightGunLocation, false);
        LeftArm.transform.rotation = LeftRot;
        RightArm.transform.rotation = RightRot;
    }

    public void SetWeapon()
    {
        if(Weapon != null)
        {
            Weapon.gameObject.SetActive(false);
        }
        WeaponType = Default;
        Weapon = Weapons[(int)Default];
        Weapon.gameObject.SetActive(true);
        Weapon.Init();
    }

    public void CheckAttack()
    {
        if (WeaponType == EnumData.Weapon.Default)
            return;
        if (Input.GetMouseButton(0))
        {
            if(Weapon != null)
                Weapon.Attack();
        }
    }

    [System.Serializable]
    public class WeaponHandState
    {
        public bool StandardHand;
        public bool LeftUse;
        public bool RightUse;

        public WeaponHandState(bool standardHand, bool leftUse, bool rightUse)
        {
            this.StandardHand = standardHand;
            this.LeftUse = leftUse;
            this.RightUse = rightUse;
        }
    }

    public List<WeaponHandState> weapons = new()
    {
        new WeaponHandState // Gun
        (
            true, // false Left true Right
            true,
            true
        ),
        new WeaponHandState // ShotGun
        (
            false,
            true,
            true
        ),
        new WeaponHandState // Riple
        (
            false,
            true,
            true
        ),
        new WeaponHandState // Sniper
        (
            false,
            true,
            true
        ),
        new WeaponHandState // Knife
        (
            false,
            true,
            true
        ),
        new WeaponHandState // Spear
        (
            true,
            true,
            false
        ),
        new WeaponHandState // ChainSaw
        (
            true,
            true,
            false
        ),
        new WeaponHandState // Bat
        (
            true,
            true,
            false
        ),
        new WeaponHandState // Default
        (
            true,
            false,
            false
        )
    };

    public void LeftSet()
    {
        var weaponData = weapons[(int)WeaponType];
        if (CheckFlip(weaponData, 1, _extraFlip))
        {
            if (CheckFlip(weaponData, 0, _extraFlip) == false)
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = 5;
                LeftRot = ForwardToMouse(LeftArm);
            }
            else
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = 3;
                LeftRot = ForwardToObj(LeftArm, Weapon.SecondHandLocation.position);
            }
        }
        else
        {
            LeftRenderer.sortingLayerName = "Entity";
            if (CheckFlip(weaponData, 0, _extraFlip) == false)
                RightRenderer.sortingOrder = 3;
            else
                RightRenderer.sortingOrder = 0;
            LeftRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    public void RightSet()
    {
        var weaponData = weapons[(int)WeaponType];
        if (CheckFlip(weaponData, 2, _extraFlip))
        {
            if (CheckFlip(weaponData, 0, _extraFlip) == true)
            {
                RightRenderer.sortingLayerName = "Entity";
                RightRenderer.sortingOrder = 5;
                RightRot = ForwardToMouse(RightArm);
            }
            else
            {
                RightRenderer.sortingLayerName = "Entity";
                RightRenderer.sortingOrder = 3;
                RightRot = ForwardToObj(RightArm, Weapon.SecondHandLocation.position);
            }
        }
        else
        {
            RightRenderer.sortingLayerName = "Entity";
            if(CheckFlip(weaponData, 0, _extraFlip) == true)
                RightRenderer.sortingOrder = 3;
            else
                RightRenderer.sortingOrder = 0;
            RightRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    private bool CheckFlip(WeaponHandState weaponHandState, int type, bool isFlip)
    {
        if (type == 0)
        {
            if (isFlip)
                return !weaponHandState.StandardHand;
            else
                return weaponHandState.StandardHand;
        }
        else if (type == 1)
        {
            if (isFlip)
                return weaponHandState.RightUse;
            else
                return weaponHandState.LeftUse;
        }
        else
        {
            if (isFlip)
                return weaponHandState.LeftUse;
            else
                return weaponHandState.RightUse;
        }
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. ���콺�� ȭ�� ��ǥ�� ������
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. ���콺�� ���� ��ǥ�� ��� (ī�޶� ���� Z ���� ����)
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // 2D ȯ�濡���� Z�� ����

        // 3. ������Ʈ�� ��ġ���� ���콺 ��ġ�� ���ϴ� ���� ���� ���
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. ���� ���͸� �������� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. ������Ʈ�� ��ü Z�� ���� ȸ�� �߰� (90��)
        float adjustedAngle = angle + 90f;

        // 7. ���� ȸ���� ��ȯ
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }




    public static Quaternion ForwardToObj(GameObject obj1, Vector3 obj2)
    {
        Vector3 direction = Vector3.zero;

        // �� ��° ������Ʈ�� ���ϴ� ���� ���� ���
        direction = obj2 - obj1.transform.position;
        direction.z = 0; // 2D ȯ�濡���� Z�� ����
        if (direction == Vector3.zero) return Quaternion.identity; // ������ ������ �⺻ ȸ�� ��ȯ

        // ���� ���͸� �������� ȸ�� ��� (Z�� ȸ����)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = angle + 90f;

        // 6. ���� ȸ���� ��ȯ
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }
}
