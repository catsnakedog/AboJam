using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background : MonoBehaviour
{
    /* Event */

    private event Action eventTap;
    private event Action eventDoubleTap;

    /* Field */

    [SerializeField] private float doubleTapThreshold;
    private DateTime lastTouchTime;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        lastTouchTime = DateTime.Now;
        eventDoubleTap = LoadGame;
    }

    /* Public Method */

    public void Tap()
    {
        DateTime currentTouchTime = DateTime.Now;
        bool isDoubleTap = CheckIsDoubleTap(currentTouchTime);
        lastTouchTime = currentTouchTime;

        if (isDoubleTap) eventDoubleTap?.Invoke();
        else eventTap?.Invoke();
    }

    /* Private Method */

    private void LoadGame()
    {
        SceneManager.LoadScene("Nomin");
        StaticData.Init();
    }

    private bool CheckIsDoubleTap(DateTime currentTouchTime)
    {
        TimeSpan timedif = currentTouchTime - lastTouchTime;
        return timedif.TotalSeconds < doubleTapThreshold;
    }
}