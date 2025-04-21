using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static EnumData;
using static Hand;
using static ObjectPool;

public class Promotion : MonoBehaviour
{
    /* Dependency */
    public GameObject button; // 하이라키 연결
    private Abocado currentAbocado => Abocado.currentAbocado; // 하드 링크
    private Message message => Message.instance; // 하드 링크
    private Reinforcement reinforcement => Reinforcement.instance; // 하드 링크
    private Demolition demolition => Demolition.instance; // 하드 링크
    private Pool pool => Pool.instance; // 하드 링크
    private Grid grid => Grid.instance; // 하드 링크

    /* Field & Property */
    public static Promotion instance; // 싱글턴
    private string path_images = "Images/UI/Promotion/"; // 타워 이미지 Resources 경로
    public int price_guard = 1;
    public int price_auto = 2;
    public int price_production = 3;
    public int price_heal = 4;
    public int price_splash = 5;
    public event Action<EnumData.TowerType, int> eventPromote;

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
            TextMeshProUGUI TMP_explain = currentObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // 컴포넌트 설정
            btn.onClick.AddListener(() => Promote(towerType));
            img.sprite = Resources.Load<Sprite>(path_images + towerType);
            if (img.sprite == null) Debug.Log($"{path_images}{towerType} 에 이미지가 존재하지 않습니다.");
            try { text.text = StaticData.text_promotion[towerType]; }
            catch { Debug.Log($"StaticData.text_promotion 에 {towerType} 설명이 존재하지 않습니다."); }
            TMP_explain.text = GetPrice(towerType).ToString();
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
        reinforcement.Off();
        demolition.Off();
        instance.gameObject.SetActive(true);
    }
    public void Off()
    {
        instance.gameObject.SetActive(false);
    }

    /* Private Method */
    /// <summary>
    /// <br>최근 선택된 아보카도를 타워로 업그레이드합니다.</br>
    /// </summary>
    /// <param name="towerName">(EnumData.Tower)towerName</param>
    private void Promote(EnumData.TowerType towerType)
    {
        if (currentAbocado.hp.HP_current <= 0) { message.On("아보카도가 파괴되어 타워를 건설할 수 없습니다.", 2f); return; }

        // 결제
        int price = GetPrice(towerType);
        if (StaticData.Garu >= price)
        {
            message.On("타워가 건설되었습니다.", 2f);
            StaticData.Garu -= price;
            StaticData.gameData.tower++;
            eventPromote?.Invoke(towerType, price);
        }
        else { message.On("가루가 부족합니다.", 2f); return; };

        // 아보카도 품질 증강
        if (towerType == EnumData.TowerType.Production)
        {
            currentAbocado.Promote();
            gameObject.SetActive(false);
            return;
        }

        // 타일 인덱스 분별
        TileIndex tileIndex = towerType switch
        {
            TowerType.Production => TileIndex.AboCado,
            TowerType.Auto => TileIndex.Auto,
            TowerType.Splash => TileIndex.Splash,
            TowerType.Heal => TileIndex.Heal,
            TowerType.Guard => TileIndex.Guard,
            _ => throw new NotImplementedException()
        };

        // 건설할 타워를 풀에서 Get & 바인딩
        gameObject.SetActive(false);
        pool.Return(currentAbocado.gameObject);
        grid.GetNearestTile(currentAbocado.transform.position).Bind(pool.Get(towerType.ToString()), tileIndex);
    }
    /// <summary>
    /// 타워 타입에 맞는 가격을 반환합니다.
    /// </summary>
    private int GetPrice(EnumData.TowerType towerType)
    {
        switch (towerType)
        {
            case TowerType.Guard: return price_guard;
            case TowerType.Auto: return price_auto;
            case TowerType.Splash: return price_splash;
            case TowerType.Production: return price_production;
            case TowerType.Heal: return price_heal;
        }

        return 0;
    }
}