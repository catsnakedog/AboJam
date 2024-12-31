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
    public enum TargetType // 오토 타게팅 타입
    {
        /// <summary>
        /// 가장 가까운 적을 타게팅
        /// </summary>
        Near,
        /// <summary>
        /// 가장 낮은 체력의 적을 타게팅
        /// </summary>
        LowHP,
    }

    /* Field & Property */
    public static List<Launcher> instances = new List<Launcher>(); // 모든 Launcher 인스턴스
    public List<GameObject> pool { get; private set; } = new List<GameObject>(); // 발사체 풀링
    public GameObject pool_hierarchy { get; private set; }
    public float speed = 0.02f; // 발사체 속도
    public float range = 5f; // 발사체 유효 사거리 (!= 타겟 감지 거리)

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
    /// <summary>
    /// 일정 거리 이내의 적을 자동 타게팅하여 발사합니다.
    /// </summary>
    /// <param name="targetType">타게팅 기준</param>
    /// <param name="range">타겟 감지 거리 (!= 유효 사거리)</param>
    public void Launch(TargetType targetType, float detection)
    {
        GameObject target = Targetting(targetType, detection);
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
        Vector3 startPos = transform.position;

        while (projectile != null && projectile.activeSelf == true)
        {
            // 진행 방향 & 타겟으로의 방향
            Vector3 direction = (destination - startPos).normalized;
            Vector3 antiDirection = (destination - projectile.transform.position).normalized;

            projectile.transform.position = projectile.transform.position += direction * speed;

            // 사거리를 벗어나면 즉시 비활성화
            if (range < (projectile.transform.position - startPos).magnitude) projectile.SetActive(false);

            yield return new WaitForSeconds(0.016f); // 대략 60 프레임 기준
        }
    }
    /// <summary>
    /// <br>사격 대상을 지정합니다.</br>
    /// <br>타겟이 없다면 null 을 반환합니다.</br>
    /// </summary>
    /// <returns>가장 가까운 적</returns>
    private GameObject Targetting(TargetType targetType, float detection)
    {
        switch (targetType)
        {
            case TargetType.Near:
                return Near(detection);
            case TargetType.LowHP:
                return LowHP(detection);
            default:
                Debug.Log($"지정한 {targetType} 의 동작이 정의되지 않았습니다.");
                return null;
        }

        /* Local Method */
        /// <summary>
        /// <br>오브젝트 리스트 중 현재 오브젝트와 가장 가까운 오브젝트를 반환합니다.</br>
        /// </summary>
        GameObject Near(float detection)
        {
            List<GameObject> targets = GetTargets(detection);

            if (targets.Count > 0) return targets[0];
            else return null;
        }
        /// <summary>
        /// <br>사거리 이내에서 체력이 가장 낮은 오브젝트를 반환합니다.</br>
        /// </summary>
        GameObject LowHP(float detection)
        {
            List<GameObject> targets = GetTargets(detection);
            List<KeyValuePair<GameObject, float>> targetsWithHP = new List<KeyValuePair<GameObject, float>>();
            foreach (HP HP in HP.instances)
            {
                if (targets.Contains(HP.entity)) targetsWithHP.Add(new KeyValuePair<GameObject, float>(HP.entity, HP.HP_current));
            }
            targetsWithHP.OrderBy(pair => pair.Value).ToList();

            if (targetsWithHP.Count > 0) return targetsWithHP[0].Key;
            else return null;
        }
        /// <summary>
        /// 범위 이내의 타겟을 반환합니다. (거리 순 ASC 정렬)
        /// </summary>
        List<GameObject> GetTargets(float detection)
        {
            List<GameObject> targets = GetTaged();
            List<KeyValuePair<GameObject, float>> withDistance = GetDistances(targets);
            List<KeyValuePair<GameObject, float>> InRange = CheckRange(withDistance, detection);
            return InRange.Select(pair => pair.Key).ToList();
        }
        /// <summary>
        /// 발사체의 태그에 해당하는 모든 오브젝트를 반환합니다.
        /// </summary>
        List<GameObject> GetTaged()
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (string clashTag in projectile.GetComponent<Projectile>().clashTags)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(clashTag))
                {
                    targets.Add(go);
                }
            }

            return targets;
        }
        /// <summary>
        /// 타겟 오브젝트들의 거리를 계산하여 거리 순 ASC 정렬 후 반환합니다.
        /// </summary>
        List<KeyValuePair<GameObject, float>> GetDistances(List<GameObject> objects)
        {
            List<KeyValuePair<GameObject, float>> gos = new List<KeyValuePair<GameObject, float>>();

            foreach (GameObject go in objects)
            {
                gos.Add(new KeyValuePair<GameObject, float>(go, Vector3.Distance(transform.position, go.transform.position)));
            }

            return gos.OrderBy(pair => pair.Value).ToList(); ;
        }
        /// <summary>
        /// 사거리 이내의 요소만 반환합니다.
        /// </summary>
        List<KeyValuePair<GameObject, float>> CheckRange(List<KeyValuePair<GameObject, float>> objects, float detection)
        {
            return objects.Where(pair => pair.Value <= detection).ToList();
        }
    }
    /// <summary>
    /// 목적지에서의 직교 벡터를 구합니다.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="destination"></param>
    private void GetOrthogonal(Vector2 start, Vector2 destination)
    {
        Vector2 direction = destination - start;
        Vector2 orthogonalDirection = new Vector2(-direction.y, direction.x);
        Vector2 orthogonalAtDestination = destination + orthogonalDirection;
    }

    /* Test Method */
    [ContextMenu("LaunchTest")]
    private void LaunchTest()
    {
        Launch(Vector3.zero);
    }
}