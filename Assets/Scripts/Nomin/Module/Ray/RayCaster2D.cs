using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /* Public Mehtod */
    /// <summary>
    /// UI 요소를 레이캐스트 합니다. 단, Joystick 레이어는 제외합니다.
    /// </summary>
    public List<RaycastResult> RayCastUI(Vector3 mousePos)
    {
        pointerEventData.position = mousePos;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        // Joystick 레이어 오브젝트 제외
        int joystickLayer = LayerMask.NameToLayer("Joystick");
        List<RaycastResult> filteredResults = results.Where(result => result.gameObject.layer != joystickLayer).ToList();

        return filteredResults;
    }
    /// <summary>
    /// <br>단일 대상을 레이캐스팅합니다.</br>
    /// <br>UI 요소 레이캐스팅 시 null 을 반환합니다.</br>
    /// </summary>
    public RaycastHit2D? RayCast(Vector3 mousePos)
    {
        // UI 요소 레이캐스팅 시 이벤트 전달 후 리턴
        List<RaycastResult> UI = RayCastUI(mousePos);
        if (UI.Count > 0)
        {
            GameObject firstHitObject = UI[0].gameObject;

            if (ExecuteEvents.CanHandleEvent<IPointerClickHandler>(firstHitObject))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = mousePos;
                ExecuteEvents.Execute(
                    target: firstHitObject,
                    eventData: pointerData,
                    functor: ExecuteEvents.pointerClickHandler
                );
            }

            return null;
        }

        // 모든 레이캐스팅 충돌체 반환
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero, 0);
        if (hits.Length == 0) return null;

        // 거리 순 정렬
        var sortedHits = hits.OrderBy(h => h.distance).ToArray();

        // 무시하고 관통할 레이어 지정
        int penetratingLayersMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Joystick"));

        // 첫 번째로 충돌한 오브젝트 반환
        foreach (var hit in sortedHits)
        {
            bool isPenetrating = (penetratingLayersMask & (1 << hit.collider.gameObject.layer)) != 0;

            if (!isPenetrating) return hit;
        }

        return null;
    }
}
