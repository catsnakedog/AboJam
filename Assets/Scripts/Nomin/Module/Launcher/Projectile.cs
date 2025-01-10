using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /* Dependency */
    public Collider2D colider2D;
    public GameObject explosion; // 없어도 작동

    /* Field & Property */
    public static List<Projectile> instances_enable = new List<Projectile>(); // 모든 발사체 인스턴스 (활성화)
    public static List<Projectile> instances_disable = new List<Projectile>(); // 모든 발사체 인스턴스 (비활성화)
    [HideInInspector] public GameObject launcher; // 발사기 참조
    public List<GameObject> pool { get; private set; } = new List<GameObject>(); // 폭발 풀링
    public GameObject pool_root { get; private set; }
    public string[] clashTags; // 충돌 대상 태그
    public float damage = 10f; // 발사체 데미지
    public int penetrate = 1; // 총 관통 수
    private int penetrate_current; // 남은 관통 수

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        pool_root = GameObject.Find("@Pooling") ?? new GameObject("@Pooling");
    }
    private void Start()
    {
        // 예외처리
        if (penetrate < 1)
        {
            Debug.Log($"{name} 의 Projectile 의 penetrate 이 너무 작아 1 로 설정되었습니다.");
            penetrate = 1;
        }
        if (clashTags.Length == 0) Debug.Log($"{name} 의 Projectile 의 충돌 대상 태그가 할당되지 않았습니다.");
        if (colider2D == null) Debug.Log($"{name} 의 Projectile 에서 colider 가 설정되지 않았습니다.");
    }
    private void OnEnable()
    {
        penetrate_current = penetrate;
        instances_enable.Add(this);
        instances_disable.Remove(this);
    }
    private void OnDisable()
    {
        instances_enable.Remove(this);
        instances_disable.Add(this);
    }
    private void OnDestroy()
    {
        foreach (var item in pool) Destroy(item); // 생성된 폭발 제거
        instances_enable.Remove(this);
        instances_disable.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 타겟과 충돌 시 실행됩니다.
    /// </summary>
    /// <param name="target">충돌 대상</param>
    public void Clash(GameObject target)
    {
        // 타겟의 태그가 clashTags 에 존재해야 충돌
        if (Array.Exists(clashTags, tag => tag == target.tag))
        {
            // 발사기를 포함한 조상 오브젝트는 충돌 대상에서 제외
            if (launcher != null) if (GetParentList(launcher.transform).Contains(target)) return;

            if (explosion != null)
            {
                // 폭발 (풀링 or 생성)
                GameObject explosion = SearchPool() ?? Create();
                explosion.transform.position = transform.position;
                explosion.GetComponent<Explosion>().Explode(clashTags);
            }

            // 타겟 HP 에 데미지 / 회복 연산
            if (damage >= 0) HP.FindHP(target).Damage(damage);
            else HP.FindHP(target).Heal(-damage);

            // 관통 게산
            penetrate_current--;
            if (penetrate_current == 0) Disappear();
        }
    }
    /// <summary>
    /// 발사체를 비활성화합니다.
    /// </summary>
    public void Disappear()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 폭발을 변경합니다.
    /// </summary>
    /// <param name="explosion"></param>
    public void SetProjectile(GameObject explosion)
    {
        this.explosion = explosion;
        foreach (var item in pool) Destroy(item);
        pool.Clear();
    }

    /* Private Method */
    /// <summary>
    /// 충돌을 감지합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Clash(collision.gameObject);
    }
    /// <summary>
    /// 특정 오브젝트를 포함한 모든 조상 오브젝트를 리스트로 반환합니다.
    /// </summary>
    /// <returns>조상 오브젝트 리스트</returns>
    private List<GameObject> GetParentList(Transform transform)
    {
        List<GameObject> parents = new List<GameObject>();
        Transform current = transform;

        while (current != null)
        {
            parents.Add(current.gameObject);
            current = current.parent;
        }

        return parents;
    }
    /// <summary>
    /// 폭발을 새로 생성합니다.
    /// </summary>
    /// <returns>새로 생성된 폭발 입니다.</returns>
    private GameObject Create()
    {
        GameObject explosion = Instantiate(this.explosion, pool_root.transform); ;

        pool.Add(explosion);
        return explosion;
    }
    /// <summary>
    /// <br>pool 에서 비활성화된 폭발을 찾습니다.</br>
    /// </summary>
    /// <returns>
    /// <br>pool 에서 사용 가능한 발사체 입니다.</br>
    /// <br>사용 가능한 폭발이 없으면 null 을 반환합니다.</br>
    /// </returns>
    private GameObject SearchPool()
    {
        GameObject go = null;

        // pool 에서 비활성화된 폭발을 찾습니다.
        foreach (var explosion in pool)
        {
            if (explosion.activeSelf == false)
            {
                go = explosion;
                go.SetActive(true);
                break;
            }
        }

        return go;
    }
}
