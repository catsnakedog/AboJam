using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    /* Dependency */
    public Button BTN_Damage;
    public Button BTN_Speed;
    public Button BTN_Range;
    public Button BTN_Knockback;

    /* Field & Property */
    public static Upgrade instacne;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instacne = this;
    }
}
