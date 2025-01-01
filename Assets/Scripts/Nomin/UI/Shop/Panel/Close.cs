using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Close : MonoBehaviour
{
    /* Dependency */
    public Button button;

    /* Field & Property */
    public static Close instance;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        button.onClick.AddListener(() => OnOff.instance.Switch());
    }
}
