using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    /* Dependency */
    private Tile currentTile => Tile.currentTile; // 하드 링크
    public GameObject tower;
    public int cost;

    /* Field */
    public static Upgrade instance; // 싱글턴
    private string path = "Prefabs/Tower/"; // 타워 프리팹 Resources 경로

    /* Intializer & Finalizer */
    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    /* Public Method */

    /// <summary>
    /// <br>최근 선택된 타일의 아보카도를 타워로 업그레이드합니다.</br>
    /// </summary>
    /// <param name="towerName">(EnumData.Tower)towerName</param>
    public void Promotion(string towerName)
    {
        // towerName 이 EnumData.Tower 에 존재하는지 검사
        try { EnumData.Tower tower = (EnumData.Tower)Enum.Parse(typeof(EnumData.Tower), towerName); }
        catch { Debug.Log($"EnumData.Tower 에 {towerName} 타워가 없습니다."); return; }

        // 해당 타워 프리팹 불러오기
        GameObject go_tower = Resources.Load<GameObject>(path + towerName);
        if (go_tower == null) { Debug.Log(path + towerName + "에 타워 프리팹이 없습니다."); return; }

        // 아보카도 제거 & 타워 건설
        currentTile.Delete();
        currentTile.Create(go_tower);
    }

    /* Private Method */




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