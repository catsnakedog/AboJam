using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public Canvas canv;
    public GridLayoutGroup grid;
    public static GameObject obj;
    public static float cellSize;
    public static float spacing;

    // 행의 개수 i, 열의 개수 j
    public static int i, j;

    private void Awake()
    {
        transform.position = Vector3.zero;
        Grid.obj = gameObject;
        cellSize = grid.cellSize.x;
        spacing = grid.spacing.x;
        i = (int)((canv.GetComponent<RectTransform>().rect.height + spacing) / (cellSize + spacing));
        j = (int)((canv.GetComponent<RectTransform>().rect.width + spacing) / (cellSize + spacing));
    }

    /// <summary>
    /// <br>[i][j] 을 인덱스화 합니다.</br>
    /// </summary>
    public static int ConvertArrayToIndex(int i, int j)
    {
        int index = Grid.j * (i - 1) + j - 1;

        return index;
    }

    /// <summary>
    /// <br>인덱스를 [i][j] 로 변경합니다.</br>
    /// </summary>
    public static (int i, int j) ConvertIndexToArray(int index)
    {
        // 행과 열 인덱스 계산
        int i = index / Grid.j + 1;
        int j = index % Grid.j + 1;

        return (i, j);
    }

    /// <summary>
    /// <br>타일 [i][j] 을 반환합니다.</br>
    /// </summary>
    public static Tile GetBlock(int i, int j)
    {
        try
        {
            return obj.transform.GetChild(ConvertArrayToIndex(i, j)).gameObject.GetComponent<Tile>();
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
