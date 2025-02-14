using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Timeline.Actions;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Unity.VisualScripting;

public class Receiver : MonoBehaviour
{
    /* Dependency */
    public static Receiver instance;
    public InputActionAsset inputAction;
    private RayCaster2D rayCaster2D => RayCaster2D.instance;
    private Promotion promotion => Promotion.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Demolition demolition => Demolition.instance;
    private List<Indicator_Circle> indicator_circle => Indicator_Circle.instances;
    private List<Indicator_Arrow> indicator_arrow => Indicator_Arrow.instances;
    private Grid grid => Grid.instance;
    private Farming farming => Farming.instance;
    [SerializeField] private Player player;

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

        InputActionMap character = inputAction.FindActionMap("Character");
        character.Enable();
        character.FindAction("Move").performed -= OnMove;
        character.FindAction("Move").performed += OnMove;
        character.FindAction("Move").canceled -= OffMove;
        character.FindAction("Move").canceled += OffMove;
    }
    private void OnDisable()
    {
        InputActionMap map = inputAction.FindActionMap("Map");
        map.FindAction("Click").performed -= OnClick;
        map.FindAction("Interaction").performed -= OnInteraction;
        map.FindAction("Demolition").performed -= OnDemolition;

        InputActionMap character = inputAction.FindActionMap("Character");
        character.FindAction("Move").performed -= OnMove;
        character.FindAction("Move").canceled -= OffMove;

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

        // 인디케이터
        //foreach (var item in indicator_circle) item.Off();
        //foreach (var item in indicator_arrow) item.Off();
    }

    /* Event Handler */
    /// <summary>
    /// Click
    /// </summary>
    private void OnClick(InputAction.CallbackContext context)
    {
        List<RaycastResult> ui = rayCaster2D.RayCastUI(Input.mousePosition);
        if (ui.Count == 0) OffUI();

        // 디버깅 용 코드입니다. 나중에 지울 예정
        Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        string name;
        if (tile.Go == null) name = "오브젝트가 없습니다.";
        else name = tile.Go.name;
        Debug.Log($"타일 정보\n좌표 : [{tile.i}][{tile.j}] | pos : {tile.pos} | 설치 : {name} | GridIndexMap : {grid.GridIndexMap[tile.i, tile.j]}");
    }
    /// <summary>
    /// Click + KeyDown(F)
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
    /// Click + KeyDown(G)
    /// </summary>
    /// <param name="context"></param>
    private void OnDemolition(InputAction.CallbackContext context)
    {
        Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Tile.currentTile = tile;
        if (tile.Go != null) demolition.On();
    }
    /// <summary>
    /// KeyDown(W | A | S | D)
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        farming.StopCultivate();
        player.PlayerMovement._movement = context.ReadValue<Vector2>();
    }
    /// <summary>
    /// KeyUp(W & A & S & D)
    /// </summary>
    /// <param name="context"></param>
    private void OffMove(InputAction.CallbackContext context)
    {
        player.PlayerMovement._movement = Vector2.zero;
    }
}