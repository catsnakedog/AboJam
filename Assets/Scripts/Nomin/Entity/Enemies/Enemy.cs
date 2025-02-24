using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy<T1, T2> : RecordInstance<T1, T2>, IEnemy 
    where T1 : ITable
    where T2 : IRecord
{
    /* Dependency */
    public HP hp;
    public Movement movement;
    protected Pool pool => Pool.instance; // 하드 링크
    protected Grid grid => Grid.instance; // 하드 링크

    /* Field & Property */
    public static List<IEnemy> instances => IEnemy.instances;
    public static IEnemy currentEnemy => IEnemy.currentEnemy;
    public int Level { get; set; } = 0; // 현재 레벨
    public int MaxLevel { get; private set; } // 최대 레벨

    /* Intializer & Finalizer & Updater */
    public virtual void Start()
    {
        base.Start();
        instances.Add(this);
        Load();

        hp.death.AddListener(() => StartCoroutine(CorDeath(0.3f)));
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    public virtual void Load()
    {
        Level = 0;
        hp.Load();
    } // 풀에서 꺼낼 때 또는 Database 에서 로드 시 자동 실행
    public void Save()
    {
        grid.GetNearestTile(gameObject.transform.position).UnBind();
    } // 풀에 집어 넣을 때 자동 실행
    /// <summary>
    /// 천천히 죽음을 맞이합니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CorDeath(float time)
    {
        StaticData.gameData.kill++;
        string originTag = gameObject.tag;
        gameObject.tag = "Untagged";

        // 투명화 대상 스프라이트
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        float[] startAlpha = new float[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++) startAlpha[i] = spriteRenderers[i].color.a;

        // 투명도 새로고침 간격
        float delay = 0.01f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);

        // 투명화
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += delay;
            float ratio = elapsedTime / time;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                float alpha = Mathf.Lerp(startAlpha[i], 0f, ratio);
                spriteRenderers[i].color = new UnityEngine.Color(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, alpha);
            }

            yield return waitForSeconds;
        }

        // 투명도 복원
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = new UnityEngine.Color(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, startAlpha[i]);
        }

        gameObject.tag = originTag;
        pool.Return(gameObject);
    }

    /* Public Method */
    /// <summary>
    /// 적 클릭 시 상호작용 입니다.
    /// </summary>
    public void OnClick()
    {
        IEnemy.currentEnemy = this;
    }
    /// <summary>
    /// <br>적을 증강합니다.</br>
    /// <br>공통 증강만 정의되어 있으며, 개별 증강은 자식 스크립트에서 구현됩니다.</br>
    /// </summary>
    public virtual void Reinforce()
    {
        hp.Heal(hp.Hp_max);
    }
}