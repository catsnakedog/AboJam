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
        // �÷��̾� Y�� ���� (1 : �⺻ ����, -1 : ������ ����)
        _playerFlip = Player.transform.localEulerAngles.y == 0 ? 1 : -1;
        // ���콺�� �÷��̾� ��� ���� (1 : ����, -1 : ������)
        _mouseFlip = Player.transform.position.x > _mainCamera.ScreenToWorldPoint(Input.mousePosition).x ? 1 : -1;
        // 3. ���� ���� ���� (1: �⺻, -1: ������)
        _flip = _playerFlip * _mouseFlip;
    }

    private void SetFlip()
    {
        // 4. �Ȱ� ������ ũ�� ����
        // �⺻������ �����տ� ���� �ö󰣰�� �ø��Ǿ���Ѵ�. ���� flip, -flip���� ����Ѵ�.
        // ������ ������ ��� �ݴ�� �ٲ�⿡ ���� ������ �������� �������ش�.
        LeftArm.transform.localScale = new Vector3(_flip, 1, 1);
        RightArm.transform.localScale = new Vector3(-_flip, 1, 1);

        // 360���� ȸ������ ������ ��Ÿ���⿡ �������� ���°�� (������ ����) y�� �ø�������Ѵ�.
        // ������ �� ���� �÷��̾ �������� �ݴ�� �ٲ�⿡ �÷��̾� ������ �������� �������ش�.
        if (_currentWeapon.HandState.StandardHand) // �����տ� ��������
            _currentWeapon.transform.localScale = new Vector3(1, -_mouseFlip, 1);
        else
            _currentWeapon.transform.localScale = new Vector3(1, _mouseFlip, 1);

        // 5. _changeHand �÷��� ����
        _changeHand = (_flip == -1);
    }

    public void LeftSet() // ����ϴ� ���, ��/�� Ȯ��
    {
        var weaponData = _currentWeapon.HandState;
        if (ApplyHandSwap(weaponData, HandType.Left, _changeHand))
        {
            if (ApplyHandSwap(weaponData, HandType.Standard, _changeHand) == false) // ��
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = (int)HandLayer.afterGun;
                _leftRot = ForwardToMouse(LeftArm);
            }
            else // ��
            {
                LeftRenderer.sortingLayerName = "Entity";
                LeftRenderer.sortingOrder = (int)HandLayer.beforeGun;
                _leftRot = ForwardToObj(LeftArm, _currentWeapon.SecondHandLocation.position);
            }
        }
        else // �����ϴ� ��� �⺻ ����
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
        // 1. ���콺�� ȭ�� ��ǥ�� ������
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. ���콺�� ���� ��ǥ�� ��� (ī�޶� ���� Z ���� ����)
        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0; // 2D ȯ�濡���� Z�� ����

        // 3. ������Ʈ�� ��ġ���� ���콺 ��ġ�� ���ϴ� ���� ���� ���
        Vector3 direction = mouseWorldPosition - obj.transform.position;

        // 4. ���� ���͸� �������� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. ������Ʈ�� ��ü Z�� ���� ȸ�� �߰� (90��)
        float adjustedAngle = angle + HandAngleCorrection;

        // 7. ���� ȸ���� ��ȯ
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }




    public static Quaternion ForwardToObj(GameObject obj1, Vector3 obj2)
    {
        // �� ��° ������Ʈ�� ���ϴ� ���� ���� ���
        Vector3 direction = obj2 - obj1.transform.position;
        direction.z = 0; // 2D ȯ�濡���� Z�� ����
        if (direction == Vector3.zero) return Quaternion.identity; // ������ ������ �⺻ ȸ�� ��ȯ

        // ���� ���͸� �������� ȸ�� ��� (Z�� ȸ����)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float adjustedAngle = angle + HandAngleCorrection;

        // 6. ���� ȸ���� ��ȯ
        return Quaternion.Euler(new Vector3(0, 0, adjustedAngle));
    }
}