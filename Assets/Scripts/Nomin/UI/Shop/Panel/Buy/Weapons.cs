using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    /* Dependency */
    public Button BTN_Gun;
    public Button BTN_Shotgun;
    public Button BTN_Sniper;
    public Button BTN_Riple;
    public Button BTN_Knife;
    public Button BTN_Bat;
    public Button BTN_Spear;
    public Button BTN_Chainsaw;

    /* Field & Property */
    public static Weapons instacne;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instacne = this;
    }
}
