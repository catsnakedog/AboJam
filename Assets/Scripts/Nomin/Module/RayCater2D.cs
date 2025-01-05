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
        // 좌클릭 시 레이캐스팅
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit2D? hit = RayCast(Input.mousePosition);
        if (hit == null) return;

        // 충돌 대상의 RayCastee2D.OnClick 실행
        RayCastee2D rayCastee = hit.Value.collider.GetComponent<RayCastee2D>();
        if (rayCastee == null) return;
        rayCastee.OnClick();
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
