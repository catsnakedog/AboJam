using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /* Dependency */
    public TextMeshProUGUI abocado;
    public TextMeshProUGUI garu;

    /* Field & Property */
    public static List<Inventory> instances = new List<Inventory>();

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        StaticData.Abocado = StaticData.Abocado;
        StaticData.Garu = StaticData.Garu;
        StaticData.Water = StaticData.Water;
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    public void UpdateAbocado()
    {
        if (abocado != null) abocado.text = StaticData.Abocado.ToString();
    }
    public void UpdateGaru()
    {
        if (garu != null) garu.text = StaticData.Garu.ToString();
    }
    public void UpdateWater()
    {
    }
}
