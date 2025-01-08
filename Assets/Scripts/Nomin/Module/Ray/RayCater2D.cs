using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RayCaster2D : MonoBehaviour
{
    /* Dependency */
    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;
    public Promotion promotion => Promotion.instance; // 하드 링크
    public Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    public Demolition demolition => Demolition.instance; // 하드 링크
    public Grid grid => Grid.instance; // 하드 링크

    /* Field & Property */
    public static RayCaster2D instance;
    private PointerEventData pointerEventData;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        pointerEventData = new PointerEventData(eventSystem);
        instance = this;
    }
    private void Update()
    {
        // 좌클릭 시 UI 닫음 (단, UI 클릭은 제외)
        if (!Input.GetMouseButtonDown(0)) return;
        List<RaycastResult> ui = RayCastUI(Input.mousePosition);
        if (ui.Count == 0) { promotion.Off(); reinforcement.Off(); demolition.Off(); }

        // 레이 캐스팅
        RaycastHit2D? hit = RayCast(Input.mousePosition);
        if (hit == null) return;

        // F 키다운 시 타워 & 아보카도 상호작용
        if (Input.GetKey(KeyCode.F))
        {
            // 충돌 대상의 RayCastee2D.OnClick 실행
            RayCastee2D rayCastee = hit.Value.collider.GetComponent<RayCastee2D>();
            if (rayCastee == null) return;
            rayCastee.OnClick();
            return;
        }

        // G 키다운 시 해당 위치 철거 패널 On
        if (Input.GetKey(KeyCode.G))
        {
            Tile tile = grid.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Tile.currentTile = tile;
            if(tile.Go != null) demolition.On();
            return;
        }
    }

    /* Public Mehtod */
    /// <summary>
    /// UI 요소를 레이캐스트 합니다.
    /// </summary>
    public List<RaycastResult> RayCastUI(Vector3 mousePos)
    {
        pointerEventData.position = mousePos;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        return results;
    }
    /// <summary>
    /// <br>단일 대상을 레이캐스팅합니다.</br>
    /// <br>UI 요소 레이캐스팅 시 null 을 반환합니다.</br>
    /// </summary>
    public RaycastHit2D? RayCast(Vector3 mousePos)
    {
        // UI 요소 레이캐스팅 시 리턴
        List<RaycastResult> UI = RayCastUI(mousePos);
        if (UI.Count > 0) return null;

        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);
    }
}
