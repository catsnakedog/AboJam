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
    public TMP_InputField text_quantity;
    public TextMeshProUGUI text_price;
    private Message message => Message.instance;

    /* Field & Property */
    public static Change instance;
    public int value; // 아보카도 1 개당 가격
    public int discount; // threshold 회 교환마다 감소할 가격
    public int threshold; // 가격이 감소하게 될 거래 횟수
    public int minValue; // 최소 아보카도 가격
    public int maxQuantity; // 거래 최대 개수
    private int quantity; // 거래할 아보카도 수
    private int tradeCount; public int TradeCount { set => tradeCount = value; } // 총 판매 개수

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        button.onClick.AddListener(() => Trade());
        button.onClick.AddListener(() => animationClick.OnClick());
        OnValueChange();
    }

    /* Public Method */
    /// <summary>
    /// 사용자가 입력한 아보카도 개수에 따라 가격을 변동합니다.
    /// </summary>
    public void OnValueChange()
    {
        try { quantity = int.Parse(text_quantity.text); } catch { }
        if (quantity > maxQuantity) { message.On($"{maxQuantity} 개 까지만 팔 수 있어요 !", 2f); quantity = maxQuantity; }

        text_price.text = GetPrice(quantity).ToString();
    }
    /// <summary>
    /// 아보카도를 판매합니다.
    /// </summary>
    /// <param name="quantity">판매 개수</param>
    public void Trade()
    {
        if (StaticData.Abocado >= quantity)
        {
            Message.instance.On($"아보카도 {quantity} 개를 가루 {GetPrice(quantity)} 개에 판매했습니다.", 2f);
            StaticData.Abocado -= quantity;
            StaticData.Garu += GetPrice(quantity);
            tradeCount += quantity;
            OnValueChange();
        }
        else Message.instance.On($"{quantity} 개 이상부터 판매할 수 있습니다.", 2f);
    }

    /* Private Method */
    /// <summary>
    /// 아보카도 수량에 따른 가치를 반환합니다.
    /// </summary>
    private int GetPrice(int quantity)
    {
        int price = 0; // 모든 아보카도의 가치
        int currentValue = value; // 현재 아보카도의 가치
        int thresholdTemp = 0; // threshold 에 도달할 때 마다 currentValue -= discount

        // 이전 까지의 아보카도 총 거래 개수로 currentValue 를 계산합니다.
        for (int i = 0; i < tradeCount; i++)
        {
            thresholdTemp++;
            if (thresholdTemp == threshold)
            {
                currentValue = Mathf.Max(minValue, currentValue - discount);
                thresholdTemp = 0;
            }
        }

        // currentValue 를 계산하며 각 아보카도의 가치를 price 에 더합니다.
        for (int i = 0; i < quantity; i++)
        {
            price += currentValue;

            thresholdTemp++;
            if (thresholdTemp == threshold)
            {
                currentValue = Mathf.Max(minValue, currentValue - discount);
                thresholdTemp = 0;
            }
        }

        return price;
    }
}
