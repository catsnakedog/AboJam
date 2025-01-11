using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Projectile : MonoBehaviour
{
    /* Dependency */
    public GameObject Explosion { get { return explosion; } private set { explosion = value; } } // 폭발, 없어도 작동
    public Pooling pooling; // 풀링
    public Collider2D colider2D;

    /* Field & Property */
    public static List<Projectile> instances_enable = new List<Projectile>(); // 모든 발사체 인스턴스 (활성화)
    public static List<Projectile> instances_disable = new List<Projectile>(); // 모든 발사체 인스턴스 (비활성화)
    [HideInInspector] public GameObject launcher; // 발사기 참조
    public string[] clashTags; // 충돌 대상 태그
    public float damage = 10f; // 발사체 데미지
    public int penetrate = 1; // 총 관통 수
    private int penetrate_current; // 남은 관통 수

    /* Backing Field */
    [SerializeField] private GameObject explosion;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        if (Explosion != null) pooling.Set(Explosion);
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

            // 폭발 (풀링 or 생성)
            if (Explosion != null)
            {
                GameObject explosion = pooling.Get();
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
    public void SetExplosion(GameObject explosion)
    {
        this.Explosion = explosion;
        pooling.Set(this.Explosion);
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
}
