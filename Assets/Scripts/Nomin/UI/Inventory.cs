using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /* Field & Property */
    public static Inventory instance; // 싱글턴
    public TextMeshProUGUI abocado;
    public TextMeshProUGUI garu;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        StaticData.Abocado = StaticData.Abocado;
        StaticData.Garu = StaticData.Garu;
        StaticData.Water = StaticData.Water;
    }

    /* Public Method */
    public void UpdateAbocado()
    {
        abocado.text = StaticData.Abocado.ToString();
    }
    public void UpdateGaru()
    {
        garu.text = StaticData.Garu.ToString();
    }
    public void UpdateWater()
    {
    }
}
