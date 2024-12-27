using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Date : MonoBehaviour
{
    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
        StaticData.Date_Time = 0;
        StartCoroutine(CorTime());
    }

    public void SkipNIght()
    {
        StaticData.Date_isMorning = true;
    }

    IEnumerator CorTime()
    {
        while (true)
        {
            // 낮
            if (StaticData.Date_isMorning)
            {
                StaticData.Date_Day++;
                foreach (var abo in Abocado.instances) abo.GrowUp();
                if (img.sprite.name != "Date_Morning")
                {
                    img.sprite = Resources.Load<Sprite>("Images/Date_Morning");
                    GetComponent<AnimationClick>().OnClick();
                };
                float elapsedTime = 0f;

                while (elapsedTime < StaticData.Date_TimeThreshold)
                {
                    StaticData.Date_Time = Mathf.FloorToInt(720 * (elapsedTime / StaticData.Date_TimeThreshold));

                    elapsedTime += 0.01f;
                    yield return new WaitForSeconds(0.01f);
                }

                StaticData.Date_Time = 0;
                StaticData.Date_isMorning = false;
            }
            // 밤
            else
            {
                if (img.sprite.name != "Date_Night")
                {
                    img.sprite = Resources.Load<Sprite>("Images/Date_Night");
                    GetComponent<AnimationClick>().OnClick();
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
