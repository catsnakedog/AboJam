using System.Collections;
using UnityEngine;

public class AnimationCameraShake : MonoBehaviour
{
    /* Field & Property */
    public static AnimationCameraShake instance;
    public float duration = 0.2f;
    public float magnitude = 0.01f;

    /* Initializer & Finalizer & Updater */
    void Awake() { instance = this; }

    /* Public Method */
    /// <summary>
    /// 화면을 흔듭니다.
    /// </summary>
    public void StartShake()
    {
        StartCoroutine(Shake());
    }

    /* Private Method */
    /// <summary>
    /// 화면을 흔듭니다.
    /// </summary>
    private IEnumerator Shake()
    {
        float elapsed = 0.0f;
        Vector3 lastShakeVector = Vector3.zero;

        while (elapsed < duration)
        {
            float x = (Mathf.PerlinNoise(Time.time * 10f, 0f) - 0.5f) * magnitude;
            float y = (Mathf.PerlinNoise(0f, Time.time * 10f) - 0.5f) * magnitude;

            transform.localPosition -= lastShakeVector;
            lastShakeVector = new Vector3(x, y, 0f);
            transform.localPosition += lastShakeVector;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition -= lastShakeVector;
    }
}
