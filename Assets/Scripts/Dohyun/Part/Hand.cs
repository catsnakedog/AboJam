using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static EnumData;
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

    public enum HandLayer
    {
        beforeBody = -2,
        InClothes = 0,
        afterBody = 2,
        beforeGun = 3,
        afterGun = 5,
    }

    public enum UsingWeapon
    {
        FirstRanged = 0,
        SecondRanged = 1,
        FirstMelee = 2,
    }


    public GameObject LeftArm;
    public GameObject RightArm;

    public Action HandAction;
    public SpriteRenderer LeftRenderer;
    public SpriteRenderer RightRenderer;
    public Transform LeftGunLocation;
    public Transform RightGunLocation;
    public EnumData.Weapon CurrentWeaponType = EnumData.Weapon.None;
    public GameObject Player;
    public GameObject WeaponRoot;
    public int AttackKeyIndex = 0;
    public KeyCode WeaponChangeKey = KeyCode.R;
    public List<Transform> WeaponHoldingLocation;
    public EnumData.Weapon FirstRangedWeapon;
    public EnumData.Weapon SecondRangedWeapon;
    public EnumData.Weapon FirstMeleeWeapon;
    public UsingWeapon UsingWeaponIndex;

    private bool _changeHand;
    private Quaternion _leftRot;
    private Quaternion _rightRot;
    private Camera _mainCamera;
    private Weapon[] _weapons;
    private Weapon _currentWeapon;

    private bool _lastStandardHand;
    private bool _lastChangeHand;

    private int _playerFlip;
    private int _mouseFlip;
    private int _flip;

    const float HandAngleCorrection = 90;

    public void Init()
    {
        _mainCamera = Camera.main;
        UsingWeaponIndex = UsingWeapon.FirstRanged;
        SetWeaponData();
        HandAction = CheckAttack;
        HandAction += CheckUsingWeaponChange;
        HandAction += SetFlipData;
        HandAction += SetFlip;
        HandAction += LeftSet;
        HandAction += RightSet;
        HandAction += SetArm;
        SetHoldWeapons();
    }

    private void SetWeaponData()
    {
        var count = WeaponRoot.transform.childCount;
        _weapons = new Weapon[count];
        for (int i = 0; i < WeaponRoot.transform.childCount; i++)
        {
            _weapons[i] = WeaponRoot.transform.GetChild(i).GetComponent<Weapon>();
        }
    }

    private void SetHoldWeapons()
    {
        HoldWeapon(FirstRangedWeapon, UsingWeapon.FirstRanged);
        HoldWeapon(SecondRangedWeapon, UsingWeapon.SecondRanged);
        HoldWeapon(FirstMeleeWeapon, UsingWeapon.FirstMelee);
    }

    private void HoldWeapon(EnumData.Weapon weaponType, UsingWeapon index)
    {
        if (weaponType == EnumData.Weapon.None)
            return;
        if (index == UsingWeaponIndex)
        {
            _lastChangeHand = !_lastChangeHand;
            _lastStandardHand = !_lastStandardHand;
            CurrentWeaponType = weaponType;
            SetWeapon();
        }
        else
        {
            Weapon weapon = _weapons[(int)weaponType];
            weapon.transform.SetParent(WeaponHoldingLocation[(int)weapon.HoldingIndex], false);
            weapon.Renderer.sortingOrder = (int)weapon.HoldingOrder;
            weapon.gameObject.SetActive(true);
        }
    }


    private void SetFlipData()
    {
        // 플레이어 Y축 방향 (1 : 기본 방향, -1 : 뒤집힌 방향)
        _playerFlip = Player.transform.localEulerAngles.y == 0 ? 1 : -1;
        // 마우스가 플레이어 상대 방향 (1 : 왼쪽, -1 : 오른쪽)
        _mouseFlip = Player.transform.position.x > _mainCamera.ScreenToWorldPoint(Input.mousePosition).x ? 1 : -1;
        // 3. 최종 방향 결정 (1: 기본, -1: 뒤집힘)
        _flip = _playerFlip * _mouseFlip;
    }

    private void SetFlip()
    {
        // 4. 팔과 무기의 크기 설정
        // 기본적으로 오른손에 총이 올라간경우 플립되어야한다. 따라서 flip, -flip으로 계산한다.
        // 하지만 뒤집힌 경우 반대로 바뀌기에 최정 방향을 기준으로 보정해준다.
        LeftArm.transform.localScale = new Vector3(_flip, 1, 1);
        RightArm.transform.localScale = new Vector3(-_flip, 1, 1);

        // 360도로 회전시켜 방향을 나타내기에 우측으로 가는경우 (정방향 기준) y축 플립해줘야한다.
        // 하지만 이 또한 플레이어가 뒤집히면 반대로 바뀌기에 플레이어 방향을 기준으로 보정해준다.
        if (_currentWeapon.HandState.StandardHand) // 오른손에 있을때만
            _currentWeapon.transform.localScale = new Vector3(1, -_mouseFlip, 1);
        else
            _currentWeapon.transform.localScale = new Vector3(1, _mouseFlip, 1);

        // 5. _changeHand 플래그 설정
        _changeHand = (_flip == -1);
    }

    public void LeftSet() // 사용하는 경우, 주/부 확인
    {
        var weaponData = _currentWeapon.HandState;
        if (ApplyHandSwap(weaponData, HandType.Left, _changeHand))
        {
            if (ApplyHandSwap(weaponData, HandType.Standard, _changeHand) == false) // 주
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = (int)HandLayer.afterGun;
                _leftRot = ForwardToMouse(LeftArm);
            }
            else // 부
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = (int)HandLayer.beforeGun;
                _leftRot = ForwardToObj(LeftArm, _currentWeapon.SecondHandLocation.position);
            }
        }
        else // 사용안하는 경우 기본 세팅
        {
            LeftRenderer.sortingLayerName = "Entity";
            RightRenderer.sortingOrder = (int)HandLayer.afterBody;
            _leftRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    public void RightSet()
    {
        var weaponData = _currentWeapon.HandState;
        if (ApplyHandSwap(weaponData, HandType.Right, _changeHand))
        {
            if (ApplyHandSwap(weaponData, HandType.Standard, _changeHand) == true)
            {
                RightRenderer.sortingLayerName = "Entity";
                RightRenderer.sortingOrder = (int)HandLayer.afterGun;
                _rightRot = ForwardToMouse(RightArm);
            }
            else
            {
                RightRenderer.sortingLayerName = "Entity";
                RightRenderer.sortingOrder = (int)HandLayer.afterBody;
                _rightRot = ForwardToObj(RightArm, _currentWeapon.SecondHandLocation.position);
            }
        }
        else
        {
            RightRenderer.sortingLayerName = "Entity";
            RightRenderer.sortingOrder = (int)HandLayer.afterBody;
            _rightRot = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }
    }

    private void SetArm()
    {
        if(_currentWeapon.HandState.StandardHand != _lastStandardHand)
        {
            SetWeaponParent();
            _lastStandardHand = _currentWeapon.HandState.StandardHand;
        }
        if(_lastChangeHand != _changeHand)
        {
            SetWeaponParent();
            _lastChangeHand = _changeHand;
        }


        LeftArm.transform.rotation = _leftRot;
        RightArm.transform.rotation = _rightRot;
    }

    public void CheckUsingWeaponChange()
    {
        if (Input.GetKeyDown(WeaponChangeKey))
        {
            if (UsingWeaponIndex == UsingWeapon.FirstMelee)
                UsingWeaponIndex = UsingWeapon.FirstRanged;
            else
                UsingWeaponIndex++;
            SetHoldWeapons();
        }
    }

    public void SetWeapon()
    {
        if(_currentWeapon != null)
        {
            if(_currentWeapon.WeaponType == FirstRangedWeapon || _currentWeapon.WeaponType == SecondRangedWeapon || _currentWeapon.WeaponType == FirstMeleeWeapon)
            {

            }
            else
            {
                _currentWeapon.gameObject.SetActive(false);
                _currentWeapon.transform.SetParent(WeaponRoot.transform, false);
            }
        }
        _currentWeapon = _weapons[(int)CurrentWeaponType];
        _currentWeapon.Renderer.sortingOrder = 4;
        _lastStandardHand = _currentWeapon.HandState.StandardHand;

        SetWeaponParent();

        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.gameObject.SetActive(true);
        _currentWeapon.Init();
    }

    public void CheckAttack()
    {
        if (CurrentWeaponType == EnumData.Weapon.None)
            return;
        if (Input.GetMouseButton(AttackKeyIndex))
        {
            if(_currentWeapon != null)
                _currentWeapon.Attack();
        }
    }

    private bool ApplyHandSwap(WeaponHandState weaponHandState, HandType type, bool isFlip)
    {
        return type switch
        {
            HandType.Standard => isFlip ? !weaponHandState.StandardHand : weaponHandState.StandardHand,
            HandType.Left => isFlip ? weaponHandState.RightUse : weaponHandState.LeftUse,
            _ => isFlip ? weaponHandState.LeftUse : weaponHandState.RightUse,
        };
    }

    private void SetWeaponParent()
    {
        if (ApplyHandSwap(_currentWeapon.HandState, HandType.Standard, _changeHand))
            _currentWeapon.transform.SetParent(RightGunLocation, false);
        else
            _currentWeapon.transform.SetParent(LeftGunLocation, false);
        _currentWeapon.transform.localPosition = Vector3.zero;
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // 2D 환경에서는 Z축 고정

        // 3. 오브젝트의 위치에서 마우스 위치로 향하는 방향 벡터 계산
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. 방향 벡터를 기준으로 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. 오브젝트의 자체 Z축 기준 회전 추가 (90도)
        float adjustedAngle = angle + HandAngleCorrection;

        // 7. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }




    public static Quaternion ForwardToObj(GameObject obj1, Vector3 obj2)
    {
        // 두 번째 오브젝트를 향하는 방향 벡터 계산
        Vector3 direction = obj2 - obj1.transform.position;
        direction.z = 0; // 2D 환경에서는 Z축 무시
        if (direction == Vector3.zero) return Quaternion.identity; // 방향이 없으면 기본 회전 반환

        // 방향 벡터를 기준으로 회전 계산 (Z축 회전만)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = angle + HandAngleCorrection;

        // 6. 최종 회전값 반환
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }
}