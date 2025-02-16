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
    public int value;
    public int maxQuantity;
    private int quantity;
    private int price;

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
        price = quantity * value;
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
            StaticData.Garu += price;
        }
        else Message.instance.On($"{quantity} 개 이상부터 판매할 수 있습니다.", 2f);
    }
}
