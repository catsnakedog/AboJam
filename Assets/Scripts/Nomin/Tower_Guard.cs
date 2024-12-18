using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Guard : MonoBehaviour
{
    Tile motherTile;

    public void Start()
    {
        motherTile = Grid.GetTile(Tile.currentTile.i, Tile.currentTile.j);
        motherTile.SwitchWall(true);
    }

    ~Tower_Guard()
    {
        motherTile.SwitchWall(false);
    }
}
