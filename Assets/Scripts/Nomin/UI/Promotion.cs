using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EnumData;

public class Promotion : MonoBehaviour
{
    /* Dependency */
    public GameObject button; // 하이라키 연결
    private Abocado currentAbocado => Abocado.currentAbocado; // 하드 링크

    /* Field & Property */
    public static Promotion instance; // 싱글턴
    private string path_prefabs = "Prefabs/Entity/Towers/"; // 타워 프리팹 Resources 경로
    private string path_images = "Images/UI/Promotion/"; // 타워 이미지 Resources 경로

    /* Intializer & Finalizer & Updater */
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
            img.sprite = Resources.Load<Sprite>(path_images + towerType);
            if (img.sprite == null) Debug.Log($"{path_images}{towerType} 에 이미지가 존재하지 않습니다.");
            try { text.text = StaticData.text_promotion[towerType]; }
            catch { Debug.Log($"StaticData.text_promotion 에 {towerType} 설명이 존재하지 않습니다."); }
        }
    }
    public void Awake()
    {
        instance = this;
        Init();
        gameObject.SetActive(false);
    }
    public void On()
    {
        Reinforcement.instance.Off();
        instance.gameObject.SetActive(true);
    }
    public void Off()
    {
        instance.gameObject.SetActive(false);
    }

    /* Private Method */
    /// <summary>
    /// <br>최근 선택된 타일의 아보카도를 타워로 업그레이드합니다.</br>
    /// </summary>
    /// <param name="towerName">(EnumData.Tower)towerName</param>
    private void Promote(EnumData.TowerType towerType)
    {
        // 아보카도 품질 증강
        if (towerType == EnumData.TowerType.Production)
        {
            currentAbocado.Promote();
            gameObject.SetActive(false);
            return;
        }

        // 타워 프리팹 불러오기
        GameObject go_tower = Resources.Load<GameObject>(path_prefabs + towerType);
        if (go_tower == null) { Debug.Log(path_prefabs + towerType + " 에 타워 프리팹이 없습니다."); return; }

        // 아보카도 제거 & 타워 건설
        Tile currentTile = Grid.instance.GetNearestTile(currentAbocado.gameObject.transform.position);
        currentTile.Delete();
        currentTile.Create(go_tower);
        gameObject.SetActive(false);
    }
}