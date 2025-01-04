using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spriteRenderer; // 맵

    /* Field & Property */
    public static Grid instance; // 싱글턴
    public int row = 1; // 행 개수
    public int column = 1; // 열 개수
    private Vector2 startPos; // 맵 왼쪽 위 좌표
    private float width; // 맵 너비
    private float height; // 맵 높이
    private float cellWidth; // 타일 너비
    private float cellHeight; // 타일 높이

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        startPos = GetStartPos();
        width = spriteRenderer.bounds.size.x;
        height = spriteRenderer.bounds.size.y;
        cellWidth = width / column;
        cellHeight = height / row;

        // 타일 생성
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Tile.instances.Add(new Tile(i, j, GetTileWorldPos(i, j)));
            }
        }
    }

    /* Public Method */
    /// <summary>
    /// 마우스 좌표에 해당하는 타일의 OnClick 메서드를 실행시킵니다.
    /// </summary>
    public void OnClick()
    {
        GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition)).OnClick();
    }

    /* Private Method */
    /// <summary>
    /// 맵의 맨 왼쪽 위 꼭짓점 좌표를 반환합니다.
    /// </summary>
    private Vector2 GetStartPos()
    {
        Bounds bounds = spriteRenderer.bounds;
        return new Vector2(bounds.min.x, bounds.max.y);
    }
    /// <summary>
    /// 타일의 월드 좌표를 반환합니다.
    /// </summary>
    private Vector2 GetTileWorldPos(int i, int j)
    {
        float xPos = startPos.x + cellWidth * (j + 0.5f);
        float yPos = startPos.y - cellHeight * (i + 0.5f);

        return new Vector2(xPos, yPos);
    }
    /// <summary>
    /// 월드 좌표와 가장 가까운 타일을 반환합니다.
    /// </summary>
    /// <returns></returns>
    private Tile GetNearestTile(Vector2 worldPos)
    {
        return Tile.instances.OrderBy(tile => Vector2.Distance(tile.pos, worldPos)).FirstOrDefault();
    }
}
