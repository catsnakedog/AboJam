using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    /* Dependency */
    public Pool pool => Pool.instance;

    /* Field & Property */
    public static List<Explosion> instances = new List<Explosion>();
    public float time = 2f; // 지속 시간
    private Coroutine lastCor;

    /* Public Method */
    public void On()
    {
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorOn());

        StartCoroutine(CorOn());
    }

    /* Private Method */
    private IEnumerator CorOn()
    {
        yield return new WaitForSeconds(time);
        pool.Return(gameObject);
        lastCor = null;
    }
}
