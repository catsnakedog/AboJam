using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject tower;
    public int cost;

    public void TowerUpgrade()
    {
        if (StaticData.Garu >= cost)
        {
            StaticData.Garu -= cost;
            Tile.currentTile.Delete();
            Tile.currentTile.Create(tower);
            transform.parent.gameObject.SetActive(false);
        }
    }
}