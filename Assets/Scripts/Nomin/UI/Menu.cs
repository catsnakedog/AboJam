using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void MenuOnOff()
    {
        panel.SetActive(!panel.activeSelf);
        TimeOnOff();
    }
    public void TimeOnOff()
    {
        if (Time.timeScale != 0) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}
