using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mark : MonoBehaviour
{
    /* Dependency */
    [SerializeField] GameObject mark;
    [SerializeField] Camera camera;
    [SerializeField] RectTransform rectCanvas;
    [SerializeField] RectTransform rectMark;

    /* Field & Property */
    public static Mark instance;
    private List<Coroutine> corsMark = new();

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// 지정한 오브젝트를 seconds 만큼 마킹합니다.
    /// </summary>
    public void On(GameObject gameObject, float seconds)
    {
        Off();
        this.gameObject.SetActive(true);
        foreach (var item in corsMark) StopCoroutine(item);
        corsMark.Clear();
        corsMark.Add(StartCoroutine(CorOn(gameObject, seconds)));
        corsMark.Add(StartCoroutine(CorTrace(gameObject, seconds)));
    }

    private IEnumerator CorOn(GameObject gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        mark.SetActive(false);
    }
    /// <summary>
    /// 마킹을 종료합니다.
    /// </summary>
    public void Off()
    {
        mark.SetActive(false);
    }

    /* Private Method */
    /// <summary>
    /// 화면 움직임에 맞춰 자연스럽게 마킹을 이동시킵니다.
    /// </summary>
    private IEnumerator CorTrace(GameObject gameObject, float seconds)
    {
        float sec = 0;
        float tick = 0.01f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(tick);
        RectTransform rectTarget = gameObject.GetComponent<RectTransform>();

        while (sec < seconds)
        {
            sec += tick;

            // 타겟이 RectTransform 일 때
            if (rectTarget != null)
            {
                Vector3 worldPos = rectTarget.position;
                Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectCanvas, screenPos, camera, out Vector2 localPos);
                rectMark.anchoredPosition = localPos;
                yield return waitForSeconds; continue;
            }
            // 타겟이 Transform 일 때
            else
            {
                // 해상도 기준 위치 (0 ~ 1)
                Vector3 screenPos = camera.WorldToScreenPoint(gameObject.transform.position);
                float ratioX = screenPos.x / Screen.width;
                float ratioY = screenPos.y / Screen.height;

                // UI 캔버스 기준 위치 (해상도 0 ~ 1 >> position)
                float posX = -(rectCanvas.rect.width / 2) + (ratioX * rectCanvas.rect.width);
                float posY = -(rectCanvas.rect.height / 2) + (ratioY * rectCanvas.rect.height);

                rectMark.anchoredPosition = new Vector2(posX, posY);

                yield return waitForSeconds;
            }
        }
    }
}
