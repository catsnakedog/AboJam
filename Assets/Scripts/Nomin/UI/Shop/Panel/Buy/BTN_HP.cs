using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BTN_HP : MonoBehaviour
{
    public HP playerHP;
    public float efficient;
    public int[] price;
    private int upgradeCount = 0;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI levelText;

    public void Upgrade()
    {
        // 최대 업그레이드 완료
        if (upgradeCount >= price.Length)
        {
            Message.instance.On("이미 최대 업그레이드에요 !", 2f);
            return;
        }

        // 비용 부족
        if (StaticData.Garu < price[upgradeCount])
        {
            Message.instance.On("비용이 충분하지 않아요 !", 2f);
            return;
        }

        UpgradeHP();
        StaticData.Garu -= price[upgradeCount];
        Message.instance.On("더욱 튼튼해졌습니다 !", 2f);
        upgradeCount++;

        levelText.text = upgradeCount.ToString();
        if (upgradeCount >= price.Length) priceText.text = "Max";
        else priceText.text = price[upgradeCount].ToString();
    }

    private void UpgradeHP()
    {
        playerHP.SetMaxHP(playerHP.Hp_max * efficient);
        playerHP.Heal(playerHP.Hp_max);
    }
}
