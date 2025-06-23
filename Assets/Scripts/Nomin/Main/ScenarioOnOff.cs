using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ScenarioOnOff : MonoBehaviour
{
    public GameObject[] tutorials;

    void Start()
    {
        foreach (var item in tutorials)
        {
            item.SetActive(BTN_DEVELOPER.isOn);
        }

        BTN_DEVELOPER.eventSwitchMode += () =>
        {
            foreach (var item in tutorials)
            {
                item.SetActive(BTN_DEVELOPER.isOn);
            }
        };
    }
}
