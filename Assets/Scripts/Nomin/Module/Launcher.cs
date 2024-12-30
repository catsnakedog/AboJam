using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    /* Dependency */
    public GameObject projectile; // 발사체

    /* Field & Property */
    public static List<Launcher> instances = new List<Launcher>(); // 모든 Launcher 인스턴스
    public List<GameObject> pool { get; private set; } = new List<GameObject>(); // 발사체 풀링
    public GameObject pool_hierarchy { get; private set; }
    public float delay = 0.1f; // 발사 딜레이
    public int count = 1; // 발사체 수
    public float speed = 0.02f; // 발사체 속도

    /* Intializer & Finalizer */
    private void Start()
    {
        if (projectile == null) Debug.Log($"{gameObject.name} 의 Launcher 에 Projectile 이 연결되지 않았습니다.");
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
            if (projectile.active == false)
            {
                go = projectile;
                go.active = true;
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
        pool_hierarchy = pool_hierarchy ?? new GameObject($"@Pool {name}");
        GameObject projectile = Instantiate(this.projectile, pool_hierarchy.transform); ;
        
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
        while (projectile != null && projectile.active == true)
        {
            // (남은 거리 < 1 회 이동 거리) 면 위치 고정
            if ((projectile.transform.position - destination).magnitude < speed)
            {
                projectile.transform.position = destination;
            }
            // 아니면 이동
            else
            {
                Vector3 direction = (destination - projectile.transform.position).normalized;
                projectile.transform.position = projectile.transform.position += direction * speed;
            }

            // 목표 지점에 도착 시 disable (pool 에 잔류)
            if (projectile.transform.position == destination) projectile.active = false;

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