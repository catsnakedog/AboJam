using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheat : MonoBehaviour
{
    private Farming farming => Farming.instance;
    public HP playerHP;

    public void ShowMeTheMoney()
    {
        StaticData.Abocado = 9999;
        StaticData.Garu = 9999;
    }

    public void OperationCwal()
    {
        farming.cultivateTime = 0.01f;
    }

    public void PowerOverwhelming()
    {
        playerHP.SetMaxHP(9999);
        playerHP.Heal(playerHP.Hp_max);
    }
}
