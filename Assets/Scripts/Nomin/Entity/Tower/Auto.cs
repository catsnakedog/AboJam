using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto : Tower
{
    /* Field & Property */
    public static List<Auto> instances = new List<Auto>(); // 모든 연사 타워 인스턴스
    [SerializeField] private float delay = 0.1f; // 공격 딜레이
    public float range = 5f; // 적 감지 범위
    private WaitForSeconds delay_waitForSeconds;
    private Coroutine corFire;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        delay_waitForSeconds = new WaitForSeconds(delay);

        Fire(true);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 인접한 적에게 공격을 개시합니다.
    /// </summary>
    /// <param name="OnOff">공격 모드 On / Off</param>
    public void Fire(bool OnOff)
    {
        if (OnOff == true) corFire = StartCoroutine(CorFire());
        else StopCoroutine(corFire);
    }
    /// <summary>
    /// 공격 딜레이를 재설정 합니다.
    /// </summary>
    /// <param name="delay">딜레이</param>
    public void SetDelay(float delay)
    {
        this.delay = delay;
        delay_waitForSeconds = new WaitForSeconds(delay);
    }

    /* Private Method */
    /// <summary>
    /// 사격을 개시합니다.
    /// </summary>
    private IEnumerator CorFire()
    {
        while (true)
        {
            GameObject target = Targetting();
            if (target != null) launcher.Launch(target.transform.position);

            yield return delay_waitForSeconds;
        }
    }
    /// <summary>
    /// 사격 대상을 지정합니다.
    /// </summary>
    /// <returns>가장 가까운 적</returns>
    private GameObject Targetting()
    {
        List<GameObject> targets = TargetObjects();
        if (targets.Count == 0) { Debug.Log($"{name} 의 타겟이 존재하지 않습니다."); return null; }

        GameObject target = Closest(targets, out float distance);
        if (distance <= range) return target;
        else return null;

        /* Local Method */
        /// <summary>
        /// 발사체의 태그에 해당하는 모든 오브젝트를 반환합니다.
        /// </summary>
        List<GameObject> TargetObjects()
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (string clashTag in launcher.projectile.GetComponent<Projectile>().clashTags)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(clashTag))
                {
                    targets.Add(go);
                }
            }

            return targets;
        }
        /// <summary>
        /// 오브젝트 리스트 중 현재 오브젝트와 가장 가까운 오브젝트를 반환합니다.
        /// </summary>
        GameObject Closest(List<GameObject> targets, out float distance)
        {
            GameObject closest = null;
            float shortestDistance = Mathf.Infinity;
            foreach (GameObject go in targets)
            {
                if (go == null) continue;

                distance = Vector3.Distance(transform.position, go.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = go;
                }
            }

            distance = shortestDistance;
            return closest;
        }
    }

    /* Test Method */
    [ContextMenu("FireOnTest")]
    private void FireOnTest()
    {
        Fire(true);
    }
    [ContextMenu("FireOffTest")]
    private void FireOffTest()
    {
        Fire(false);
    }
}
