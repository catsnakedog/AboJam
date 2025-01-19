using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFindMonster2 : IPathFind
{
    public int[] Target = new int[] { 1, 2, 3, 4, 5 };

    private Vector3 _targetPos;

    public void MakeRoute(Vector3 characterPos)
    {
        var tile = Grid.instance.GetNearestTile(characterPos);
        var target = FindClosestPoint(Grid.instance.GridIndexMap, tile.i, tile.j, Target);
        if (target == null)
        {
            Debug.Log("타켓이 존재하지 않습니다.");
            return;
        }

        _targetPos = Grid.instance.GetTile(((int, int))target).pos;
    }

    public Vector2 GetDirection(Vector3 characterPos)
    {
        return (_targetPos - characterPos).normalized;
    }

    public (int, int)? FindClosestPoint(int[,] map, int playerX, int playerY, int[] targets)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        int xMinDistance = int.MaxValue;
        int yMinDistance = int.MaxValue;
        int minDistance = int.MaxValue;

        int closestX = -1;
        int closestY = -1;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (!targets.Contains(map[x, y]))
                    continue;
                int xDistance = Math.Abs(playerX - x);
                int yDistance = Math.Abs(playerY - y);

                if (xDistance > xMinDistance && yDistance > yMinDistance)
                    continue;

                int distance = xDistance * xDistance + yDistance * yDistance;
                if (distance > minDistance)
                    continue;

                closestX = x;
                closestY = y;
                minDistance = distance;
            }
        }

        if (closestX == -1)
            return null;
        else
            return (closestX, closestY);
    }
}