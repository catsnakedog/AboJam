using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Message : MonoBehaviour
{
    /* Dependency */
    public TextMeshProUGUI textMeshProUGUI;

    /* Field & Property */
    public static Message instance; // 싱글턴
    private Coroutine lastCor;
    private bool _force = false;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// 일정 시간동안 메시지를 출력합니다.
    /// </summary>
    /// <param name="force">이 메시지가 종료될 때 까지 다른 메시지가 출력되지 못하게 합니다.</param>
    public void On(string text, float seconds, bool force = false)
    {
        if (_force == true && force == false) return;
        else if (force == true) _force = force;

        gameObject.SetActive(false);
        gameObject.SetActive(true);
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorOn(text, seconds));
    }
    /// <summary>
    /// 메시지를 닫습니다.
    /// </summary>
    public void Off()
    {
        gameObject.SetActive(false);
        _force = false;
    }

    /* Private Method */
    /// <summary>
    /// 일정 시간동안 메시지를 출력합니다.
    /// </summary>
    /// <param name="text">메시지</param>
    /// <param name="seconds">시간</param>
    private IEnumerator CorOn(string text, float seconds)
    {
        textMeshProUGUI.text = text;
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        _force = false;
    }
}
