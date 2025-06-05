using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Receiver : MonoBehaviour
{
    /* Dependency */
    public static Receiver instance;
    private RayCaster2D rayCaster2D => RayCaster2D.instance;
    private Promotion promotion => Promotion.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Demolition demolition => Demolition.instance;
    private Grow grow => Grow.instance;
    private List<Indicator_Circle> indicator_circles => Indicator_Circle.instances;
    private List<Indicator_Arrow> indicator_arrows => Indicator_Arrow.instances;
    private Grid grid => Grid.instance;
    private Farming farming => Farming.instance;
    private Menu menu => Menu.instance;
    private Highlight highlight => Highlight.instance;
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private Player player;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button shopOnOff;
    [SerializeField] private GameObject shortcut;
    [SerializeField] private Camera camera;
    [SerializeField] private Button btnGrowYes;
    [SerializeField] private Button btnDemolitionYes;
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private EventSystem eventSystem;
    public static Action<bool> eventMove;

    /* Field & Property */
    private Coroutine corKeepAttack;
    private PointerEventData pointerEventData;

    /* Initializer & Finalizer &  Updater */
    private void Start()
    {
        instance = this;
    }
    private void OnEnable()
    {
        inputAction.Enable();

        InputActionMap map = inputAction.FindActionMap("Map");
        map.Enable();
        map.FindAction("Click").performed -= OnClick;
        map.FindAction("Click").performed += OnClick;
        map.FindAction("Interaction").performed -= OnInteraction;
        map.FindAction("Interaction").performed += OnInteraction;
        map.FindAction("Demolition").performed -= OnDemolition;
        map.FindAction("Demolition").performed += OnDemolition;
        map.FindAction("HighlightCultivate").performed -= OnHighlightCultivate;
        map.FindAction("HighlightCultivate").performed += OnHighlightCultivate;
        map.FindAction("HighlightCultivate").canceled -= OffHighlight;
        map.FindAction("HighlightCultivate").canceled += OffHighlight;
        map.FindAction("HighlightCultivate").canceled -= OnInteraction;
        map.FindAction("HighlightCultivate").canceled += OnInteraction;
        map.FindAction("HighlightDemolition").performed -= OnHighlightDemolition;
        map.FindAction("HighlightDemolition").performed += OnHighlightDemolition;
        map.FindAction("HighlightDemolition").canceled -= OffHighlight;
        map.FindAction("HighlightDemolition").canceled += OffHighlight;

        InputActionMap character = inputAction.FindActionMap("Character");
        character.Enable();
        character.FindAction("Move").performed -= OnMove;
        character.FindAction("Move").performed += OnMove;
        character.FindAction("Move").canceled -= OffMove;
        character.FindAction("Move").canceled += OffMove;
        character.FindAction("Attack").started -= OnAttack;
        character.FindAction("Attack").started += OnAttack;
        character.FindAction("Attack").performed -= KeepAttack;
        character.FindAction("Attack").performed += KeepAttack;
        character.FindAction("Attack").canceled -= OffAttack;
        character.FindAction("Attack").canceled += OffAttack;

        InputActionMap ui = inputAction.FindActionMap("UI");
        ui.Enable();
        ui.FindAction("Close").performed -= OnClose;
        ui.FindAction("Close").performed += OnClose;
        ui.FindAction("Enter").performed -= OnEnter;
        ui.FindAction("Enter").performed += OnEnter;
        ui.FindAction("Shop").performed -= OnOffShop;
        ui.FindAction("Shop").performed += OnOffShop;
    }
    private void OnDisable()
    {
        InputActionMap map = inputAction.FindActionMap("Map");
        map.FindAction("Click").performed -= OnClick;
        map.FindAction("Interaction").performed -= OnInteraction;
        map.FindAction("Demolition").performed -= OnDemolition;
        map.FindAction("HighlightCultivate").performed -= OnHighlightCultivate;
        map.FindAction("HighlightCultivate").canceled -= OffHighlight;
        map.FindAction("HighlightCultivate").canceled -= OnInteraction;
        map.FindAction("HighlightDemolition").performed -= OnHighlightDemolition;
        map.FindAction("HighlightDemolition").canceled -= OffHighlight;

        InputActionMap character = inputAction.FindActionMap("Character");
        character.FindAction("Move").performed -= OnMove;
        character.FindAction("Move").canceled -= OffMove;
        character.FindAction("Attack").started -= OnAttack;
        character.FindAction("Attack").performed -= KeepAttack;
        character.FindAction("Attack").canceled -= OffAttack;

        InputActionMap ui = inputAction.FindActionMap("UI");
        ui.FindAction("Close").performed -= OnClose;
        ui.FindAction("Enter").performed -= OnEnter;
        ui.FindAction("Shop").performed -= OnOffShop;

        inputAction.Disable();
    }

    /* Private Method */
    /// <summary>
    /// 모든 UI 요소를 끕니다.
    /// </summary>
    private void OffUI()
    {
        promotion.Off();
        reinforcement.Off();
        demolition.Off();
        grow.Off();

        // 인디케이터 종료
        foreach (var a in indicator_circles) a.Off();
        foreach (var b in indicator_arrows) b.Off();
    }

    /* Map Event Handler */
    /// <summary>
    /// HighlightCultivate + KeyDown(Right Click)
    /// </summary>
    private void OnHighlightCultivate(InputAction.CallbackContext context)
    {
        Tile tile = grid.GetNearestTile(camera.ScreenToWorldPoint(context.ReadValue<Vector2>()), true);
        if (tile == null || highlight == null) highlight.Off();
        else highlight.On(tile, false);
    }
    /// <summary>
    /// HighlightDemolition + KeyDown(Right Click)
    /// </summary>
    private void OnHighlightDemolition(InputAction.CallbackContext context)
    {
        Tile tile = grid.GetNearestTile(camera.ScreenToWorldPoint(context.ReadValue<Vector2>()), true);
        if (tile == null || highlight == null) highlight.Off();
        else highlight.On(tile, true);
    }
    private void OffHighlight(InputAction.CallbackContext context)
    {
        highlight.Off();
    }
    /// <summary>
    /// Click
    /// </summary>
    private void OnClick(InputAction.CallbackContext context)
    {
        List<RaycastResult> ui = rayCaster2D.RayCastUI(Input.mousePosition);
        if (ui.Count == 0) OffUI();
        highlight.Off();

        // 디버깅 용 코드입니다. 나중에 지울 예정
        Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        string name;
        if (tile.Go == null) name = "오브젝트가 없습니다.";
        else name = tile.Go.name;
        // Debug.Log($"타일 정보\n좌표 : [{tile.i}][{tile.j}] | pos : {tile.pos} | 설치 : {name} | GridIndexMap : {grid.GridIndexMap[tile.i, tile.j]}");
    }
    /// <summary>
    /// Click + KeyDown(Right Click)
    /// </summary>
    /// <param name="context"></param>
    private void OnInteraction(InputAction.CallbackContext context)
    {
        // 레이 캐스팅
        RaycastHit2D? hit = rayCaster2D.RayCast(Input.mousePosition);
        if (hit == null) return;

        // 충돌 대상의 RayCastee2D.OnClick 실행
        try { hit.Value.collider.GetComponent<RayCastee2D>().OnClick(); } catch { return; }
    }
    /// <summary>
    /// Click + KeyDown(F)
    /// </summary>
    /// <param name="context"></param>
    private void OnDemolition(InputAction.CallbackContext context)
    {
        if (demolition.gameObject.activeSelf) return;
        Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Tile.currentTile = tile;
        if (tile.Go != null) demolition.On();
    }

    /* Character Event Handler */
    /// <summary>
    /// KeyDown(W | A | S | D)
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        eventMove.Invoke(true);
        farming.StopCultivate();
        player.PlayerMovement._movement = context.ReadValue<Vector2>();
    }
    private void OffMove(InputAction.CallbackContext context)
    {
        eventMove.Invoke(false);
        player.PlayerMovement._movement = Vector2.zero;
    }
    /// <summary>
    /// KeyDown(Click)
    /// </summary>
    /// <param name="context"></param>
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI()) return;
        if (CheckAttack() == false) return;

        // 공격 개시
        if (player.Hand._CurrentWeapon != null) player.Hand._CurrentWeapon.AttackStart();
    }
    /// <summary>
    /// Key(Click)
    /// </summary>
    /// <param name="context"></param>
    private void KeepAttack(InputAction.CallbackContext context)
    {
        if (IsPointerOverUI()) return;
        if (shopPanel.activeInHierarchy) return;

        if (corKeepAttack != null) StopCoroutine(corKeepAttack);
        corKeepAttack = StartCoroutine(CorKeepAttack());
    }
    private IEnumerator CorKeepAttack()
    {
        while (CheckAttack())
        {
            // 근접 무기 옷 흔들기
            if (!player.Hand._CurrentWeapon.IsReload && player.Hand._CurrentWeapon.AttackType == Weapon.WeaponAttackType.Ranged ||
                player.Hand._CurrentWeapon.AttackType == Weapon.WeaponAttackType.Melee)
                player.Hand.ShakeClothes(player.Hand._CurrentWeapon.ClothesShake);

            // 원거리 무기 옷 흔들기
            if (player.Hand._CurrentWeapon.AttackType == Weapon.WeaponAttackType.Gage)
                player.Hand.ShakeClothes(player.Hand._CurrentWeapon.ClothesShake);

            // 공격 지속
            if (player.Hand._CurrentWeapon != null)
                player.Hand._CurrentWeapon.Attack();

            yield return null;
        }
    }
    private void OffAttack(InputAction.CallbackContext context)
    {
        if (corKeepAttack != null) StopCoroutine(corKeepAttack);
        if (CheckAttack() == false) return;

        // 게이지 무기 옷 흔들기
        if (!player.Hand._CurrentWeapon.IsReload && player.Hand._CurrentWeapon.AttackType == Weapon.WeaponAttackType.Charge)
            player.Hand.ShakeClothes(player.Hand._CurrentWeapon.ClothesShake);

        // 공격 종료
        if (player.Hand._CurrentWeapon != null)
            player.Hand._CurrentWeapon.AttackEnd();
    }

    /* UI Event Handler */
    /// <summary>
    /// KeyDown(ESC)
    /// </summary>
    /// <param name="context"></param>
    private void OnClose(InputAction.CallbackContext context)
    {
        bool flag = false;
        if (shopPanel.activeSelf) flag = true;
        if (shortcut.activeSelf) flag = true;
        if (promotion.gameObject.activeSelf) flag = true;
        if (reinforcement.gameObject.activeSelf) flag = true;
        if (demolition.gameObject.activeSelf) flag = true;
        if (grow.gameObject.activeSelf) flag = true;

        shopPanel.SetActive(false);
        shortcut.SetActive(false);
        OffUI();

        if(!flag || menu.panel.activeSelf) menu.MenuOnOff();
    }
    /// <summary>
    /// KeyDown(Enter)
    /// </summary>
    /// <param name="context"></param>
    private void OnEnter(InputAction.CallbackContext context)
    {
        if (demolition.gameObject.activeSelf) btnDemolitionYes.onClick.Invoke();
        if (grow.gameObject.activeSelf) btnGrowYes.onClick.Invoke();
    }
    /// <summary>
    /// KeyDown(Q)
    /// </summary>
    /// <param name="context"></param>
    private void OnOffShop(InputAction.CallbackContext context)
    {
        shopOnOff.onClick.Invoke();
    }

    /* Private Method */
    /// <summary>
    /// 공격 시 유효성 검사
    /// </summary>
    private bool CheckAttack()
    {
        if (Keyboard.current.fKey.isPressed) return false;
        if (player.Hand.CurrentSlotWeaponType == EnumData.Weapon.None) return false;
        if (player.Hand.IsSwitchWeapon) return false;
        if (Time.timeScale == 0) return false;
        return true;
    }
    /// <summary>
    /// UI 상호작용 여부 검사
    /// </summary>
    private bool IsPointerOverUI()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }
}