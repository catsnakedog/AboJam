using System.Linq;
using System.Text;
using UnityEngine;

public class Grid : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spriteRenderer; // 맵

    /* Field & Property */
    public static Grid instance; // 싱글턴
    public int[,] GridIndexMap { get { return gridIndexMap; } } // 간단하게 종류만 나타내는 배열 (pathFind을 위해 추가 - 0116 김도현)
    public int Row { get { return row; } private set { row = value; } } // 행 개수
    public int Column { get { return column; } private set { column = value; } } // 열 개수
    public Vector2 StartPos { get { return startPos; } private set { startPos = value; } } // 맵 왼쪽 위 좌표
    public float Width { get { return width; } private set { width = value; } } // 맵 너비
    public float Height { get { return height; } private set { height = value; } } // 맵 높이
    public float CellWidth { get { return cellWidth; } private set { cellWidth = value; } } // 타일 너비
    public float CellHeight { get { return cellHeight; } private set { cellHeight = value; } } // 타일 높이
    #region Backing Field
    private int[,] gridIndexMap;
    [SerializeField] private int row = 1;
    [SerializeField] private int column = 1;
    private Vector2 startPos;
    private float width;
    private float height;
    private float cellWidth;
    private float cellHeight;
    #endregion

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        StartPos = GetStartPos();
        Width = spriteRenderer.bounds.size.x;
        Height = spriteRenderer.bounds.size.y;
        CellWidth = Width / Column;
        CellHeight = Height / Row;
        gridIndexMap = new int[Row, Column];

        // 타일 생성
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                Tile.instances.Add(new Tile(i, j, GetTileWorldPos(i, j)));
                gridIndexMap[i, j] = (int)EnumData.TileIndex.Empty;
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
    /// <summary>
    /// 지정한 행렬 좌표의 타일을 반환합니다.
    /// </summary>
    public Tile GetTile((int, int) matrixCoord)
    {
        return Tile.instances.Where(tile => tile.i == matrixCoord.Item1 && tile.j == matrixCoord.Item2).FirstOrDefault();
    }
    /// <summary>
    /// 좌표와 가장 가까운 타일을 반환합니다.
    /// </summary>
    /// <param name="innerCheck">타일 안쪽 위치인지 체크</param>
    public Tile GetNearestTile(Vector2 worldPos, bool isInTile = false)
    {
        Tile tile = Tile.instances.OrderBy(tile => Vector2.Distance(tile.pos, worldPos)).FirstOrDefault();

        switch (isInTile)
        {
            case true:
                if (IsInTile(worldPos, tile)) return tile;
                else return null;
            case false:
                return tile;
        }

        /// <summary>
        /// 지정한 위치가 특정 타일 내부인지 여부를 반환합니다.
        /// </summary>
        bool IsInTile(Vector2 pos, Tile tile)
        {
            return Mathf.Abs(pos.x - tile.pos.x) <= cellWidth / 2 && Mathf.Abs(pos.y - tile.pos.y) <= CellHeight / 2;
        }
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
        float xPos = StartPos.x + CellWidth * (j + 0.5f);
        float yPos = StartPos.y - CellHeight * (i + 0.5f);

        return new Vector2(xPos, yPos);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        StringBuilder sb = new();

        for (int i = 0; i < Grid.instance.Row; i++)
        {
            for (int j = 0; j < Grid.instance.Column; j++)
            {
                sb.Append(Grid.instance.GridIndexMap[i, j]);
                sb.Append(" ");
            }
            sb.Append("\n");
        }
        Debug.Log(sb.ToString());
    }
}
