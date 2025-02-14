using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CoolTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Image image;
    private Coroutine corLast;

    public void Go(float seconds)
    {
        if(corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorGo(seconds));
    }
    private IEnumerator CorGo(float seconds)
    {
        float remainTime = seconds;

        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            image.fillAmount = remainTime / seconds;

            string[] tokens = TimeSpan.FromSeconds(remainTime).ToString("s\\:ff").Split(':');
            tmp.text = string.Format("{0}:{1}", tokens[0], tokens[1]);

            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
