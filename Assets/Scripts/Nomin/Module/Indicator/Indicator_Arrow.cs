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
    public static List<Indicator_Arrow> instances = new List<Indicator_Arrow>();
    public float time = 1f;
    public float frame = 60; // 초당 각도 변화
    private float delay;
    private WaitForSeconds waitForSeconds;
    private Coroutine corLast;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        delay = 1 / frame;
        waitForSeconds = new WaitForSeconds(delay);
    }
    private void OnEnable()
    {
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorIndicate());
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 다른 인디케이터는 종료하고, 현재 인디케이터는 스위칭 합니다.
    /// </summary>
    public void Swtich()
    {
        bool isActive = gameObject.activeSelf;
        foreach (var item in instances) item.gameObject.SetActive(false);

        gameObject.SetActive(!isActive);
    }
    /// <summary>
    /// 인디케이터를 종료합니다.
    /// </summary>
    public void Off()
    {
        gameObject.SetActive(false);
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
