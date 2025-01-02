using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BTN_Buy : MonoBehaviour
{
    /* Dependency */
    public Button button;
    public AnimationClick animationClick;
    public Image image;
    public TextMeshProUGUI tmp_price;
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

    #region Backing Field
    private int price;
    #endregion

    /* Field & Property */
    public static List<BTN_Buy> instances = new List<BTN_Buy>();

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        Price = 10;
        button.onClick.AddListener(() => animationClick.OnClick());
        button.onClick.AddListener(() => Check());
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
    private void Check()
    {
        if (StaticData.Garu < Price) Message.instance.On("상품을 구매하기 위한 가루가 부족합니다.", 2f);
        else
        {
            Message.instance.On("상품 구매가 완료되었습니다.", 2f);
            StaticData.Garu -= Price;
        }
    }
}
