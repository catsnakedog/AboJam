using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light : MonoBehaviour
{
    /* Dependency */
    public Light2D light2D;

    /* Field & Property */
    public static List<Light> instances = new List<Light>();
    public float onTime = 1f; // 빛 On 시간
    public float keepTime = 2f; // 빛 Keep 시간
    public float offTime = 1f; // 빛 Off 시간

    private float intensity;
    private WaitForSeconds waitForSeconds; // 갱신 시간
    private float frame = 60; // 초당 60 번 색상 변화
    private float delay;
    private Coroutine lastCor;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instances.Add(this);
        intensity = light2D.intensity;
        delay = 1 / frame;
        waitForSeconds = new WaitForSeconds(delay);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    private void OnEnable()
    {
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorLight());
    }

    /* Public Method */
    /// <summary>
    /// 빛을 점등 후 소등 시킵니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CorLight()
    {
        float elapsedTime = 0f;

        // 빛 점등
        while (elapsedTime < onTime)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / onTime;
            if (ratio > 1) ratio = 1;

            light2D.intensity = Mathf.Lerp(0, intensity, ratio);
            Debug.Log(intensity);

            yield return waitForSeconds;
        }

        // 빛 유지
        Debug.Log(intensity);
        yield return new WaitForSeconds(keepTime);

        // 빛 소등
        elapsedTime = 0f;
        while (elapsedTime < offTime)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / offTime;
            if (ratio > 1) ratio = 1;

            light2D.intensity = Mathf.Lerp(intensity, 0, ratio);
            Debug.Log(intensity);

            yield return waitForSeconds;
        }

        lastCor = null;
    }
}
