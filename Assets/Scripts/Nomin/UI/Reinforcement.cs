using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Reinforcement : MonoBehaviour
{
    /* Dependency */
    private Tower currentTower => Tower.currentTower; // 하드 링크
    public Button BTN_reinforce;
    public Image IMG_tower;
    public TextMeshProUGUI TMP_explain;
    public TextMeshProUGUI TMP_level;
    public TextMeshProUGUI TMP_price;

    /* Field & Property */
    public static Reinforcement instance; // 싱글턴
    private string path_images = "Images/UI/Reinforcement/"; // 타워 이미지 Resources 경로

    /* Intializer & Finalizer & Updater */
    private void Init()
    {
        // TowerType 추론
        string towerTypeString = currentTower.GetType().ToString();
        EnumData.TowerType towerType = (EnumData.TowerType)Enum.Parse(typeof(EnumData.TowerType), towerTypeString);

        // 타워 이미지 & 설명 표기
        IMG_tower.sprite = Resources.Load<Sprite>($"{path_images}{towerTypeString}/Level{currentTower.Level}");
        if (IMG_tower.sprite == null) Debug.Log($"{path_images}{towerTypeString}/Level{currentTower.Level} 이미지가 존재하지 않습니다.");
        try { TMP_explain.text = StaticData.text_reinforcement[towerType]; }
        catch { Debug.Log($"StaticData.text_reinforcement 에 {towerType} 설명이 존재하지 않습니다."); }

        // 레벨 & 증강 가격 표기
        TMP_level.text = $"Lv. {(currentTower.Level + 1).ToString()}";
        if (currentTower.Level == currentTower.MaxLevel) TMP_price.text = "MAX";
        else TMP_price.text = currentTower.ReinforceCost[currentTower.Level].ToString();
    }
    public void Awake()
    {
        instance = this;
        BTN_reinforce.onClick.AddListener(Reinforce);
        gameObject.SetActive(false);
    }
    public void On()
    {
        Init();
        Promotion.instance.Off();
        gameObject.SetActive(true);
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }

    /* Public Method */
    /// <summary>
    /// <br>최근 선택된 타워를 증강합니다.</br>
    /// </summary>
    public void Reinforce()
    {
        currentTower.Reinforce();
        Off();
    }
}