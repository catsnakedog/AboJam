using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Record : MonoBehaviour, IScrollHandler, IPointerEnterHandler
{
    /* Dependency */
    public GameObject prefab;
    private GameObject log;

    /* Field & Property */
    public string data;
    public static Action eventClick;
    public static Action eventHover;

    /// <summary>
    /// 부모 ScrollRect 로 스크롤 이벤트를 전달합니다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.scrollHandler);
    }
    /// <summary>
    /// 로그를 스위칭합니다.
    /// </summary>
    public void ViewLog()
    {
        eventClick.Invoke();
        if (log == null)
        {
            log = Instantiate(prefab, gameObject.transform.parent);
            log.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);

            log.GetComponentInChildren<TextMeshProUGUI>().text = data;
        }
        else Destroy(log);
    }
    /// <summary>
    /// 로그를 작성합니다.
    /// </summary>
    public void WriteLog(GameData gameData)
    {
        data =
            $"처치한 적 : {gameData.kill}\n" +
            $"심은 아보카도 수 : {gameData.abocado}\n" +
            $"건설한 타워 수 : {gameData.tower}\n" +
            $"사용한 보약 수 : {gameData.garu}\n";
    }

    /// <summary>
    /// 마우스 호버링 이벤트입니다.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventHover.Invoke();
    }
}
