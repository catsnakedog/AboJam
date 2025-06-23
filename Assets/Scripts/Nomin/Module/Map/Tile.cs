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
using static EnumData;

public class Tile
{
    /* Dependency */
    public Pool pool => Pool.instance;
    private Grid grid => Grid.instance;
    private Promotion promotion => Promotion.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Farming farming => Farming.instance;

    /* Field & Property */
    public static List<Tile> instances = new List<Tile>(); // 모든 타일 인스턴스
    public static Tile currentTile; // 최근 선택된 타일
    public static int boundaryRestriction = 3; // 외곽 건설 금지
    public GameObject Go { get; private set; } = null; // 타일에 배치된 프리팹
    public readonly int i;
    public readonly int j;
    public bool isWall;
    public readonly Vector2 pos;

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
        if (IsNearMapBoundary(this, boundaryRestriction)) return;

        promotion.Off();
        reinforcement.Off();
        currentTile = this;

        // 경작 시작
        farming.Cultivate(this);
    }
    /// <summary>
    /// 지정한 오브젝트를 타일에 바인딩 합니다.
    /// </summary>
    public void Bind(GameObject obj, EnumData.TileIndex tileIndex)
    {
        Go = obj;
        Go.transform.position = pos;
        grid.GridIndexMap[i, j] = (int)tileIndex;
    }
    /// <summary>
    /// 바인딩을 해제합니다.
    /// </summary>
    public void UnBind()
    {
        Go = null;
        grid.GridIndexMap[i, j] = (int)EnumData.TileIndex.Empty;
    }

    /// <summary>
    /// 타일이 맵 외곽 n 타일 이내인지 검사합니다. (체비셰프 거리)
    /// </summary>
    public static bool IsNearMapBoundary(Tile tile, int n)
    {
        int distLeft = tile.i;
        int distRight = Grid.instance.Column - 1 - tile.i;
        int distTop = tile.j;
        int distBottom = Grid.instance.Row - 1 - tile.j;

        int distToEdge = Math.Min(Math.Min(distLeft, distRight), Math.Min(distTop, distBottom));

        return distToEdge < n;
    }
}