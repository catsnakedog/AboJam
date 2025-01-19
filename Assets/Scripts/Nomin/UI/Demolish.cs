using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Demolition : MonoBehaviour
{
    /* Dependency */
    /* Field & Property */
    public static Demolition instance; // 싱글턴
    private Promotion promotion => Promotion.instance; // 하드 링크
    private Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    private Tile currentTile => Tile.currentTile; // 하드 링크

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
    /// 최근 타일의 인스턴스를 철거합니다.
    /// </summary>
    /// <param name="matrixCoord">맵의 행렬 좌표</param>
    public void Demolish()
    {
        Grid.instance.Delete((currentTile.i, currentTile.j));
        Off();
    }
}
