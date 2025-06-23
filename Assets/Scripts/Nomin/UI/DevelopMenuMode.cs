using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopMenuMode : MonoBehaviour
{
    public GameObject[] menus;

    void Start()
    {
        foreach (var item in menus)
        {
            item.SetActive(BTN_DEVELOPER.isOn);
        }
    }
}
