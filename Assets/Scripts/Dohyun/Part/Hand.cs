using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Weapon;

[System.Serializable]
public class Hand
{
    public enum HandType
    {
        Standard = 0,
        Left = 1,
        Right = 2,
    }

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

    private bool _changeHand;

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
        {
            if (Player.transform.localEulerAngles.y != 0)
            {
                LeftArm.transform.localScale = new Vector3(-1, 1, 1);
                RightArm.transform.localScale = new Vector3(1, 1, 1);
                if(weapons[(int)WeaponType].StandardHand)
                    Weapon.transform.localScale = new Vector3(1, -1, 1);
                else
                    Weapon.transform.localScale = new Vector3(1, 1, 1);
                _changeHand = true;
            }
            else
            {
                LeftArm.transform.localScale = new Vector3(1, 1, 1);
                RightArm.transform.localScale = new Vector3(-1, 1, 1);
                if (weapons[(int)WeaponType].StandardHand)
                    Weapon.transform.localScale = new Vector3(1, -1, 1);
                else
                    Weapon.transform.localScale = new Vector3(1, 1, 1);
                _changeHand = false;
            }
        }
        else
        {
            if (Player.transform.localEulerAngles.y != 0)
            {
                LeftArm.transform.localScale = new Vector3(1, 1, 1);
                RightArm.transform.localScale = new Vector3(-1, 1, 1);
                if (weapons[(int)WeaponType].StandardHand)
                    Weapon.transform.localScale = new Vector3(1, 1, 1);
                else
                    Weapon.transform.localScale = new Vector3(1, -1, 1);
                _changeHand = false;
            }

            else
            {
                LeftArm.transform.localScale = new Vector3(-1, 1, 1);
                RightArm.transform.localScale = new Vector3(1, 1, 1);
                if (weapons[(int)WeaponType].StandardHand)
                    Weapon.transform.localScale = new Vector3(1, 1, 1);
                else
                    Weapon.transform.localScale = new Vector3(1, -1, 1);
                _changeHand = true;
            }
        }
    }

    private void SetArm()
    {
        if(CheckFlip(weapons[(int)WeaponType], HandType.Standard, _changeHand) == false)
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
        Weapon = Weapons[(int)WeaponType];
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

    public void LeftSet()
    {
        var weaponData = weapons[(int)WeaponType];
        if (CheckFlip(weaponData, HandType.Left, _changeHand))
        {
            if (CheckFlip(weaponData, HandType.Standard, _changeHand) == false)
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
            if (CheckFlip(weaponData, HandType.Standard, _changeHand) == false)
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
        if (CheckFlip(weaponData, HandType.Right, _changeHand))
        {
            if (CheckFlip(weaponData, HandType.Standard, _changeHand) == true)
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
            if(CheckFlip(weaponData, HandType.Standard, _changeHand) == true)
                RightRenderer.sortingOrder = 3;
            else
                RightRenderer.sortingOrder = 0;
            RightRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    private bool CheckFlip(WeaponHandState weaponHandState, HandType type, bool isFlip)
    {
        return type switch
        {
            HandType.Standard => isFlip ? !weaponHandState.StandardHand : weaponHandState.StandardHand,
            HandType.Left => isFlip ? weaponHandState.RightUse : weaponHandState.LeftUse,
            _ => isFlip ? weaponHandState.LeftUse : weaponHandState.RightUse,
        };
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // 2D 환경에서는 Z축 고정

        // 3. 오브젝트의 위치에서 마우스 위치로 향하는 방향 벡터 계산
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. 방향 벡터를 기준으로 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. 오브젝트의 자체 Z축 기준 회전 추가 (90도)
        float adjustedAngle = angle + 90f;

        // 7. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }




    public static Quaternion ForwardToObj(GameObject obj1, Vector3 obj2)
    {
        Vector3 direction = Vector3.zero;

        // 두 번째 오브젝트를 향하는 방향 벡터 계산
        direction = obj2 - obj1.transform.position;
        direction.z = 0; // 2D 환경에서는 Z축 무시
        if (direction == Vector3.zero) return Quaternion.identity; // 방향이 없으면 기본 회전 반환

        // 방향 벡터를 기준으로 회전 계산 (Z축 회전만)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = angle + 90f;

        // 6. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
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
}


