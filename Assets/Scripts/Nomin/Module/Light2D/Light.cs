using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light : MonoBehaviour, IPoolee
{
    /* Dependency */
    public Light2D light2D;
    public Pool pool => Pool.instance;

    /* Field & Property */
    public static List<Light> instances = new List<Light>();
    public float onTime = 1f; // 빛 On 시간
    public float keepTime = 2f; // 빛 Keep 시간
    public float offTime = 1f; // 빛 Off 시간
    public float frame = 60; // 초당 빛 변화
    private float intensity; // 원래 빛 광도
    private WaitForSeconds waitForSeconds;
    private float delay;
    private Coroutine corLast;

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
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorLight());
    }
    public void Load() { } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save() { } // 풀에 집어 넣을 때 자동 실행

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
            yield return waitForSeconds;
        }

        // 빛 유지
        yield return new WaitForSeconds(keepTime);

        // 빛 소등
        elapsedTime = 0f;
        while (elapsedTime < offTime)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / offTime;
            if (ratio > 1) ratio = 1;

            light2D.intensity = Mathf.Lerp(intensity, 0, ratio);
            yield return waitForSeconds;
        }

        corLast = null;
        pool.Return(gameObject);
    }
}
