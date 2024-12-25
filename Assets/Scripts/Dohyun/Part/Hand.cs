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

    // Player에게 Update 실행을 넘기기 위한 Action, Player쪽에서 실행시켜준다.
    public Action HandAction;

    // 각종 키 관련 데이터, 임시로 넣은거고 키 데이터 관련은 따로 뺄 생각임!
    public int AttackKeyIndex = 0;
    public KeyCode WeaponChangeKey = KeyCode.R;


    // 실질적인 무기 데이터, 각 슬롯에 무엇을 장착 중인지, 어떤 무기를 사용 중인지
    public EnumData.Weapon FirstRangedWeapon;
    public EnumData.Weapon SecondRangedWeapon;
    public EnumData.Weapon FirstMeleeWeapon;
    public UsingWeapon UsingWeaponIndex;

    public EnumData.Weapon CurrentWeaponType { get { // 현재 무기가 무엇인지 알려주는 프로퍼티
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
        // 플레이어 Y축 방향 (1 : 기본 방향, -1 : 뒤집힌 방향)
        _playerFlip = _player.transform.localEulerAngles.y == 0 ? 1 : -1;
        // 마우스가 플레이어 상대 방향 (1 : 왼쪽, -1 : 오른쪽)
        _mouseFlip = _player.transform.position.x > ScreenToWorld2D(Input.mousePosition).x ? 1 : -1;
        // 3. 최종 방향 결정 (1: 기본, -1: 뒤집힘)
        _flip = _playerFlip * _mouseFlip;
    }

    private void SetFlip()
    {
        // 4. 팔과 무기의 크기 설정
        // 기본적으로 오른손에 총이 올라간경우 플립되어야한다. 따라서 flip, -flip으로 계산한다.
        // 하지만 뒤집힌 경우 반대로 바뀌기에 최정 방향을 기준으로 보정해준다.
        _leftArm.transform.localScale = new Vector3(_flip, 1, 1);
        _rightArm.transform.localScale = new Vector3(-_flip, 1, 1);

        // 360도로 회전시켜 방향을 나타내기에 우측으로 가는경우 (정방향 기준) y축 플립해줘야한다.
        // 하지만 이 또한 플레이어가 뒤집히면 반대로 바뀌기에 플레이어 방향을 기준으로 보정해준다.
        if (_currentWeapon.HandState.StandardHand) // 오른손에 있을때만
            _currentWeapon.transform.localScale = new Vector3(1, -_mouseFlip, 1);
        else
            _currentWeapon.transform.localScale = new Vector3(1, _mouseFlip, 1);

        // 5. _changeHand 플래그 설정
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
                // 이미 주무기는 넣어놨으니 새무기를 꺼내기만 하면 됨
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

    private IEnumerator HoldStandardWeapon() // 무기를 홀더에 넣는 과정, 실행되면 무조건 끝나야함 !최우선!
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

    private IEnumerator DrawStandardWeapon() // 무기를 넣은 상태에서 선택된 무기를 꺼내는 과정
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
    /// 착용 중인 총들만 활성화 시킨다. 그 외는 비활성화 시킨다.
    /// </summary>
    private void SetHoldWeapon()
    {
        int[] holdWeapons = new int[3] { (int)FirstRangedWeapon, (int)SecondRangedWeapon, (int)FirstMeleeWeapon};

        for (int i = 0; i < _weapons.Length; i++)
        {
            if (holdWeapons.Contains(i)) // 착용 목록에 있다면
            {
                if (holdWeapons[(int)UsingWeaponIndex] == i) // 손에 들고 있는 무기라면 손에 장착시킨다
                {
                    _currentWeapon = _weapons[i]; // 현재 장착 중인 무기로 표시한다.
                    _currentWeapon.Renderer.sortingOrder = 4;
                    _lastStandardHand = _currentWeapon.HandState.StandardHand;

                    SetWeaponParent();
                }
                else // 손에 들고 있는 무기가 아니라면 홀딩 위치로 이동시킨다
                {
                    _weapons[i].transform.SetParent(WeaponHoldingLocation[(int)_currentWeapon.HoldingIndex], false);
                    _weapons[i].Renderer.sortingOrder = (int)_currentWeapon.HoldingOrder;
                }
            }
            else // 착용 목록에 없다면 WeaponRoot로 이동시킴.
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
                Debug.Log($"{_currentWeapon.WeaponType}은 착용중인 무기가 아닙니다. 교체 실패");
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
        // 1. 마우스의 화면 좌표를 가져옴
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. 마우스의 월드 좌표를 계산 (카메라 기준 Z 깊이 설정)
        Vector3 mouseWorldPosition = ScreenToWorld2D(mouseScreenPosition);

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

    Vector3 ScreenToWorld2D(Vector3 screenPosition)
    {
        // 1. 화면 좌표를 0~1로 정규화
        float normalizedX = screenPosition.x / Screen.width;
        float normalizedY = screenPosition.y / Screen.height;

        // 2. 카메라의 월드 좌표 범위 계산
        float halfHeight = _mainCamera.orthographicSize; // 카메라의 세로 절반 크기
        float halfWidth = halfHeight * _mainCamera.aspect; // 카메라의 가로 절반 크기 (aspect 비율 고려)

        // 3. 월드 좌표 계산
        float worldX = Mathf.Lerp(_mainCamera.transform.position.x - halfWidth,
                                  _mainCamera.transform.position.x + halfWidth,
                                  normalizedX);

        float worldY = Mathf.Lerp(_mainCamera.transform.position.y - halfHeight,
                                  _mainCamera.transform.position.y + halfHeight,
                                  normalizedY);

        // 4. Z축 깊이는 0
        float worldZ = 0f;

        return new Vector3(worldX, worldY, worldZ);
    }
}