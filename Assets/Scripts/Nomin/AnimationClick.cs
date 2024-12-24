using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClick : MonoBehaviour
{
    public float scaleFactor = 1.2f;
    public float animationDuration = 0.2f;

    private Coroutine corLast;
    private Vector3 originalScale;
    private Vector3 ref_realScale
    {
        get
        {
            RectTransform rect = GetComponent<RectTransform>();
            if (rect == null) return transform.localScale;
            else return rect.localScale;
        }

        set
        {
            RectTransform rect = GetComponent<RectTransform>();
            if (rect == null) transform.localScale = value;
            else rect.localScale = value;
        }
    }
    void Start() { originalScale = ref_realScale; }

    /// <summary>
    /// 자연스러운 클릭 애니메이션을 연출합니다.
    /// </summary>
    public void OnClick()
    {
        if (corLast != null) StopCoroutine(corLast);
        ref_realScale = originalScale;
        corLast = StartCoroutine(CorClick());
    }
    private IEnumerator CorClick()
    {
        // 크기 증가
        float elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            ref_realScale = Vector3.Lerp(originalScale, originalScale * scaleFactor, elapsed / (animationDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        ref_realScale = originalScale * scaleFactor;

        // 크기 복구
        elapsed = 0f;
        while (elapsed < animationDuration / 2)
        {
            ref_realScale = Vector3.Lerp(originalScale * scaleFactor, originalScale, elapsed / (animationDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        ref_realScale = originalScale;
    }
}