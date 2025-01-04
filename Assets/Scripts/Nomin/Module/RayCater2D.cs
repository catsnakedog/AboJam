using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayCaster2D : MonoBehaviour, IPointerClickHandler
{
    /* Field & Property */
    public static RayCaster2D Instance;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        Instance = this;
    }

    /* Public Method */
    /// <summary>
    /// 마우스 클릭 이벤트 핸들러
    /// </summary>
    /// <param name="eventData">클릭한 마우스 버튼 정보 (Left / Middle / Right)</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 좌클릭 시 레이캐스팅
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 마우스 pos > 월드 pos 후 RayCasting
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 충돌한 오브젝트의 RayCastee2D.OnClick() 실행
            if (hit.collider != null)
            {
                RayCastee2D rayCastee = hit.collider.GetComponent<RayCastee2D>();
                if (rayCastee != null) rayCastee.OnClick();
            }
        }
    }
}
