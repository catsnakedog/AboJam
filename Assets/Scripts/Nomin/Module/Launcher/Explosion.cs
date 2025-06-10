using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : RecordInstance<Table_Explosion, Record_Explosion>, IPoolee
{
    /* Dependency */
    public GameObject light2D; // 빛, 없어도 작동
    public Targeter targeter; // 조준경
    public Pool pool => Pool.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance; // 런타임 데이터베이스

    /* Field & Property */
    public static List<Explosion> instances = new List<Explosion>();
    public float radius = 5f; // 폭발 데미지 범위
    public float damage = 3f; // 폭발 데미지
    public float time = 2f; // 폭발 시간
    private Coroutine lastCor;
    public static Action<string, Vector2> eventExplode;

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportExplosion(initialRecords[0].ID, out Vector3 scale, ref radius, ref damage, ref time);

        transform.localScale = scale;
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save() { } // 풀에 집어 넣을 때 자동 실행

    /* Public Method */
    /// <summary>
    /// 폭발합니다.
    /// </summary>
    /// <param name="tags">피해를 입힐 오브젝트의 태그</param>
    /// <param name="seconds">이펙트 재생 시간</param>
    public void Explode(string[] tags)
    {
        if(initialRecords[0].ID == "Explosion_Meteor") eventExplode.Invoke(initialRecords[0].ID, gameObject.transform.position);
        SplashDamage(tags, radius);
        if (lastCor != null) StopCoroutine(lastCor);
        lastCor = StartCoroutine(CorOn());

        // 빛 (풀링 or 생성)
        if (light2D != null)
        {
            GameObject light2D = pool.Get(this.light2D.name);
            light2D.transform.position = transform.position;
        }
    }

    /* Private Method */
    /// <summary>
    /// 일정 시간동안 이펙트를 보입니다.
    /// </summary>
    /// <param name="seconds">이펙트 유지 시간</param>
    /// <returns></returns>
    private IEnumerator CorOn()
    {
        yield return new WaitForSeconds(time);
        pool.Return(gameObject);
        lastCor = null;
    }
    /// <summary>
    /// 일정 거리 이내, 특정 태그의 타겟에게 데미지를 입힙니다.
    /// </summary>
    /// <param name="distance"></param>
    private void SplashDamage(string[] tags, float radius)
    {
        List<GameObject> targets = targeter.GetTargets(tags, radius);
        foreach (GameObject target in targets) HP.FindHP(target).Damage(damage);
    }
}
