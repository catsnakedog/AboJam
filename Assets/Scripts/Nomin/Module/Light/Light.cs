using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light : RecordInstance<Table_Light, Record_Light>, IPoolee
{
    /* Dependency */
    public Light2D light2D;
    public Pool pool => Pool.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Light> instances = new List<Light>();
    public UnityEngine.Color color = UnityEngine.Color.white; // 빛 색상
    public float radius = 3f; // 빛 반지름
    public float intensity = 3; // 빛 광도
    public float onTime = 1f; // 빛 On 시간
    public float keepTime = 2f; // 빛 Keep 시간
    public float offTime = 1f; // 빛 Off 시간
    public float frame = 60; // 초당 빛 변화
    private WaitForSeconds waitForSeconds;
    private float delay;
    private Coroutine corLast;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        instances.Add(this);

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
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportLight(initialRecords[0].ID, ref color, ref radius, ref intensity, ref onTime, ref keepTime, ref offTime, ref frame);

        light2D.color = new UnityEngine.Color(color.r / 255f, color.g / 255f, color.b / 255f, color.a);
        light2D.pointLightOuterRadius = radius;
        light2D.intensity = intensity;
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
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
