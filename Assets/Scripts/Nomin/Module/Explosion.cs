using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    /* Field & Property */
    private Coroutine coroutine;
    public float radius; // 폭발 데미지 반지름

    /* Intializer & Finalizer & Updater */


    /* Public Method */
    public void Explode(float seconds)
    {
        if(coroutine != null) StopCoroutine(coroutine);
        //SplashDamage(radius);
        coroutine = StartCoroutine(CorOn(seconds));
    }

    /* Private Method */
    /// <summary>
    /// 일정 시간동안 이펙트를 보입니다.
    /// </summary>
    /// <param name="seconds">이펙트 유지 시간</param>
    /// <returns></returns>
    private IEnumerator CorOn(float seconds)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 일정 거리 이내, 특정 태그의 타겟에게 데미지를 입힙니다.
    /// </summary>
    /// <param name="distance"></param>
    private void SplashDamage(string[] tags, float distance)
    {
        // 타겟 태그를 체크하고..
        // 태그의 모든 적을 반환하고..
        // 

        /// <summary>
        /// 태그에 해당하는 모든 오브젝트를 반환합니다.
        /// </summary>
        List<GameObject> GetTaged(string[] tags)
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (string tag in tags)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
                {
                    targets.Add(go);
                }
            }

            return targets;
        }
    }
}
