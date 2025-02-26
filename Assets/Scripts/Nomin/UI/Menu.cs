using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    /* Dependency */
    [SerializeField] GameObject panel;
    [SerializeField] Bat bat;

    /* Field & Property */
    public static Menu instance;

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
    }

    /* Public Method */
    public void MenuOnOff()
    {
        panel.SetActive(!panel.activeSelf);
        TimeOnOff();

        // 배트 이펙트를 제거하기 위함
        bat.ChargeAttack(0);
    }
    public void TimeOnOff()
    {
        if (Time.timeScale != 0) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}
