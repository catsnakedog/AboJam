using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System;

public class BTN_Weapons : MonoBehaviour
{
    /* Dependency */
    public Button button;
    public AnimationClick animationClick;
    public Image image;
    public Image garu;
    public TextMeshProUGUI tmp_price;
    public Sprite sprite_unlock;
    public Sprite sprite_lock;
    public Sprite sprite_select;
    public Swap swap => Swap.instance;

    /* Field & Property */
    public static List<BTN_Weapons> instances_melee = new List<BTN_Weapons>();
    public static List<BTN_Weapons> instances_range = new List<BTN_Weapons>();
    public event Action<int> eventWeapon;
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
    } // 가격
    [SerializeField] private int price = 0; // 백킹 필드
    [SerializeField] private bool purchase = false; public bool Purchase { get => purchase; } // 구매 여부
    [SerializeField] private bool isMelee = false; // 근거리 or 원거리 설정
    [SerializeField] private EnumData.Weapon weapon; public EnumData.Weapon Weapon { get => weapon; } // 버튼에 대응하는 무기

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        // 근거리 / 원거리 인스턴스 리스트 등록
        if(isMelee == true) instances_melee.Add(this);
        if(isMelee == false) instances_range.Add(this);

        // 이미지 초기화
        if (purchase == true) { ChangeImage(); Price = price; image.sprite = sprite_select; }
        if (purchase == false) { image.sprite = sprite_lock; Price = price; }

        button.onClick.AddListener(() => animationClick.OnClick());
        button.onClick.AddListener(() => Buy());
    }
    private void OnDestroy()
    {
        if (isMelee == true) instances_melee.Remove(this);
        if (isMelee == false) instances_range.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// 상품을 구매합니다.
    /// </summary>
    public void Buy()
    {
        if (Check() == false) return;

        Checkout();
        ChangeImage();
        purchase = true;
    }

    /* Private Method */
    /// <summary>
    /// 상품 구매 가능 여부를 계산합니다.
    /// </summary>
    private bool Check()
    {
        // 이미 구매한 상품인지 체크
        if (purchase == true)
        {
            if (swap == null) return false;
            Message.instance.On("무기를 장착했습니다.", 2f);
            swap.SetSlot(weapon);

            // 근접 무기 Select 해제
            if (isMelee == true)
                foreach (BTN_Weapons item in instances_melee)
                    if (item.image.sprite == item.sprite_select) item.image.sprite = item.sprite_unlock;

            // 원거리 무기 Select 해제
            if (isMelee == false)
                foreach (BTN_Weapons item in instances_range)
                    if (item.image.sprite == item.sprite_select) item.image.sprite = item.sprite_unlock;

            // 현재 무기 Select
            image.sprite = sprite_select;
            return false;
        }

        // 가루 부족 한지 체크
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
        if (eventWeapon == null) eventWeapon += Log.addWeaponLog;
        eventWeapon?.Invoke(price);
    }
    /// <summary>
    /// <br>구매 시 이미지를 변경합니다.</br>
    /// </summary>
    private void ChangeImage()
    {
        image.sprite = sprite_unlock;
        tmp_price.gameObject.SetActive(false);
        garu.gameObject.SetActive(false);
    }
}
