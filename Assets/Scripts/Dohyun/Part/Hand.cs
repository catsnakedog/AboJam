using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public enum WeaponSlot
    {
        FirstRanged = 0,
        SecondRanged = 1,
        FirstMelee = 2,
    }

    // Player���� Update ������ �ѱ�� ���� Action, Player�ʿ��� ��������ش�.
    public Action HandAction;

    // ���� Ű ���� ������, �ӽ÷� �����Ű� Ű ������ ������ ���� �� ������!
    /// <summary>
    /// ���� Ű
    /// </summary>
    public int AttackKeyIndex = 0;
    /// <summary>
    /// ���� ��ü Ű
    /// </summary>
    public KeyCode SlotChangeKey = KeyCode.R;


    // �������� ���� ������, �� ���Կ� ������ ���� ������, � ���⸦ ��� ������
    /// <summary>
    /// ���Ÿ� ����1
    /// </summary>
    public EnumData.Weapon FirstSlot;
    /// <summary>
    /// ���Ÿ� ����2
    /// </summary>
    public EnumData.Weapon SecondSlot;
    /// <summary>
    /// �ٰŸ� ����1
    /// </summary>
    public EnumData.Weapon ThirdSlot;
    /// <summary>
    /// ��� ���� ����
    /// </summary>
    public WeaponSlot CurrentSlotIndex = WeaponSlot.FirstRanged;

    public EnumData.Weapon CurrentSlotWeaponType { get { // ���� ���Ⱑ �������� �˷��ִ� ������Ƽ
            return GetSlotWeaponType(CurrentSlotIndex);
    } }


    /// <summary>
    /// ���� ��ü�� �ʿ��� �ð�
    /// </summary>
    public float WeaponSwitchTime = 2f;

    // ������ ���θ� �Ǻ��Ͽ� �� ��ġ�� �ݴ�� �ؾ��ϴ���
    private bool _changeHand;
    private Camera _mainCamera;
    
    /// <summary>
    /// ����� ���� ����
    /// </summary>
    private Weapon[] _weapons;
    /// <summary>
    /// ���� ��� ���� ����
    /// </summary>
    private Weapon _currentWeapon;

    /// <summary>
    /// ��ȭ�� �����ϰ� ���Ӱ� �����ϱ� ���� ������
    /// </summary>
    private bool _lastWeaponParent;

    // ��ġ ������ �ʿ��� ������Ʈ���� �����͵�
    private GameObject _leftArm;
    private GameObject _rightArm;
    private SpriteRenderer _leftRenderer;
    private SpriteRenderer _rightRenderer;
    private Transform _leftWeaponLocation;
    private Transform _rightWeaponLocation;
    private GameObject _player;
    private GameObject _weaponRoot;
    private List<Transform> _weaponHoldingLocation;
    private Transform _weaponHoldingLocationRoot;
    private GameObject _clothes;

    // Flip �Ǻ��� 1, -1(������) ���� �����Ѵ�
    private int _playerFlip;
    private int _mouseFlip;
    /// <summary>
    /// ���������� flip �Ǿ�����
    /// </summary>
    private int _flip;
    
    // ������ ���ȴ�
    /// <summary>
    /// ���� ��ü ������, ��ü �߿� �ٸ� �ൿ�� ���Ѵ�
    /// </summary>
    private bool _isSwitchWeapon = false;
    /// <summary>
    /// ���� ��ü ���� ��ġ
    /// </summary>
    private Quaternion _switchLeftTargetRot;
    private Quaternion _switchRightTargetRot;
    /// <summary>
    /// ���� ��ü ���� ��ġ
    /// </summary>
    private Quaternion _switchLeftOriRot;
    private Quaternion _switchRightOriRot;
    /// <summary>
    /// ��ü ȣ�� �� ��ġ
    /// </summary>
    private Quaternion _firstLeftTargetRot;
    private Quaternion _firstRightTargetRot;
    /// <summary>
    /// ��ü ���൵
    /// </summary>
    private float _switchProgressTime;


    // Clothes �ݵ� ����
    public float ShakeAmount;
    public float HealShake = 0.5f; // ��鸲 ȸ�� �ð�
    public float MaxShake = 15f; // �ִ� ��鸲 ���� ����


    /// <summary>
    /// ����� ȸ���� ��ü ���� ������ �ȵ� ���� �ѹ� ���� �ǵ帮�� ����
    /// </summary>
    const float HandAngleCorrection = 90;

    public void Init()
    {
        InitObj();

        CurrentSlotIndex = WeaponSlot.FirstRanged;
        SetWeaponData();
        HandAction = CheckAttack; // �����ߴ��� üũ
        HandAction += CheckSlotSwitch; // ������ �ٲ������ üũ
        HandAction += SetClothes;
        HandAction += CheckFlipData; // ������ �ٲ������ üũ �� ���� ����
        HandAction += CheckCurrentSlotWeaponParent; // ������ ��ġ�� �ٲ������ üũ �� ����
        HandAction += SetArmRot; // ���� ������ ����

        SetSlotWeapons(); // ���� ���۽� �ϴ� �⺻������ ����
    }

    /// <summary>
    /// ������Ʈ�� ���� (���� ����) �̸� ������� ������ Player ���� ������Ʈ�� �̸� ���� ����
    /// </summary>
    private void InitObj()
    {
        _mainCamera = Camera.main;
        _player = GameObject.FindWithTag("Player");
        _leftArm = _player.transform.Find("LeftArm").gameObject;
        _rightArm = _player.transform.Find("RightArm").gameObject;
        _leftRenderer = _leftArm.GetComponent<SpriteRenderer>();
        _rightRenderer = _rightArm.GetComponent<SpriteRenderer>();
        _leftWeaponLocation = _leftArm.transform.Find("LeftWeaponLocation");
        _rightWeaponLocation = _rightArm.transform.Find("RightWeaponLocation");
        _clothes = _player.transform.Find("Clothes").gameObject;
        _weaponRoot = _player.transform.Find("WeaponRoot").gameObject;
        _weaponHoldingLocationRoot = _player.transform.Find("WeaponHoldingLocationRoot");
        _weaponHoldingLocation = new();
        for (int i = 0; i < _weaponHoldingLocationRoot.childCount; i++)
        {
            _weaponHoldingLocation.Add(_weaponHoldingLocationRoot.GetChild(i)); // �������� �������ϱ� ������� �־�!!
        }
    }


    /// <summary>
    /// _weaponRoot�� �����ϴ� ��� Weapon�� ã�Ƽ� _weapons�� �־��ش�
    /// </summary>
    private void SetWeaponData()
    {
        var count = _weaponRoot.transform.childCount;
        _weapons = new Weapon[count];
        for (int i = 0; i < _weaponRoot.transform.childCount; i++)
        {
            _weapons[i] = _weaponRoot.transform.GetChild(i).GetComponent<Weapon>();
        }
    }

    /// <summary>
    /// ������ ������ �����ϰ� ���� ���⽽�� ������ ������ ��� �Ǿ� �ִ� ������ ���� �����ϰ� �ʱ�ȭ�Ѵ�
    /// ���� ���⸦ ��ü ���̿��ٸ� ���� �ʱ�ȭ ��Ű�� �����Ѵ�.
    /// </summary>
    private void SetSlotWeapons()
    {
        // ���� ���� �ʱ�ȭ

        int[] holdWeapons = new int[3] { (int)FirstSlot, (int)SecondSlot, (int)FirstSlot };

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (holdWeapons.Contains(i)) // ���� ��Ͽ� �ִٸ�
            {
                if (!_weapons[i].gameObject.activeSelf)
                {
                    _weapons[i].gameObject.SetActive(true);
                }
                if (holdWeapons[(int)CurrentSlotIndex] == i) // �տ� ��� �ִ� ������ �տ� ������Ų��
                {
                    _currentWeapon = _weapons[i]; // ���� ���� ���� ����� ǥ���Ѵ�.
                    _currentWeapon.Renderer.sortingOrder = 4;
                    SetCurrentSlotWeaponParent(); // ���� ���� ���⸦ ���� �տ� ����
                }
                else // �տ� ��� �ִ� ���Ⱑ �ƴ϶�� Ȧ�� ��ġ�� �̵���Ų��
                {
                    SetHoldSlotWeaponParent(_weapons[i]);
                }
            }
            else // ���� ��Ͽ� ���ٸ� WeaponRoot�� �̵���Ŵ. ���� �̹� WeaponRoot��� ��ŵ
            // ����� �������� Ȱ��ȭ �Ǿ��ִ� �ѵ��� �����ϱ� ���� activeSelf�� ó��
            {
                if (_weapons[i].gameObject.activeSelf)
                {
                    ReturnWeaponToRoot(_weapons[i]);
                }
            }
        }
    }

    public void SetClothes()
    {
        if(ShakeAmount > 0)
        {
            ShakeAmount -= HealShake * Time.deltaTime;
            if(ShakeAmount < 0 )
                ShakeAmount = 0;
        }
        else
        {
            ShakeAmount += HealShake * Time.deltaTime;
            if (ShakeAmount > 0)
                ShakeAmount = 0;
        }
           
        _clothes.transform.localRotation = Quaternion.Euler(0f, 0f, ShakeAmount);
    }

    public void ShakeClothes(float shakeRange)
    {
        if (_flip == 1)
            ShakeAmount += UnityEngine.Random.Range(0, shakeRange);
        else
            ShakeAmount -= UnityEngine.Random.Range(0, shakeRange);

        ShakeAmount = Mathf.Clamp(ShakeAmount, -MaxShake, MaxShake);
    }


    private int _lastPlayerFlip = 1;
    /// <summary>
    /// �����÷��̾� ��Ȳ�� ������ ��Ȳ���� ����ϰ� �����Ų��
    /// </summary>
    private void CheckFlipData()
    {
        // �÷��̾� Y�� ���� (1 : �⺻ ����, -1 : ������ ����)
        _playerFlip = _player.transform.localEulerAngles.y == 0 ? 1 : -1;
        if (_lastPlayerFlip != _playerFlip)
            ShakeAmount = -ShakeAmount;
        _lastPlayerFlip = _playerFlip;
        // ���콺�� �÷��̾� ��� ���� (1 : ����, -1 : ������)
        _mouseFlip = _player.transform.position.x > ScreenToWorld2D(Input.mousePosition).x ? 1 : -1;
        // 3. ���� ���� ���� (1: �⺻, -1: ������)
        _flip = _playerFlip * _mouseFlip;
        SetFlip();
    }

    /// <summary>
    /// ����� FipData�� ������� �÷��̾��� ��, ������ Scale�� �����ϰ� �� ��ü ���θ� �����Ѵ�
    /// </summary>
    private void SetFlip()
    {
        // �Ȱ� ������ ũ�� ����
        // �⺻������ �����տ� ���� �ö󰣰�� �ø��Ǿ���Ѵ�. ���� flip, -flip���� ����Ѵ�.
        // ������ ������ ��� �ݴ�� �ٲ�⿡ ���� ������ �������� �������ش�.
        _leftArm.transform.localScale = new Vector3(_flip, 1, 1);
        _rightArm.transform.localScale = new Vector3(-_flip, 1, 1);

        // 360���� ȸ������ ������ ��Ÿ���⿡ �������� ���°�� (������ ����) y�� �ø�������Ѵ�.
        // ������ �� ���� �÷��̾ �������� �ݴ�� �ٲ�⿡ �÷��̾� ������ �������� �������ش�.
        if (_currentWeapon.HandState.StandardHand) // �����տ� ��������
            _currentWeapon.transform.localScale = new Vector3(1, -_mouseFlip, 1);
        else
            _currentWeapon.transform.localScale = new Vector3(1, _mouseFlip, 1);

        // 5. _changeHand �÷��� ����
        _changeHand = (_flip == -1);
    }

    /// <summary>
    /// ������ HandState�� ���� �׿� ���缭 ���� �����Ѵ�
    /// </summary>
    /// <param name="handType">��� ���� ������ ������</param>
    private void SetArm(HandType handType)
    {
        bool isLeft = handType == HandType.Left;
        var arm = isLeft ? _leftArm : _rightArm;
        var renderer = isLeft ? _leftRenderer : _rightRenderer;
        Quaternion rot;
        var weaponData = _currentWeapon.HandState;

        if (ApplyHandSwap(weaponData, handType, _changeHand))
        {
            if (!IsRightHandStandard() == isLeft)
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.afterGun;
                rot = ForwardToMouse(arm);
            }
            else
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.beforeGun;
                rot = ForwardToObj(arm, _currentWeapon.SecondHandLocation.position);
            }
        }
        else
        {
            renderer.sortingLayerName = "Entity";
            renderer.sortingOrder = (int)HandLayer.afterBody;
            rot = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        arm.transform.rotation = rot;
    }


    /// <summary>
    /// �� �� ���� �����Ѵ�
    /// </summary>
    private void SetArmRot()
    {
        SetArm(HandType.Left);
        SetArm(HandType.Right);
    }

    /// <summary>
    /// Slot�� �����ϴ����� üũ�Ѵ�
    /// </summary>
    public void CheckSlotSwitch() // _crruentWeapon�� �ᱹ ����Ǹ� ��
    {
        if (Input.GetKeyDown(SlotChangeKey))
        {
            if (CurrentSlotIndex == WeaponSlot.FirstMelee)
                CurrentSlotIndex = WeaponSlot.FirstRanged;
            else
                CurrentSlotIndex++;
            if(_isSwitchWeapon)
            {
            }
            else
            {
                SwitchWeapon();
            }
        }
    }

    private void SwitchWeapon()
    {
        _isSwitchWeapon = true;
        HandAction -= CheckFlipData;
        HandAction -= CheckCurrentSlotWeaponParent;
        HandAction -= SetArmRot; // ��ü �߿� �ϴ� ��ž
        
        _switchLeftTargetRot = ForwardToObj(_leftArm, _weaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
        _switchRightTargetRot = ForwardToObj(_rightArm, _weaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
        _firstLeftTargetRot = _leftArm.transform.rotation;
        _firstRightTargetRot = _rightArm.transform.rotation;
        _switchLeftOriRot = _leftArm.transform.rotation;
        _switchRightOriRot = _rightArm.transform.rotation;
        _switchProgressTime = 0;

        HandAction += HoldStandardWeapon;
    }

    private void HoldStandardWeapon() // ���⸦ Ȧ���� �ִ� ����, ����Ǹ� ������ �������� !�ֿ켱!
    {
        _switchProgressTime += Time.deltaTime; // �̵��� �ð� ��ʷ� �̵�
        _leftArm.transform.rotation = Quaternion.Slerp(_switchLeftOriRot, _switchLeftTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));
        _rightArm.transform.rotation = Quaternion.Slerp(_switchRightOriRot, _switchRightTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));
        if (_switchProgressTime >= (WeaponSwitchTime - 0.4f) / 2f) // �̵� �Ϸ�
        {
            _leftArm.transform.rotation = _switchLeftTargetRot;
            _rightArm.transform.rotation = _switchRightTargetRot;

            _switchProgressTime = 0;

            HandAction -= HoldStandardWeapon;
            HandAction += WaitHold;
        }
    }

    private void WaitHold()
    {
        _switchProgressTime += Time.deltaTime;
        if (_switchProgressTime >= 0.2f) // �̵� �Ϸ�
        {
            _currentWeapon.transform.SetParent(_weaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
            _currentWeapon.Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;

            _currentWeapon = _weapons[(int)CurrentSlotWeaponType];

            _switchLeftTargetRot = ForwardToObj(_leftArm, _weaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
            _switchRightTargetRot = ForwardToObj(_rightArm, _weaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
            _switchLeftOriRot = _leftArm.transform.rotation;
            _switchRightOriRot = _rightArm.transform.rotation;

            _switchProgressTime = 0;

            HandAction -= WaitHold;
            HandAction += DrawStandardWeapon;
        }
    }

    private void DrawStandardWeapon() // ���⸦ ���� ���¿��� ���õ� ���⸦ ������ ����
    {
        _switchProgressTime += Time.deltaTime; // �̵��� �ð� ��ʷ� �̵�
        _leftArm.transform.rotation = Quaternion.Slerp(_switchLeftOriRot, _switchLeftTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));
        _rightArm.transform.rotation = Quaternion.Slerp(_switchRightOriRot, _switchRightTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));

        if (_switchProgressTime >= (WeaponSwitchTime-0.4f) /4f) // �̵� �Ϸ�
        {
            _leftArm.transform.rotation = _switchLeftTargetRot;
            _rightArm.transform.rotation = _switchRightTargetRot;
            _switchLeftOriRot = _leftArm.transform.rotation;
            _switchRightOriRot = _rightArm.transform.rotation;
            SetCurrentSlotWeaponParent();
            _currentWeapon.Renderer.sortingOrder = 4;
            _switchProgressTime = 0;

            HandAction -= DrawStandardWeapon; // �ֹ������ ����
            HandAction += WaitDraw;
        }
    }

    private void WaitDraw()
    {
        _switchProgressTime += Time.deltaTime;
        if (_switchProgressTime >= 0.2f) // �̵� �Ϸ�
        {
            _switchProgressTime = 0;

            HandAction -= WaitDraw;
            HandAction += ReadyStandardWeapon;
        }
    }

    private void ReadyStandardWeapon() // ���⸦ ���� ���¿��� ���õ� ���⸦ ������ ����
    {
        _switchProgressTime += Time.deltaTime; // �̵��� �ð� ��ʷ� �̵�
        _leftArm.transform.rotation = Quaternion.Slerp(_switchLeftOriRot, _firstLeftTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));
        _rightArm.transform.rotation = Quaternion.Slerp(_switchRightOriRot, _firstLeftTargetRot, _switchProgressTime / (WeaponSwitchTime / 2f));

        if (_switchProgressTime >= (WeaponSwitchTime - 0.4f) / 4f) // �̵� �Ϸ�
        {
            _leftArm.transform.rotation = _firstLeftTargetRot;
            _rightArm.transform.rotation = _firstRightTargetRot;


            HandAction -= ReadyStandardWeapon;
            HandAction += CheckFlipData;
            HandAction += CheckCurrentSlotWeaponParent;
            HandAction += SetArmRot;
            _isSwitchWeapon = false;
        }
    }

    public void CheckAttack()
    {
        if (CurrentSlotWeaponType == EnumData.Weapon.None)
            return;
        if (Input.GetMouseButton(AttackKeyIndex))
        {
            if (!_currentWeapon.IsReload)
                ShakeClothes(_currentWeapon.ClothesShake);
            if (_currentWeapon != null)
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

    private void CheckCurrentSlotWeaponParent()
    {
        if (_lastWeaponParent != IsRightHandStandard())
            SetCurrentSlotWeaponParent();
    }

    private void SetCurrentSlotWeaponParent()
    {
        if (IsRightHandStandard())
        {
            _currentWeapon.transform.SetParent(_rightWeaponLocation, false);
            _lastWeaponParent = true;
        }
        else
        {
            _currentWeapon.transform.SetParent(_leftWeaponLocation, false);
            _lastWeaponParent = false;
        }
        _currentWeapon.transform.localPosition = Vector3.zero;
    }

    private void SetHoldSlotWeaponParent(WeaponSlot targetSlot)
    {
        var weapon = _weapons[(int)GetSlotWeaponType(targetSlot)];
        weapon.transform.SetParent(_weaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
        weapon.Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;
    }

    private void SetHoldSlotWeaponParent(Weapon targetWeapon)
    {
        targetWeapon.transform.SetParent(_weaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
        targetWeapon.Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;
    }

    private void ReturnWeaponToRoot(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        weapon.transform.SetParent(_weaponRoot.transform, false);
    }

    private bool IsRightHandStandard()
    {
        return ApplyHandSwap(_currentWeapon.HandState, HandType.Standard, _changeHand);
    }

    private EnumData.Weapon GetSlotWeaponType(WeaponSlot slot)
    {
        return slot switch
        {
            WeaponSlot.FirstRanged => FirstSlot,
            WeaponSlot.SecondRanged => SecondSlot,
            WeaponSlot.FirstMelee => ThirdSlot,
            _ => throw new NotImplementedException()
        };
    }

    public Quaternion ForwardToMouse(GameObject obj)
    {
        // 1. ���콺�� ȭ�� ��ǥ�� ������
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. ���콺�� ���� ��ǥ�� ��� (ī�޶� ���� Z ���� ����)
        Vector3 mouseWorldPosition = ScreenToWorld2D(mouseScreenPosition);

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

    Vector3 ScreenToWorld2D(Vector3 screenPosition)
    {
        // 1. ȭ�� ��ǥ�� 0~1�� ����ȭ
        float normalizedX = screenPosition.x / Screen.width;
        float normalizedY = screenPosition.y / Screen.height;

        // 2. ī�޶��� ���� ��ǥ ���� ���
        float halfHeight = _mainCamera.orthographicSize; // ī�޶��� ���� ���� ũ��
        float halfWidth = halfHeight * _mainCamera.aspect; // ī�޶��� ���� ���� ũ�� (aspect ���� ���)

        // 3. ���� ��ǥ ���
        float worldX = Mathf.Lerp(_mainCamera.transform.position.x - halfWidth,
                                  _mainCamera.transform.position.x + halfWidth,
                                  normalizedX);

        float worldY = Mathf.Lerp(_mainCamera.transform.position.y - halfHeight,
                                  _mainCamera.transform.position.y + halfHeight,
                                  normalizedY);

        // 4. Z�� ���̴� 0
        float worldZ = 0f;

        return new Vector3(worldX, worldY, worldZ);
    }
}