using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BTN_DEVELOPER : MonoBehaviour
{
    public static event Action eventSwitchMode;
    public static bool isOn = false;

    public void Awake()
    {
        eventSwitchMode = null;
    }

    public void SwitchDevelopMode()
    {
        isOn = !isOn;
        eventSwitchMode?.Invoke();
    }
}
