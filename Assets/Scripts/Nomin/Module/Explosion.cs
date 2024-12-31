using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    /* Field & Property */
    public Coroutine coroutine;
    public Targeter targeter; // 조준경
    public List<Explosion> instances = new List<Explosion>();
    public float radius = 5f; // 폭발 반경
    public float damage = 3f; // 폭발 데미지
    public float time = 2f; // 폭발 시간

    /* Initializer & Finalizer & Updater */
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
        coroutine = StartCoroutine(CorOn());
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
