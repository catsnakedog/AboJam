using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Indicator_Arrow : MonoBehaviour
{
    /* Dependency */
    public Material material;

    /* Field & Property */
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
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorIndicate());
    }

    /* Private Method */
    /// <summary>
    /// 화살표를 표시합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorIndicate()
    {
        float elapsedTime = 0f;
        Vector4 mainTexST = material.GetVector("_MainTex_ST");

        while (elapsedTime < time)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / time;
            if (ratio > 1) ratio = 1;

            mainTexST.w = Mathf.Lerp(0f, -4f, ratio);
            material.SetVector("_MainTex_ST", mainTexST);

            yield return waitForSeconds;
        }
    }
}
