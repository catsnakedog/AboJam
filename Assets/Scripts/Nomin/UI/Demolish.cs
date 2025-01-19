using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Demolition : MonoBehaviour
{
    /* Dependency */
    private Promotion promotion => Promotion.instance; // 하드 링크
    private Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    private Pool pool => Pool.instance; // 하드 링크
    private Tile currentTile => Tile.currentTile; // 하드 링크

    /* Field & Property */
    public static Demolition instance; // 싱글턴

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public void On()
    {
        reinforcement.Off();
        promotion.Off();
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// 최근 타일의 오브젝트를 풀로 반환합니다.
    /// </summary>
    public void Demolish()
    {
        pool.Return(currentTile.Go);
        Off();
    }
}
