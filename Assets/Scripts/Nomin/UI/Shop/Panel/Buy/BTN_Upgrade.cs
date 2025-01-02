using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class BTN_Upgrade : MonoBehaviour
{
    /* Dependency */
    public Button button;
    public AnimationClick animationClick;
    public Image image;
    public TextMeshProUGUI tmp_price;
    public TextMeshProUGUI tmp_level;
    public Sprite[] sprites_level;

    /* Field & Property */
    public static List<BTN_Upgrade> instances = new List<BTN_Upgrade>();
    [SerializeField] private int[] prices;
    public int Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
            tmp_price.text = price.ToString();
        }
    }
    private int price; // 백킹 필드
    public int Level
    {
        get
        {
            return level;
        }

        private set
        {
            level = value;
            tmp_level.text = "Lv. " + level.ToString();
        }
    }
    private int level; // 백킹 필드
    [SerializeField] private int maxLevel = 1;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        ChangeImage();
        ChangePrice();
        Level = 0;
        button.onClick.AddListener(() => animationClick.OnClick());
        button.onClick.AddListener(() => Buy());
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
        try { Price = prices[Level]; }
        catch { }
    }
}
