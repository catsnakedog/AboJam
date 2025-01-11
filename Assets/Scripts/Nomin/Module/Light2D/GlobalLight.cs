using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    /* Dependency */
    public Light2D light2D;

    /* Field & Property */
    public static GlobalLight instance;
    public LightPreset morning = new LightPreset(1, new Color32(255, 255, 255, 255));
    public LightPreset sunset = new LightPreset(0.55f, new Color32(245, 113, 97, 255));
    public LightPreset night = new LightPreset(0.2f, new Color32(255, 255, 255, 255));
    private WaitForSeconds waitForSeconds;
    private int frame = 60; // 초당 60 번 색상 변화
    private Coroutine lastCor;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        waitForSeconds = new WaitForSeconds((1 / frame));
    }

    /* Public Method */
    /// <summary>
    /// 전역 빛을 부드럽게 조정합니다.
    /// </summary>
    /// <param name="lightPreset">사전 설정된 빛 세팅 으로, GlobalLight.cs 에서 정의합니다.</param>
    /// <param name="speed">전역 빛이 변하는 속도 입니다. (0 ~ 1)</param>
    public void Set(LightPreset lightPreset, float speed)
    {
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorSetLight(lightPreset, speed));
    }

    /* Private Method */
    /// <summary>
    /// 전역 빛을 부드럽게 조절합니다.
    /// </summary>
    /// <param name="lightPreset"></param>
    /// <param name="speed">전역 빛이 변하는 속도 입니다. (0 ~ 1)</param>
    /// <returns></returns>
    private IEnumerator CorSetLight(LightPreset lightPreset, float speed)
    {
        float startIntensity = light2D.intensity;
        Color startColor = light2D.color;
        float ratio = 0;

        // 광도, 색상을 선형 보간
        while (ratio < 1)
        {
            ratio += speed;
            if (ratio > 1) ratio = 1;

            light2D.intensity = Mathf.Lerp(startIntensity, lightPreset.intensity, ratio);
            light2D.color = Color.Lerp(startColor, lightPreset.color, ratio);

            yield return waitForSeconds;
        }

        lastCor = null;
    }
}

/// <summary>
/// 글로빌 빛 프리셋 입니다.
/// </summary>
public struct LightPreset
{
    public float intensity;
    public Color32 color;

    public LightPreset(float intensity, Color32 color)
    {
        this.intensity = intensity;
        this.color = color;
    }
}