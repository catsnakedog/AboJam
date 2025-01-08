using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor.Timeline.Actions;
using UnityEngine.EventSystems;

public class Receiver : MonoBehaviour
{
    /* Dependency */
    public static Receiver instance;
    public InputActionAsset inputAction;
    private RayCaster2D rayCaster2D => RayCaster2D.instance; // 하드 링크
    private Promotion promotion => Promotion.instance; // 하드 링크
    private Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    private Demolition demolition => Demolition.instance; // 하드 링크
    private Grid grid => Grid.instance; // 하드 링크

    /// <summary>
    /// 이벤트 핸들러를 정의합니다.
    /// </summary>
    private void Start()
    {
        instance = this;

        // Map 상호작용 정의
        InputActionMap map = inputAction.FindActionMap("Map");
        map.Enable();
        map.FindAction("Click").performed += (context) =>
        {
            List<RaycastResult> ui = rayCaster2D.RayCastUI(Input.mousePosition);
            if (ui.Count == 0) { promotion.Off(); reinforcement.Off(); demolition.Off(); }
        }; // Click
        map.FindAction("Interaction").performed += (context) =>
        {
            // 레이 캐스팅
            RaycastHit2D? hit = rayCaster2D.RayCast(Input.mousePosition);
            if (hit == null) return;

            // 충돌 대상의 RayCastee2D.OnClick 실행
            RayCastee2D rayCastee = hit.Value.collider.GetComponent<RayCastee2D>();
            if (rayCastee == null) return;
            rayCastee.OnClick();
        }; // Click + F 키다운
        map.FindAction("Demolition").performed += (context) =>
        {
            Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Tile.currentTile = tile;
            if (tile.Go != null) demolition.On();
        }; // Click + G 키다운
    }
}
