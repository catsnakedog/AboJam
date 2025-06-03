using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class BTN_Upgrade : RecordInstance<Table_Upgrade, Record_Upgrade>
{
    /* Dependency */
    private Database_AboJam database_abojam => Database_AboJam.instance;
    public Upgrade upgrade => global::Upgrade.instance;
    public Button button;
    public AnimationClick animationClick;
    public Image image;
    public TextMeshProUGUI tmp_price;
    public TextMeshProUGUI tmp_level;
    public Sprite[] sprites_level;

    /* Field & Property */
    public static List<BTN_Upgrade> instances = new List<BTN_Upgrade>();
    public event Action<int> eventUpgrade;
    [SerializeField] private int maxLevel = 1; public int MaxLevel { get => maxLevel; }
    private int price; public int Price
    {
        get => price;

        set
        {
            price = value;
            tmp_price.text = price.ToString();
        }
    }
    private int level; public int Level
    {
        get => level;

        private set
        {
            level = value;
            tmp_level.text = "Lv. " + level.ToString();
        }
    }

    [SerializeField] public string ID; // Primary Key
    [SerializeField] public int[] reinforceCost; public int[] ReinforceCost { get => reinforceCost; set => reinforceCost = value; } // 레벨업 비용 (개수 = 최대 레벨 결정)
    [SerializeField] private float coefficient;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();
        instances.Add(this);

        Level = 0;
        button.onClick.AddListener(() => animationClick.OnClick());
        button.onClick.AddListener(() => Buy());
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportUpgrade(ID, ref reinforceCost, ref coefficient);

        ChangeImage();
        ChangePrice();
    } // Import 시 자동 실행
    private void OnEnable()
    {
        Load();
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 상품을 구매합니다.
    /// </summary>
    public void Buy()
    {
        if (Check() == false) return;

        Checkout();
        LevelUp();
        ChangePrice();
        ChangeImage();
        Upgrade();
    }

    /* Private Method */
    /// <summary>
    /// 상품 구매 가능 여부를 계산합니다.
    /// </summary>
    private bool Check()
    {
        // 이미 최대 레벨인지 체크
        if (Level >= maxLevel)
        {
            Message.instance.On("이미 최대 레벨입니다.", 2f);
            return false;
        }

        // 가루 부족한지 체크
        if (StaticData.Garu < Price)
        {
            Message.instance.On("상품을 구매하기 위한 가루가 부족합니다.", 2f);
            return false;
        }

        return true;
    }
    /// <summary>
    /// 결제합니다.
    /// </summary>
    private void Checkout()
    {
        Message.instance.On("상품 구매가 완료되었습니다.", 2f);
        StaticData.Garu -= price;
        if (eventUpgrade == null) eventUpgrade += Log.addCharacterLog;
        eventUpgrade?.Invoke(price);
    }
    /// <summary>
    /// 업그레이드 레벨을 증가시킵니다.
    /// </summary>
    private void LevelUp()
    {
        Level++;
        if (Level == maxLevel) tmp_price.text = "MAX";
    }
    /// <summary>
    /// <br>구매 시 다음 상품 이미지로 변경합니다.</br>
    /// <br>다음 이미지가 없으면 현재 이미지를 유지합니다.</br>
    /// </summary>
    private void ChangeImage()
    {
        try { image.sprite = sprites_level[Level]; }
        catch { }
    }
    /// <summary>
    /// <br>구매 시 다음 가격으로 변경합니다.</br>
    /// <br>다음 가격이 없으면 현재 가격을 유지합니다.</br>
    /// </summary>
    private void ChangePrice()
    {
        try { Price = reinforceCost[Level]; }
        catch { }
    }
    /// <summary>
    /// ID 에 따라 무기의 각 속성을 업그레이드 합니다.
    /// </summary>
    private void Upgrade()
    {
        if (ID == "Upgrade_Damage") upgrade.multiplierDamage += coefficient;
        if (ID == "Upgrade_Knockback") upgrade.multiplierKnockback += coefficient;
        if (ID == "Upgrade_Range") upgrade.multiplierRange += coefficient;
        if (ID == "Upgrade_Rate") upgrade.multiplierRate += coefficient;
        upgrade.Apply();
    }
}
