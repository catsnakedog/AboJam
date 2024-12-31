using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderData;

public class Launcher : MonoBehaviour
{
    /* Dependency */
    public GameObject projectile; // 발사체
    public Targeter targeter; // 조준경

    /* Field & Property */
    public static List<Launcher> instances = new List<Launcher>(); // 모든 Launcher 인스턴스
    public List<GameObject> pool { get; private set; } = new List<GameObject>(); // 발사체 풀링
    public GameObject pool_root { get; private set; }
    public float speed = 0.02f; // 발사체 속도
    public float range = 5f; // 발사체 유효 사거리 (!= 타겟 감지 거리)

    /* Intializer & Finalizer */
    private void Awake()
    {
        pool_root = GameObject.Find("@Pooling") ?? new GameObject("@Pooling");
    }
    private void Start()
    {
        if (projectile == null) Debug.Log($"{gameObject.name} 의 Launcher 에 Projectile 이 연결되지 않았습니다.");
        instances.Add(this);
    }
    private void OnDestroy()
    {
        // 생성된 발사체 제거
        foreach (var item in pool) Destroy(item);
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 발사체를 목적지까지 등속으로 발사합니다.
    /// </summary>
    /// <param name="destination">목적지</param>
    public void Launch(Vector3 destination)
    {
        // 발사체 장전 (풀링 or 생성)
        GameObject projectile = SearchPool() ?? Create();
        projectile.transform.position = transform.position;

        StartCoroutine(CorLaunch(projectile, destination));
    }
    /// <summary>
    /// 일정 거리 이내의 적을 자동 타게팅하여 발사합니다.
    /// </summary>
    /// <param name="targetType">타게팅 기준</param>
    /// <param name="range">타겟 감지 거리 (!= 유효 사거리)</param>
    public void Launch(Targeter.TargetType targetType, float detection)
    {
        GameObject target = targeter.Targetting(targetType, projectile.GetComponent<Projectile>().clashTags, detection);
        if (target != null) Launch(target.transform.position);
    }

    /* Private Method */
    /// <summary>
    /// <br>pool 에서 비활성화된 발사체를 찾습니다.</br>
    /// </summary>
    /// <returns>
    /// <br>pool 에서 사용 가능한 발사체 입니다.</br>
    /// <br>사용 가능한 발사체가 없으면 null 을 반환합니다.</br>
    /// </returns>
    private GameObject SearchPool()
    {
        GameObject go = null;

        // pool 에서 비활성화된 발사체를 찾습니다.
        foreach (var projectile in pool)
        {
            if (projectile.activeSelf == false)
            {
                go = projectile;
                go.SetActive(true);
                break;
            }
        }

        return go;
    }
    /// <summary>
    /// 발사체를 새로 생성합니다.
    /// </summary>
    /// <returns>새로 생성된 발사체 입니다.</returns>
    private GameObject Create()
    {
        GameObject projectile = Instantiate(this.projectile, pool_root.transform); ;

        pool.Add(projectile);
        return projectile;
    }
    /// <summary>
    /// 발사체를 목표 지점까지 등속 운동 시킵니다.
    /// </summary>
    /// <param name="projectile">발사체</param>
    /// <param name="destination">목적지</param>
    private IEnumerator CorLaunch(GameObject projectile, Vector3 destination)
    {
        Vector3 startPos = transform.position;

        while (projectile != null && projectile.activeSelf == true)
        {
            // 진행 방향 & 타겟으로의 방향
            Vector3 direction = (destination - startPos).normalized;
            Vector3 antiDirection = (destination - projectile.transform.position).normalized;

            projectile.transform.position = projectile.transform.position += direction * speed;

            // 사거리를 벗어나면 비활성화
            if (range < (projectile.transform.position - startPos).magnitude) projectile.GetComponent<Projectile>().Disappear();

            yield return new WaitForSeconds(0.016f); // 대략 60 프레임 기준
        }
    }

    /* Test Method */
    [ContextMenu("LaunchTest")]
    private void LaunchTest()
    {
        Launch(Vector3.zero);
    }
}