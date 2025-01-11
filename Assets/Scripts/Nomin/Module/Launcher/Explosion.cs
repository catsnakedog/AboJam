using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Explosion : MonoBehaviour
{
    /* Dependency */
    public GameObject Light2D { get { return light2D; } private set { light2D = value; } } // 빛, 없어도 작동
    public Targeter targeter; // 조준경
    public Pooling pooling; // 풀링

    /* Field & Property */
    public static List<Explosion> instances = new List<Explosion>();
    public float radius = 5f; // 폭발 반경
    public float damage = 3f; // 폭발 데미지
    public float time = 2f; // 폭발 시간
    private Coroutine lastCor;

    /* Backing Field */
    [SerializeField] private GameObject light2D;

    /* Initializer & Finalizer & Updater */
    private void Awake()
    {
        if (Light2D != null) pooling.Set(Light2D);
    }
    private void Start()
    {
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 폭발합니다.
    /// </summary>
    /// <param name="tags">피해를 입힐 오브젝트의 태그</param>
    /// <param name="seconds">이펙트 재생 시간</param>
    public void Explode(string[] tags)
    {
        SplashDamage(tags, radius);
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorOn());

        // 빛 (풀링 or 생성)
        if (Light2D != null)
        {
            GameObject light2D = pooling.Get();
            light2D.transform.position = transform.position;
        }
    }

    /* Private Method */
    /// <summary>
    /// 일정 시간동안 이펙트를 보입니다.
    /// </summary>
    /// <param name="seconds">이펙트 유지 시간</param>
    /// <returns></returns>
    private IEnumerator CorOn()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        lastCor = null;
    }
    /// <summary>
    /// 일정 거리 이내, 특정 태그의 타겟에게 데미지를 입힙니다.
    /// </summary>
    /// <param name="distance"></param>
    private void SplashDamage(string[] tags, float radius)
    {
        List<GameObject> targets = targeter.GetTargets(tags, radius);
        foreach (GameObject target in targets) HP.FindHP(target).Damage(damage);
    }
}
