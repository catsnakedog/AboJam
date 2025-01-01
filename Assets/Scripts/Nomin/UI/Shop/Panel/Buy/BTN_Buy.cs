using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BTN_Buy : MonoBehaviour
{
    /* Dependency */
    public Button button;
    public AnimationClick animationClick;
    public Image image;
    public TextMeshProUGUI price;

    /* Field & Property */
    public static List<BTN_Buy> instances = new List<BTN_Buy>();

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instances.Add(this);
        button.onClick.AddListener(() => animationClick.OnClick());
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
