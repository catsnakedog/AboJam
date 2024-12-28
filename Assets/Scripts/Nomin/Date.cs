using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Date : MonoBehaviour
{
    /* Dependency */
    public Sprite sprite_morning;
    public Sprite sprite_night;
    public TextMeshProUGUI text_day;
    public TextMeshProUGUI text_time;
    private Image image;
    private AnimationClick animationClick;

    /* Field & Property */
    public static Date instance; // 싱글턴
    public bool timeFlow = true; // 시간 흐름 On / Off
    [SerializeField] private double secondsPerDay = 86400;
    [SerializeField] private string startTime = "06:00"; // 게임 시작 시각
    public string morningStartTime = "06:00"; // 낮 시작 시각
    public string morningEndTime = "18:00"; // 낮 종료 시각
    public bool isMorning { get; private set; } = true;
    public DateTime dateTime = DateTime.MinValue; // 게임 시각
    private TimeSpan start;
    private TimeSpan end;
    private DateTime last = DateTime.Now;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        image = GetComponent<Image>();
        animationClick = GetComponent<AnimationClick>();
        dateTime += StringToTime(startTime);
        start = StringToTime(morningStartTime);
        end = StringToTime(morningEndTime);
        StartCoroutine(CorTime());
    }

    /* Private Method */
    /// <summary>
    /// <br>게임 시간 Update 를 위한 코루틴 입니다.</br>
    /// </summary>
    private IEnumerator CorTime()
    {
        while (true)
        {
            UpdateTime(timeFlow);
            UpdateImage();
            yield return new WaitForSeconds(0.1f);
        }
    }
    /// <summary>
    /// <br>시간 흐름을 계산하여 더합니다.</br>
    /// </summary>
    private void UpdateTime(bool timeFlow)
    {
        if (!timeFlow) return;

        // 게임시간 += (경과시간 * 시간비율)
        // 1) 경과시간 : 현재 SetTime 시각 - 이전 SetTime 시각
        // 2) 시간비율 : 현실 하루 (초) / 게임 하루 (초)
        DateTime now = DateTime.Now;
        TimeSpan elapsed = now - last; // 1)
        last = now;
        double ratio = 86400d / secondsPerDay; // 2)
        dateTime += elapsed * ratio;

        // UI 업데이트
        TimeSpan total = dateTime - DateTime.MinValue;
        text_day.text = $"DAY - {((int)(total.TotalDays + 1))}";
        text_time.text = $"{dateTime.Hour}:{dateTime.Minute}";
    }
    /// <summary>
    /// 시각에 따라 이미지를 변경합니다.
    /// </summary>
    private void UpdateImage()
    {
        // 낮과 밤 판정
        bool morning = dateTime.TimeOfDay >= start && dateTime.TimeOfDay < end;

        // 낮 / 밤이 달라졌을 경우에만 실행
        if (isMorning != morning)
        {
            isMorning = morning;
            if (isMorning == true) image.sprite = sprite_morning;
            else image.sprite = sprite_night;
            animationClick.OnClick();
        }
    }
    /// <summary>
    /// <br>인스펙터의 string 을 TimeSpan 으로 변환합니다.</br>
    /// </summary>
    /// <param name="timeString">변환될 string 값</param>
    /// <returns>TimeSpan 으로 변환된 string 입니다.</returns>
    private TimeSpan StringToTime(string timeString)
    {
        try { TimeSpan time = TimeSpan.Parse(timeString); return time; }
        catch { Debug.Log($"{gameObject.name} 의 {timeString} 는 유효한 시각이 아닙니다."); return new TimeSpan(0, 0, 0); }
    }
}