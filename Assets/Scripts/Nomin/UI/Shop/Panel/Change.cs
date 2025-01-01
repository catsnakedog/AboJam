using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Change : MonoBehaviour
{
    /* Dependency */
    public Button button;
    public AnimationClick animationClick;
    public TextMeshProUGUI text_price;
    public TextMeshProUGUI text_quantity;

    /* Field & Property */
    public static Change instance;
    public int Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
            text_price.text = price.ToString();
        }
    }
    public int Quantity
    {
        get
        {
            return quantity;
        }

        set
        {
            quantity = value;
            text_quantity.text = quantity.ToString();
        }
    }
    #region Backing Field
    private int price;
    private int quantity;
    #endregion

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        button.onClick.AddListener(() => Trade(quantity));
        button.onClick.AddListener(() => animationClick.OnClick());
        Price = 6;
        Quantity = 1;
    }

    /* Public Method */
    /// <summary>
    /// 아보카도를 판매합니다.
    /// </summary>
    /// <param name="quantity">판매 개수</param>
    public void Trade(int quantity)
    {
        if (StaticData.Abocado >= quantity)
        {
            StaticData.Abocado -= Quantity;
            StaticData.Garu += Quantity * Price;
        }
    }
}
