using SuperTiled2Unity.Editor.Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Indicator_Circle : MonoBehaviour
{
    /* Dependency */
    public Material material;

    /* Field & Property */
    public static List<Indicator_Circle> instances = new List<Indicator_Circle>();
    [Range(0, 360)] public float angle;
    public float openTime = 1f;
    public float turnSpeed = 10f;
    public float frame = 60; // 초당 이펙트 변화
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
        // 회전 및 각도 초기화
        material.SetFloat("_Angle", 0);
        transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        // 코루틴 초기화 및 시작
        if (corLast != null) StopCoroutine(corLast);
        corLast = StartCoroutine(CorIndicate());
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 다른 인디케이터는 종료하고, 현재 인디케이터는 켭니다.
    /// </summary>
    public void Swtich()
    {
        bool isActive = gameObject.activeSelf;
        foreach (Indicator_Circle item in instances) item.Off();

        if (isActive == true) Off();
        if (isActive == false) On();
    }
    /// <summary>
    /// 인디케이터를 켭니다.
    /// </summary>
    public void On()
    {
        gameObject.SetActive(true);
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
    /// 일정한 각도 범위를 표시합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorIndicate()
    {
        float elapsedTime = 0f;

        // 각도 오픈
        while (elapsedTime < openTime)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / openTime;
            if (ratio > 1) ratio = 1;

            // Lerp를 사용하여 _Angle 값을 점진적으로 변경
            float angle = Mathf.Lerp(0, this.angle, ratio);

            // 머티리얼에 값 적용
            material.SetFloat("_Angle", angle);

            yield return waitForSeconds;
        }

        // 회전
        while (true)
        {
            transform.Rotate(0f, 0f, turnSpeed * delay);
            yield return waitForSeconds;
        }
    }
}
