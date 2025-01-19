using System;
using System.Collections.Generic;

public class GridProcessor
{
    public static int[,] IndexingGrid(int[,] grid)
    {
        int row = grid.GetLength(0);
        int col = grid.GetLength(1);
        int[,] result = new int[row, col];
        Array.Copy(grid, result, grid.Length); 
        int regionIndex = 10;

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        // BFS 전염 처리
        void InfectArea(int startX, int startY)
        {
            Queue<(int, int)> queue = new();
            queue.Enqueue((startX, startY));
            result[startX, startY] = regionIndex;

            while (queue.Count > 0)
            {
                var (cx, cy) = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int nx = cx + dx[i];
                    int ny = cy + dy[i];

                    if (nx >= 0 && nx < row && ny >= 0 && ny < col)
                    {
                        if (result[nx, ny] == 0 || result[nx, ny] == 1)
                        {
                            result[nx, ny] = regionIndex;
                            queue.Enqueue((nx, ny));
                        }
                    }
                }
            }
        }

        // 전체 맵 탐색
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if ((result[i, j] == 0 || result[i, j] == 1))
                {
                    InfectArea(i, j);
                    regionIndex++; // 새로운 구역 인덱스 증가
                }
            }
        }

        return result;
    }
}