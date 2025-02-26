using Synty.Interface.FantasyWarriorHUD.Samples;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CoolTimer : MonoBehaviour
{
    /* Dependency */
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] Image image;
    [SerializeField] SampleButtonAction sampleButtonAction;

    /* Field & Property */
    public static CoolTimer instance;
    Coroutine corLast;
    float remainTime; public float RemainTime { get => remainTime; }

    /* Intializer & Finalizer & Updater */
    void Start()
    {
        instance = this;
    }

    /* Public Method */
    /// <summary>
    /// 타이머를 시작합니다.
    /// </summary>
    public void Go(float seconds)
    {
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorGo(seconds));
    }
    /// <summary>
    /// 현재 타이머에 시간을 추가합니다.
    /// </summary>
    public void AddTime(float seconds)
    {
        remainTime += seconds;
    }

    /* Private Method */
    /// <summary>
    /// 타이머를 시작합니다.
    /// </summary>
    IEnumerator CorGo(float seconds)
    {
        remainTime = seconds;
        float lastTime = Time.time;

        while (remainTime > 0)
        {
            // 시간 계산
            float checkedTime = Time.time;
            float timeDiff = checkedTime - lastTime;
            lastTime = checkedTime;
            remainTime -= timeDiff;

            // 이미지 & 텍스트 시각화
            image.fillAmount = remainTime / seconds;
            string[] tokens = TimeSpan.FromSeconds(remainTime).ToString("m\\:ss").Split(':');
            tmp.text = string.Format("{0}:{1}", tokens[0], tokens[1]);

            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
