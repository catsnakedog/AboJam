using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Menu : MonoBehaviour
{
    /* Dependency */
    [SerializeField] public GameObject panel;
    [SerializeField] Bat bat;
    [SerializeField] ChainSaw chainsaw;
    [SerializeField] private Player player;

    /* Field & Property */
    public static Menu instance;
    public static Action eventClick;

    /* Initializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
    }

    /* Public Method */
    public void MenuOnOff()
    {
        // 사망 시 Menu 작동 금지
        if (player.enabled == false) return;

        eventClick.Invoke();
        panel.SetActive(!panel.activeSelf);
        TimeOnOff();

        // 배트 이펙트를 제거하기 위함
        bat.ChargeAttack(0);
        // 전기톱 버그 악용 제거
        if(chainsaw.GetTrigger) chainsaw.TriggerEnd();
    }
    public void TimeOnOff()
    {
        if (Time.timeScale != 0) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}
