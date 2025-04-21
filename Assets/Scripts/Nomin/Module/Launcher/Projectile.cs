using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Cryptography.X509Certificates;
using Unity.Properties;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Projectile : RecordInstance<Table_Projectile, Record_Projectile>, IPoolee
{
    /* Dependency */
    public GameObject explosion; // 폭발, 없어도 작동
    public Pool pool => Pool.instance;
    public Collider2D colider2D;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스
    private Upgrade upgrade => Upgrade.instance;

    /* Field & Property */
    public static List<Projectile> instances = new List<Projectile>(); // 모든 발사체 인스턴스
    [HideInInspector] public GameObject launcher; // 발사기 참조
    [SerializeField] private string[] clashTags; public string[] ClashTags { get { Start(); return clashTags; } set => clashTags = value; } // 충돌 대상 태그
    public float damage = 10f; // 발사체 데미지
    public float multiplierDamage = 1.0f; // 데미지 계수
    public int penetrate = 1; // 총 관통 수
    private int penetrate_current; // 남은 관통 수

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        startFlag = false;
    }
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();
        instances.Add(this);

        // 예외처리
        if (penetrate < 1)
        {
            Debug.Log($"{name} 의 Projectile 의 penetrate 이 너무 작아 1 로 설정되었습니다.");
            penetrate = 1;
        }
        if (clashTags.Length == 0) Debug.Log($"{name} 의 Projectile 의 충돌 대상 태그가 할당되지 않았습니다.");
        if (colider2D == null) Debug.Log($"{name} 의 Projectile 에서 colider 가 설정되지 않았습니다.");
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    private void OnEnable()
    {
        // Load();
    } // 캐릭터용 임시 Load 입니다. 나중에 지우고, 캐릭터 불릿의 풀링을 바꿔야 합니다.
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportProjectile(initialRecords[0].ID, ref clashTags, ref damage, ref penetrate);

        penetrate_current = penetrate;
        if(upgrade != null) upgrade.ApplyRanged(this);
    } // 풀에서 꺼낼 때 / Import 시 자동 실행
    public void Save()
    {

    } // 풀에 집어 넣을 때 자동 실행

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

            // 힐 모드 (damage < 0) 인데 최대 체력인 대상은 충돌 대상에서 제외
            HP targetHP = HP.FindHP(target);
            if (damage < 0 && targetHP.HP_current == targetHP.Hp_max) return;

            // 폭발 (풀링 or 생성)
            if (explosion != null)
            {
                GameObject explosion = pool.Get(this.explosion.name);
                explosion.transform.position = transform.position;
                explosion.GetComponent<Explosion>().Explode(clashTags);
            }

            // 타겟 HP 에 데미지 / 회복 연산
            if (damage >= 0) targetHP.Damage(damage * multiplierDamage);
            else targetHP.Heal(-damage);

            // 관통 게산
            penetrate_current--;
            if (penetrate_current == 0) pool.Return(gameObject);
        }
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
