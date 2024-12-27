using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    /* Dependency */
    public Canvas canvas; // 하이라키 연결
    public GridLayoutGroup gridLayoutGroup; // 하이라키 연결

    /* Field & Property */
    public static Grid instance; // 싱글턴
    public float CellSize { get; private set; } // 한 셀 너비
    public float Spacing { get; private set; } // 셀 사이 공간
    public int I { get; private set; }
    public int J { get; private set; }

    /* Intializer & Finalizer */
    private void Awake()
    {
        instance = this;
        transform.position = Vector3.zero;
        CellSize = gridLayoutGroup.cellSize.x;
        Spacing = gridLayoutGroup.spacing.x;
        I = (int)((canvas.GetComponent<RectTransform>().rect.height + Spacing) / (CellSize + Spacing));
        J = (int)((canvas.GetComponent<RectTransform>().rect.width + Spacing) / (CellSize + Spacing));
    }

    /* Public Method */
    /// <summary>
    /// <br>그리드의 [i][j] 오브젝트를 반환합니다.</br>
    /// </summary>
    public GameObject GetObject(int i, int j)
    {
        return gameObject.transform.GetChild(ConvertArrayToIndex(i, j)).gameObject;
    }
    /// <summary>
    /// <br>[i][j] 을 인덱스화 합니다.</br>
    /// </summary>
    public int ConvertArrayToIndex(int i, int j)
    {
        int index = j * (i - 1) + j - 1;

        return index;
    }
    /// <summary>
    /// <br>인덱스를 [i][j] 로 변경합니다.</br>
    /// </summary>
    public (int i, int j) ConvertIndexToArray(int index)
    {
        // 행과 열 인덱스 계산
        int i = index / J + 1;
        int j = index % J + 1;

        return (i, j);
    }
}
