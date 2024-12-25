using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Player���� Update ������ �ѱ�� ���� Action, Player�ʿ��� ��������ش�.
    public Action HandAction;

    // ���� Ű ���� ������, �ӽ÷� �����Ű� Ű ������ ������ ���� �� ������!
    public int AttackKeyIndex = 0;
    public KeyCode WeaponChangeKey = KeyCode.R;


    // �������� ���� ������, �� ���Կ� ������ ���� ������, � ���⸦ ��� ������
    public EnumData.Weapon FirstRangedWeapon;
    public EnumData.Weapon SecondRangedWeapon;
    public EnumData.Weapon FirstMeleeWeapon;
    public UsingWeapon UsingWeaponIndex;

    public EnumData.Weapon CurrentWeaponType { get { // ���� ���Ⱑ �������� �˷��ִ� ������Ƽ
            return UsingWeaponIndex switch
            {
                UsingWeapon.FirstRanged => FirstRangedWeapon,
                UsingWeapon.SecondRanged => SecondRangedWeapon,
                UsingWeapon.FirstMelee => FirstMeleeWeapon,
                _ => throw new NotImplementedException()
            };
    } }

    public float WeaponChangeTime = 2f;

    private bool _changeHand;
    private Camera _mainCamera;
    private Weapon[] _weapons;
    private Weapon _currentWeapon;

    private bool _lastStandardHand;
    private bool _lastChangeHand;

    private GameObject _leftArm;
    private GameObject _rightArm;
    private SpriteRenderer _leftRenderer;
    private SpriteRenderer _rightRenderer;
    private Transform _leftWeaponLocation;
    private Transform _rightWeaponLocation;
    private GameObject _player;
    private GameObject _weaponRoot;
    private Player _playerScript;
    private List<Transform> _weaponHoldingLocation;
    private Transform _weaponHoldingLocationRoot;

    private int _playerFlip;
    private int _mouseFlip;
    private int _flip;
    
    private bool _isChangingWeapon = false;
    private bool _isEndHoldStandardWeapon = false;
    private bool _isEndDrawStandardWeapon = false;


    const float HandAngleCorrection = 90;

    public void Init()
    {
        _mainCamera = Camera.main;
        _player = GameObject.FindWithTag("Player");
        _playerScript = _player.GetComponent<Player>();
        _leftArm = _player.transform.Find("LeftArm").gameObject;
        _rightArm = _player.transform.Find("RightArm").gameObject;
        _leftRenderer = _leftArm.GetComponent<SpriteRenderer>();
        _rightRenderer = _rightArm.GetComponent<SpriteRenderer>();
        _leftWeaponLocation = _leftArm.transform.Find("LeftWeaponLocation");
        _rightWeaponLocation = _rightArm.transform.Find("RightWeaponLocation");
        _weaponRoot = _player.transform.Find("WeaponRoot").gameObject;
        _weaponHoldingLocationRoot = _player.transform.Find("WeaponHlodingLocationRoot");
        _weaponHoldingLocation = new();
        for (int i = 0; i < _weaponHoldingLocationRoot.childCount; i++)
        {
            _weaponHoldingLocation.Add(_weaponHoldingLocationRoot.GetChild(i));
        }

        _isEndHoldStandardWeapon = false;

        UsingWeaponIndex = UsingWeapon.FirstRanged;
        SetWeaponData();
        HandAction = CheckAttack;
        HandAction += CheckFlipData;
        HandAction += CheckUsingWeaponSwitch;
        HandAction += CheckWeaponParentChange;
        HandAction += SetFlip;
        HandAction += SetArmRot;
        SetHoldWeapons();
    }


    private void SetWeaponData()
    {
        var count = _weaponRoot.transform.childCount;
        _weapons = new Weapon[count];
        for (int i = 0; i < _weaponRoot.transform.childCount; i++)
        {
            _weapons[i] = _weaponRoot.transform.GetChild(i).GetComponent<Weapon>();
        }
    }

    private void SetHoldWeapons()
    {
        SetHoldWeapon(FirstRangedWeapon, UsingWeapon.FirstRanged);
        SetHoldWeapon(SecondRangedWeapon, UsingWeapon.SecondRanged);
        SetHoldWeapon(FirstMeleeWeapon, UsingWeapon.FirstMelee);
    }

    private void SetHoldWeapon(EnumData.Weapon weaponType, UsingWeapon index)
    {
        if (weaponType == EnumData.Weapon.None)
            return;
        if (index == UsingWeaponIndex)
        {
            _lastChangeHand = !_lastChangeHand;
            _lastStandardHand = !_lastStandardHand;
            SetWeapon();
        }
        else
        {
            Weapon weapon = _weapons[(int)weaponType];
            weapon.transform.SetParent(WeaponHoldingLocation[(int)weapon.HoldingIndex], false);
            weapon.Renderer.sortingOrder = (int)weapon.HoldingOrder;
        }
    }


    private void CheckFlipData()
    {
        // �÷��̾� Y�� ���� (1 : �⺻ ����, -1 : ������ ����)
        _playerFlip = _player.transform.localEulerAngles.y == 0 ? 1 : -1;
        // ���콺�� �÷��̾� ��� ���� (1 : ����, -1 : ������)
        _mouseFlip = _player.transform.position.x > ScreenToWorld2D(Input.mousePosition).x ? 1 : -1;
        // 3. ���� ���� ���� (1: �⺻, -1: ������)
        _flip = _playerFlip * _mouseFlip;
    }

    private void SetFlip()
    {
        // 4. �Ȱ� ������ ũ�� ����
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

    private void CheckWeaponParentChange()
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
    }

    private void SetArmRot()
    {
        SetArm(HandType.Left);
        SetArm(HandType.Right);
    }

    public void CheckUsingWeaponSwitch()
    {
        if (Input.GetKeyDown(WeaponChangeKey))
        {
            if (UsingWeaponIndex == UsingWeapon.FirstMelee)
                UsingWeaponIndex = UsingWeapon.FirstRanged;
            else
                UsingWeaponIndex++;
            if(_isEndHoldStandardWeapon)
            {
                // �̹� �ֹ���� �־������ �����⸦ �����⸸ �ϸ� ��
            }
            else
            {
                SwitchWeapon();
            }
            SetHoldWeapons();
        }
    }

    private void SwitchWeapon()
    {
        _isChangingWeapon = true;
        _playerScript.StartCoroutine(HoldStandardWeapon());
        _isChangingWeapon = false;
    }

    private IEnumerator HoldStandardWeapon() // ���⸦ Ȧ���� �ִ� ����, ����Ǹ� ������ �������� !�ֿ켱!
    {
        var arm = IsRightHandStandard() ? _rightArm : _leftArm;

        var targetPos = ForwardToObj(arm, WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
        var oriPos = arm.transform.rotation;

        float t = 0;
        while (t <= WeaponChangeTime / 2f)
        {
            t += Time.deltaTime;
            if (t > WeaponChangeTime / 2f)
                t = WeaponChangeTime;
            arm.transform.rotation = Quaternion.Slerp(oriPos, targetPos, t / (WeaponChangeTime / 2f));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        arm.transform.rotation = targetPos;

        _currentWeapon.transform.SetParent(WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
        _currentWeapon.Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;

        _isEndHoldStandardWeapon = true;
    }

    private IEnumerator DrawStandardWeapon() // ���⸦ ���� ���¿��� ���õ� ���⸦ ������ ����
    {
        var arm = IsRightHandStandard() ? _rightArm : _leftArm;

        var targetPos = ForwardToObj(arm, WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex].position);
        var oriPos = arm.transform.rotation;

        float t = 0;
        while (t <= WeaponChangeTime / 2f)
        {
            t += Time.deltaTime;
            if (t > WeaponChangeTime / 2f)
                t = WeaponChangeTime;
            arm.transform.rotation = Quaternion.Slerp(oriPos, targetPos, t / (WeaponChangeTime / 2f));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        arm.transform.rotation = targetPos;

        _currentWeapon.transform.SetParent(WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
        _currentWeapon.Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;

        _isEndHoldStandardWeapon = true;
    }

    /// <summary>
    /// ���� ���� �ѵ鸸 Ȱ��ȭ ��Ų��. �� �ܴ� ��Ȱ��ȭ ��Ų��.
    /// </summary>
    private void SetHoldWeapon()
    {
        int[] holdWeapons = new int[3] { (int)FirstRangedWeapon, (int)SecondRangedWeapon, (int)FirstMeleeWeapon};

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (holdWeapons.Contains(i)) // ���� ��Ͽ� �ִٸ�
            {
                if (holdWeapons[(int)UsingWeaponIndex] == i) // �տ� ��� �ִ� ������ �տ� ������Ų��
                {
                    _currentWeapon = _weapons[i]; // ���� ���� ���� ����� ǥ���Ѵ�.
                    _currentWeapon.Renderer.sortingOrder = 4;
                    _lastStandardHand = _currentWeapon.HandState.StandardHand;

                    SetWeaponParent();
                }
                else // �տ� ��� �ִ� ���Ⱑ �ƴ϶�� Ȧ�� ��ġ�� �̵���Ų��
                {
                    _weapons[i].transform.SetParent(WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
                    _weapons[i].Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;
                }
            }
            else // ���� ��Ͽ� ���ٸ� WeaponRoot�� �̵���Ŵ.
            {
                _weapons[i].gameObject.SetActive(false);
                _weapons[i].transform.SetParent(_weaponRoot.transform, false);
            }
        }
    }



    public void SetWeapon()
    {
        if(_currentWeapon != null)
        {
            if(_currentWeapon.WeaponType == FirstRangedWeapon || _currentWeapon.WeaponType == SecondRangedWeapon || _currentWeapon.WeaponType == FirstMeleeWeapon)
            {
                Debug.Log($"{_currentWeapon.WeaponType}�� �������� ���Ⱑ �ƴմϴ�. ��ü ����");
                return;
            }
        }
        _currentWeapon = _weapons[(int)CurrentWeaponType];
        _currentWeapon.Renderer.sortingOrder = 4;
        _lastStandardHand = _currentWeapon.HandState.StandardHand;

        SetWeaponParent();

        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.Init();
    }

    public void CheckAttack()
    {
        if (_currentWeaponType == EnumData.Weapon.None)
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
        if (IsRightHandStandard())
            _currentWeapon.transform.SetParent(_rightWeaponLocation, false);
        else
            _currentWeapon.transform.SetParent(_leftWeaponLocation, false);
        _currentWeapon.transform.localPosition = Vector3.zero;
    }

    private bool IsRightHandStandard()
    {
        return ApplyHandSwap(_currentWeapon.HandState, HandType.Standard, _changeHand);
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