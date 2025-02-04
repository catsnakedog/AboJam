using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Date : RecordInstance<Table_Date, Record_Date>
{
    /* Dependency */
    public Sprite sprite_morning;
    public Sprite sprite_night;
    public TextMeshProUGUI text_day;
    public TextMeshProUGUI text_time;
    private Image image;
    private AnimationClick animationClick;
    private GlobalLight globalLight => GlobalLight.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance;

    /* Field & Property */
    public static Date instance; // 싱글턴
    public bool timeFlow = true; // 시간 흐름 On / Off
    [SerializeField] private int secondsPerDay = 86400;
    [SerializeField] private string startTime = "06:00"; // 게임 시작 시각
    [SerializeField] private string morningTime = "06:00"; // 낮 시작 시각
    [SerializeField] private string sunsetTime = "17:30"; // 해질녘 시작 시간
    [SerializeField] private string nightTime = "20:00"; // 밤 시작 시간
    public bool isMorning { get; private set; } = true;
    public bool isSunset { get; private set; } = false;
    public bool isNight { get; private set; } = false;
    public DateTime dateTime = DateTime.MinValue; // 게임 시각
    public UnityEvent morningStart; // 아침이 시작할 때 작동할 메서드
    public UnityEvent sunsetStart; // 해질녘이 시작할 때 작동할 메서드
    public UnityEvent nightStart; // 밤이 시작할 때 작동할 메서드
    private TimeSpan start;
    private TimeSpan end;
    private DateTime last = DateTime.Now;
    private float refreshTime = 0.1f;
    private WaitForSeconds waitForSeconds;

    /* Intializer & Finalizer & Updater */
    private void Awake()
    {
        instance = this;
        timeFlow = true;
    }
    private void Start()
    {
        // Start 사용 시 필수 고정 구현
        if (startFlag == true) return;
        startFlag = true;
        base.Start();
        Load();

        image = GetComponent<Image>();
        animationClick = GetComponent<AnimationClick>();
        dateTime += StringToTime(startTime);
        morningStart.AddListener(() => { Debug.Log("아침이 시작되었습니다."); });
        sunsetStart.AddListener(() => { Debug.Log("해질녘이 시작되었습니다."); });
        nightStart.AddListener(() => { Debug.Log("밤이 시작되었습니다."); });
        StartCoroutine(CorTime());
        morningStart?.Invoke();
        waitForSeconds = new WaitForSeconds(refreshTime);
    }
    public void Load()
    {
        // Load 사용 시 필수 고정 구현
        if (startFlag == false) Start();
        database_abojam.ExportDate(initialRecords[0].ID, ref secondsPerDay, ref startTime, ref morningTime, ref sunsetTime, ref nightTime);

        start = StringToTime(morningTime);
        end = StringToTime(nightTime);
    } // Import 시 자동 실행
    /// <summary>
    /// <br>게임 시간 Update 를 위한 코루틴 입니다.</br>
    /// </summary>
    private IEnumerator CorTime()
    {
        while (true)
        {
            UpdateTime(timeFlow);
            UpdateImage();
            if (isSunset == false && dateTime.TimeOfDay > TimeSpan.Parse(sunsetTime)) StartSunset();
            if (isMorning == false && isNight == false) StartNight();
            yield return waitForSeconds;
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
        if (elapsed.TotalSeconds > refreshTime * 5) { elapsed = TimeSpan.FromSeconds(refreshTime); }
        last = now;
        double ratio = 86400d / (double)secondsPerDay; // 2)
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

    /* Public Method */
    /// <summary>
    /// 밤을 스킵하고 아침이 시작됩니다.
    /// </summary>
    public void SkipNight()
    {
        if (isMorning == true) return;
        morningStart?.Invoke();
        text_time.enabled = true;
        dateTime = dateTime.Date + TimeSpan.FromDays(1) + StringToTime(morningTime);
        isNight = false;
        isSunset = false;
        timeFlow = true;
        last = DateTime.Now;
        globalLight.Set(globalLight.morning, 0.01f);
        Debug.Log("밤 스킵");
    }

    /* Private Method */
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
    /// <summary>
    /// <br>밤이 시작됩니다.</br>
    /// </summary>
    private void StartNight()
    {
        nightStart?.Invoke();
        isNight = true;
        text_time.enabled = false;
        timeFlow = false;
        globalLight.Set(globalLight.night, 0.01f);
    }
    /// <summary>
    /// <br>해질녘이 시작됩니다.</br>
    /// </summary>
    private void StartSunset()
    {
        sunsetStart?.Invoke();
        isSunset = true;
        globalLight.Set(globalLight.sunset, 0.01f);
    }
}