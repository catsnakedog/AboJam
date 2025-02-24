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
    private int tradeCount; // 총 거래 횟수
    private int price; // 현재 거래 가격
    private int nextPrice; // 다음 거래 가격

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

        price = quantity * GetCalculatedValue();
        text_price.text = price.ToString();
    }
    /// <summary>
    /// 아보카도를 판매합니다.
    /// </summary>
    /// <param name="quantity">판매 개수</param>
    public void Trade()
    {
        if (StaticData.Abocado >= quantity)
        {
            Message.instance.On($"아보카도 {quantity} 개를 가루 {price} 개에 판매했습니다.", 2f);
            StaticData.Abocado -= quantity;
            StaticData.Garu += quantity * GetCalculatedValue();
            tradeCount++;
            OnValueChange();
        }
        else Message.instance.On($"{quantity} 개 이상부터 판매할 수 있습니다.", 2f);
    }
    
    /* Private Method */
    /// <summary>
    /// 거래 횟수에 따라 차감된 금액을 반환합니다.
    /// </summary>
    private int GetCalculatedValue()
    {
        int calculatedValue = value;
        int tempTradeCount = tradeCount;
        while (tempTradeCount >= threshold && calculatedValue > minValue)
        {
            calculatedValue -= discount;
            tempTradeCount -= threshold;
        }

        return calculatedValue;
    }
}
