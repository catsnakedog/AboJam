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

    /* Field & Property */
    public static List<Projectile> instances_enable = new List<Projectile>(); // 모든 발사체 인스턴스 (활성화)
    public static List<Projectile> instances_disable = new List<Projectile>(); // 모든 발사체 인스턴스 (비활성화)
    public string[] clashTags; // 충돌 대상 태그
    public float damage = 10f; // 발사체 데미지
    public int penetrate = 1; // 총 관통 수
    private int penetrate_current; // 남은 관통 수

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
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
            HP.FindHP(target).Damage(damage);
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

    /* Private Method */
    /// <summary>
    /// 충돌을 감지합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Clash(collision.gameObject);
    }
}
