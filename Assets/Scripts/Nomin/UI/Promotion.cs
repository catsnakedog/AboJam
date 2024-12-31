using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Promotion : MonoBehaviour
{
    /* Dependency */
    public GameObject button; // 하이라키 연결
    private Tile currentTile => Tile.currentTile; // 하드 링크

    /* Field & Property */
    public static Promotion instance; // 싱글턴
    private string path_prefabs = "Prefabs/Entity/Towers/"; // 타워 프리팹 Resources 경로
    private string path_images = "Images/UI/Promotion/"; // 타워 프리팹 Resources 경로

    /* Intializer & Finalizer & Updater */
    public void Awake()
    {
        instance = this;
        Init();
        gameObject.SetActive(false);
    }
    private void Init()
    {
        List<GameObject> list_go = new List<GameObject>();

        // 타워 종류마다 업그레이드 버튼 생성
        foreach (EnumData.TowerType towerType in Enum.GetValues(typeof(EnumData.TowerType)))
        {
            // 컴포넌트 연결
            list_go.Add(Instantiate(button, transform));
            GameObject currentObject = list_go[^1];
            Button btn = currentObject.GetComponent<Button>();
            Image img = currentObject.GetComponent<Image>();
            TextMeshProUGUI text = currentObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            // 컴포넌트 설정
            btn.onClick.AddListener(() => Promote(towerType));
            try { img.sprite = Resources.Load<Sprite>(path_images + towerType); }
            catch { Debug.Log($"{path_images}{towerType} 에 이미지가 존재하지 않습니다."); }
            try { text.text = StaticData.text_promotion[towerType]; }
            catch { Debug.Log($"StaticData.text_promotion 에 {towerType} 설명이 존재하지 않습니다."); }
        }
    }

    /* Private Method */
    /// <summary>
    /// <br>최근 선택된 타일의 아보카도를 타워로 업그레이드합니다.</br>
    /// </summary>
    /// <param name="towerName">(EnumData.Tower)towerName</param>
    private void Promote(EnumData.TowerType towerType)
    {
        // 해당 타워 프리팹 불러오기
        GameObject go_tower = Resources.Load<GameObject>(path_prefabs + towerType);
        if (go_tower == null) { Debug.Log(path_prefabs + towerType + " 에 타워 프리팹이 없습니다."); return; }

        // 아보카도 제거 & 타워 건설
        currentTile.Delete();
        currentTile.Create(go_tower);
        gameObject.SetActive(false);
    }
}