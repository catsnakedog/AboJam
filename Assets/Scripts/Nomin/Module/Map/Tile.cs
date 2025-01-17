using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Tile
{
    /* Field & Property */
    public static List<Tile> instances = new List<Tile>(); // 모든 타일 인스턴스
    public static Tile currentTile; // 최근 선택된 타일
    public GameObject Go { get; set; } = null; // 타일에 배치된 프리팹
    public readonly int i;
    public readonly int j;
    public bool isWall;
    public readonly Vector2 pos;
    private string path_abocado = "Prefabs/Entity/Abocado";

    /* Intializer & Finalizer & Updater */
    public Tile(int i, int j, Vector2 pos)
    {
        instances.Add(this);
        this.i = i;
        this.j = j;
        this.pos = pos;
    }

    /* Public Method */
    /// <summary>
    /// <br>타일을 클릭했을 때 발생하는 이벤트 입니다.</br>
    /// <br>설치된 오브젝트로 이벤트가 구분됩니다.</br>
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"타일 [" + i + "][" + j + "] 이 클릭되었습니다.");
        Promotion.instance.Off();
        Reinforcement.instance.Off();
        currentTile = this;

        // NULL : Abocado 프리팹 건설
        if (Go == null) Create(Resources.Load<GameObject>(path_abocado), EnumData.TileIndex.AboCado);
    }
    /// <summary>
    /// 현재 타일 위치에 지정한 프리팹을 생성합니다.
    /// </summary>
    public void Create(GameObject Go, EnumData.TileIndex tileIndex)
    {
        Go.transform.position = pos;
        Grid.instance.Create((i, j), Go);
        Grid.instance.GridIndexMap[i, j] = (int)tileIndex;
    }
    /// <summary>
    /// 현재 타일 위치에 지정한 프리팹을 바인딩 합니다.
    /// </summary>
    public void Bind(GameObject Go, EnumData.TileIndex tileIndex)
    {
        Go.transform.position = pos;
        Grid.instance.Bind((i, j), Go);
        Grid.instance.GridIndexMap[i, j] = (int)tileIndex;
    }
    /// <summary>
    /// 타일에 존재하는 프리팹을 제거합니다.
    /// </summary>
    public void Delete()
    { 
        Grid.instance.Delete((i, j));
    }
}