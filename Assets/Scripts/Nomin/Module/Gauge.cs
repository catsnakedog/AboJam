using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spr_empty;
    public SpriteRenderer spr_max;

    /* Field & Property */
    public static List<Gauge> instances = new List<Gauge>();
    public float speed = 0.02f;
    public float max = 100f;
    public float current = 0f;
    public float ratio = 0f;
    private Material material; // spr_max 를 우측부터 자르기 위한 머터리얼

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        material = spr_max.material;
        spr_max.sortingOrder = spr_empty.sortingOrder + 1;
        material.SetFloat("_Fill", 0);
    }
    private void FixedUpdate()
    {
        UpdateGauge();
    }
    /// <summary>
    /// 게이지를 업데이트 합니다.
    /// </summary>
    private void UpdateGauge()
    {
        // ratio 에 변동이 생겼을 때만 동작
        float ratio_new = current / max;
        if (Mathf.Abs(ratio - ratio_new) < 0.01f) return;

        // UI 업데이트
        ratio = Mathf.Lerp(material.GetFloat("_Fill"), ratio_new, speed);
        material.SetFloat("_Fill", ratio);
    }

    /* Public Method */
    /// <summary>
    /// 게이지를 비웁니다.
    /// </summary>
    /// <param name="value">소모량</param>
    public void Drain(float value)
    {
        if (current == 0) return;
        current = Mathf.Max(current - value, 0);
    }
    /// <summary>
    /// 게이지를 채웁니다.
    /// </summary>
    /// <param name="value">회복량</param>
    public void Fill(float value)
    {
        current = Mathf.Min(current + value, max);
    }
}