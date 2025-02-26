using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    /* Dependency */
    public Camera cam;
    public GameObject UI;

    /* Field & Property */
    public float speed_zoom ;
    public float min_zoom;
    public float max_zoom;
    public float time_smooth;
    private float target_zoom;
    private float speed_zoom_before;
    private bool isOnUI = true;

    /* Initalizer & Finalizer & Updater */
    private void Start()
    {
        target_zoom = cam.orthographicSize;
    }
    private void FixedUpdate()
    {
        // 최소 줌 아니면 UI 숨기기
        if (target_zoom > min_zoom && isOnUI) HideUI(UI.transform, false);
        else if(target_zoom == min_zoom && !isOnUI) HideUI(UI.transform, true);

        // PC (마우스 휠)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            target_zoom -= scroll * speed_zoom;
            target_zoom = Mathf.Clamp(target_zoom, min_zoom, max_zoom);
        }

        // 모바일 (두 손가락 터치)
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 두 손가락 사이 거리 차이 계산 (Zoom 전 / 후)
            float prevDistance = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;
            float distance = prevDistance - currentDistance;

            target_zoom += distance * speed_zoom * 0.01f;
            target_zoom = Mathf.Clamp(target_zoom, min_zoom, max_zoom);
        }

        // 부드러운 전환
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, target_zoom, ref speed_zoom_before, time_smooth);
    }

    /* Private Method */
    /// <summary>
    /// UI 요소를 재귀적으로 숨깁니다. (이미지 / TMP)
    /// </summary>
    void HideUI(Transform parent, bool visible)
    {
        foreach (Transform child in parent)
        {
            // 메뉴 / 상점 / 스킬은 제외합니다.
            if (child.name == "Menu") continue;
            if (child.name == "Shop") continue;
            if (child.name == "Skill") continue;

            Image img = child.GetComponent<Image>();
            if (img != null) img.enabled = visible;

            TextMeshProUGUI tmp = child.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                UnityEngine.Color color = tmp.color;
                color.a = visible ? 1f : 0f;
                tmp.color = color;
            }

            HideUI(child, visible);
        }

        isOnUI = visible;
    }
}
