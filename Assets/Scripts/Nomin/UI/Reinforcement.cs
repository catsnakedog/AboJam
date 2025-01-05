using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reinforcement : MonoBehaviour
{
    /* Dependency */
    private Tower currentTower => Tower.currentTower; // 하드 링크
    public Button button;
    public Image image;
    public TextMeshProUGUI tmp;

    /* Field & Property */
    public static Reinforcement instance; // 싱글턴
    private string path_images = "Images/UI/Reinforcement/"; // 타워 이미지 Resources 경로

    /* Intializer & Finalizer & Updater */
    private void Init()
    {
        // TowerType 추론
        string towerTypeString = currentTower.GetType().ToString();
        EnumData.TowerType towerType = (EnumData.TowerType)Enum.Parse(typeof(EnumData.TowerType), towerTypeString);

        // 이미지 & 설명 초기화
        image.sprite = Resources.Load<Sprite>(path_images + towerTypeString);
        if (image.sprite == null) Debug.Log($"{path_images}{towerTypeString} 에 이미지가 존재하지 않습니다.");
        try { tmp.text = StaticData.text_reinforcement[towerType]; }
        catch { Debug.Log($"StaticData.text_reinforcement 에 {towerType} 설명이 존재하지 않습니다."); }
    }
    public void Awake()
    {
        instance = this;
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