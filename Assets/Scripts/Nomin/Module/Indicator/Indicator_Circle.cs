using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Indicator_Circle : MonoBehaviour
{
    /* Dependency */
    public Material material;

    /* Field & Property */
    [Range(0, 360)] public float angle;
    public float time = 1f;
    public float frame = 60; // 초당 각도 변화
    private float delay;
    private WaitForSeconds waitForSeconds;
    private Coroutine corLast;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        delay = 1 / frame;
        waitForSeconds = new WaitForSeconds(delay);
    }
    private void OnEnable()
    {
        material.SetFloat("_Angle", 0);
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorIndicate());
    }

    /* Private Method */
    /// <summary>
    /// 일정한 각도 범위를 표시합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorIndicate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / time;
            if (ratio > 1) ratio = 1;

            // Lerp를 사용하여 _Angle 값을 점진적으로 변경
            float angle = Mathf.Lerp(0, this.angle, ratio);

            // 머티리얼에 값 적용
            material.SetFloat("_Angle", angle);

            yield return waitForSeconds;
        }
    }
}
