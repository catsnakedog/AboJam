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
        HandAction += LeftSet;
        HandAction += RightSet;
        HandAction += SetArm;
        SetWeapon();
    }

    private void SetArm()
    {
        if(CheckFlip(weapons[(int)WeaponType].StandardHand == 0))
            Weapon.transform.parent = LeftGunLocation;
        else
            Weapon.transform.parent = RightGunLocation;
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
        public int StandardHand;
        public bool LeftUse;
        public bool RightUse;

        public WeaponHandState(int standardHand, bool leftUse, bool rightUse)
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
            1, // 0 Left 1 Right
            true,
            true
        ),
        new WeaponHandState // ShotGun
        (
            0,
            true,
            true
        ),
        new WeaponHandState // Riple
        (
            0,
            true,
            true
        ),
        new WeaponHandState // Sniper
        (
            0,
            true,
            true
        ),
        new WeaponHandState // Knife
        (
            0,
            true,
            true
        ),
        new WeaponHandState // Spear
        (
            1,
            true,
            false
        ),
        new WeaponHandState // ChainSaw
        (
            1,
            true,
            false
        ),
        new WeaponHandState // Bat
        (
            1,
            true,
            false
        ),
        new WeaponHandState // Default
        (
            1,
            false,
            false
        )
    };

    public void LeftSet()
    {
        var weaponData = weapons[(int)WeaponType];
        if (CheckFlip(weaponData.LeftUse))
        {
            if (CheckFlip(weaponData.StandardHand == 0))
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
            if (weaponData.StandardHand == 1)
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
        if (CheckFlip(weaponData.RightUse))
        {
            if (CheckFlip(weaponData.StandardHand == 1))
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
            if(weaponData.StandardHand == 1)
                RightRenderer.sortingOrder = 3;
            else
                RightRenderer.sortingOrder = 0;
            RightRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    private bool CheckFlip(bool ori)
    {
        if (Player.transform.localRotation.y == 180)
            return !ori;
        else
            return ori;
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. ���콺�� ȭ�� ��ǥ�� ������
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. ���콺�� ���� ��ǥ�� ��� (ī�޶� ���� Z ���� ����)
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));
        mouseWorldPosition.z = 0; // 2D ȯ�濡���� Z�� ����

        // 3. ������Ʈ�� ��ġ���� ���콺 ��ġ�� ���ϴ� ���� ���� ���
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. ���� ���͸� �������� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. ������Ʈ�� ��ü Z�� ���� ȸ�� �߰� (90��)
        float adjustedAngle = angle + 90f;

        // 6. ����/���� �Ǻ��Ͽ� Flip ó��
        Vector3 localScale = obj.transform.localScale;
        if (direction.x > 0) // ���� ����
        {
            localScale.x = -1;
            _extraFlip = true;
        }
        else // ���� ����
        {
            localScale.x = 1;
            _extraFlip = false;
        }
        obj.transform.localScale = localScale;

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
