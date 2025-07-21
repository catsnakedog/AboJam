using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Date : RecordInstance<Table_Date, Record_Date>
{
    /* Dependency */
    public Sprite sprite_morning;
    public Sprite sprite_night;
    public Sprite sprite_skipMorning;
    public Sprite sprite_skipNight;
    public TextMeshProUGUI text_day;
    public TextMeshProUGUI text_time;
    public enum GameTime : byte
    {
        Morning,
        Sunset,
        Night
    }
    public Image skipImage;
    Image image;
    AnimationClick animationClick;
    GlobalLight globalLight => GlobalLight.instance;
    Database_AboJam database_abojam => Database_AboJam.instance;
    Message message => Message.instance;
    CoolTimer cooltimer => CoolTimer.instance;
    List<IEnemy> iEnemies => IEnemy.instances;
    Spawner spawner => Spawner.instance;

    /* Field & Property */
    public static Date instance; // 싱글턴
    public bool timeFlow = true; // 시간 흐름 On / Off
    [SerializeField] public int secondsPerDay = 86400;
    [SerializeField] private string startTime = "06:00"; // 게임 시작 시각
    [SerializeField] private string morningTime = "06:00"; // 낮 시작 시각
    [SerializeField] private string sunsetTime = "17:30"; // 해질녘 시작 시간
    [SerializeField] private string nightTime = "20:00"; // 밤 시작 시간
    public DateTime dateTime = DateTime.MinValue; // 게임 시각
    public GameTime gameTime; // 낮 / 해질녘 / 밤 표기
    public UnityEvent morningStart; // 아침이 시작할 때 작동할 메서드
    public UnityEvent sunsetStart; // 해질녘이 시작할 때 작동할 메서드
    public UnityEvent nightStart; // 밤이 시작할 때 작동할 메서드
    public static Action<int> eventMorning;
    public static Action eventNight;
    public static Action eventSkipSuccess;
    public static Action eventSkipFail;
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
            if (timeFlow) UpdateTime();
            UpdateGameTime();
            UpdateText();
            yield return waitForSeconds;
        }
    }
    /// <summary>
    /// <br>시간 흐름을 계산하여 더합니다.</br>
    /// <br>게임시간 += (경과시간 * 시간비율)</br>
    /// <br>1) 경과시간 : 현재 SetTime 시각 - 이전 SetTime 시각</br>
    /// <br>2) 시간비율 : 현실 하루 (초) / 게임 낮 (초)</br>
    /// </summary>
    private void UpdateTime()
    {
        DateTime now = DateTime.Now;
        TimeSpan elapsed = now - last; // 1)
        if (elapsed.TotalSeconds > refreshTime * 5) { elapsed = TimeSpan.FromSeconds(refreshTime); }
        last = now;

        double ratio = (StringToTime(nightTime) - StringToTime(morningTime)).TotalSeconds / (double)secondsPerDay; // 2)
        dateTime += elapsed * ratio;
    }
    /// <summary>
    /// 시각에 따라 게임 타임을 변경합니다.
    /// </summary>
    /// <param name="isChanged">GameTime 이 변경되었는지 여부를 반환합니다.</param>
    private void UpdateGameTime()
    {
        TimeSpan currentTime = dateTime.TimeOfDay;
        TimeSpan morningStartTime = TimeSpan.Parse(morningTime);
        TimeSpan sunsetStartTime = TimeSpan.Parse(sunsetTime);
        TimeSpan nightStartTime = TimeSpan.Parse(nightTime);

        GameTime nextGameTime;
        if (currentTime >= morningStartTime && currentTime < sunsetStartTime) nextGameTime = GameTime.Morning;
        else if (currentTime >= sunsetStartTime && currentTime < nightStartTime) nextGameTime = GameTime.Sunset;
        else nextGameTime = GameTime.Night;

        // 시간의 흐름에 따라 Skip
        if (gameTime != nextGameTime) Skip();
    }
    /// <summary>
    /// 시각에 따라 텍스트를 변경합니다.
    /// </summary>
    private void UpdateText()
    {
        TimeSpan total = dateTime - DateTime.MinValue;
        text_day.text = $"DAY {((int)(total.TotalDays + 1))}";
        text_time.text = $"{dateTime.Hour}:{dateTime.Minute}";
    }

    /* Public Method */
    /// <summary>
    /// 현재 GameTime 을 스킵합니다.
    /// </summary>
    public void Skip()
    {
        if (gameTime == GameTime.Morning) SkipMorning();
        else if (gameTime == GameTime.Sunset) SkipSunset();
        else if (gameTime == GameTime.Night) SkipNight();
    }
    /// <summary>
    /// 특정 GameTime 까지 강제 스킵합니다.
    /// </summary>
    public void Skip(GameTime gameTime)
    {
        while (this.gameTime != gameTime) Skip();
    }
    /// <summary>
    /// 특정 날짜의 아침까지 강제 스킵합니다.
    /// </summary>
    public void Skip(int day)
    {
        // 빛 & 이벤트 호출
        morningStart?.Invoke();
        globalLight.Set(globalLight.morning, 0.01f);

        // 시간 설정 (흐름)
        dateTime = DateTime.MinValue.AddDays(day);
        gameTime = GameTime.Morning;
        text_time.enabled = true;
        timeFlow = true;
        dateTime = dateTime.Date + StringToTime(morningTime);
        last = DateTime.Now;
        ChangeImage();

        TimeSpan total = dateTime - DateTime.MinValue;
        eventMorning?.Invoke((int)(total.TotalDays + 1));
    }
    /// <summary>
    /// 아침을 스킵하고 해질녘이 시작됩니다.
    /// </summary>
    /// <param name="skipedSeconds">스킵된 초</param>
    public void SkipMorning()
    {
        // 빛 & 이벤트 호출
        if (gameTime != GameTime.Morning) return; 
        sunsetStart?.Invoke();
        globalLight.Set(globalLight.sunset, 0.01f);
        eventSkipSuccess.Invoke();

        // 시간 설정
        DateTime beforeTime = dateTime;
        dateTime = dateTime.Date + StringToTime(sunsetTime);
        cooltimer.AddTime(-(float)GameTimeToRealTime((dateTime - beforeTime).TotalSeconds));

        // GameTime 및 Image 넘김
        gameTime++;
        if (!Enum.IsDefined(typeof(GameTime), gameTime)) gameTime = 0;
        ChangeImage();
    }
    /// <summary>
    /// 해질녘을 스킵하고 밤이 시작됩니다.
    /// </summary>
    /// <param name="skipedSeconds">스킵된 초</param>
    public void SkipSunset()
    {
        // 빛 & 이벤트 호출
        if (gameTime != GameTime.Sunset) return; 
        nightStart?.Invoke();
        globalLight.Set(globalLight.night, 0.01f);
        eventSkipSuccess.Invoke();

        // 시간 설정 (정지)
        text_time.enabled = false;
        timeFlow = false;
        DateTime beforeTime = dateTime;
        dateTime = dateTime.Date + StringToTime(nightTime);
        cooltimer.AddTime(-(float)GameTimeToRealTime((dateTime - beforeTime).TotalSeconds));

        // GameTime 및 Image 넘김
        gameTime++;
        if (!Enum.IsDefined(typeof(GameTime), gameTime)) gameTime = 0;
        ChangeImage();

        eventNight?.Invoke();
    }
    /// <summary>
    /// 밤을 스킵하고 아침이 시작됩니다.
    /// </summary>
    public void SkipNight()
    {
        // 빛 & 이벤트 호출
        if (gameTime != GameTime.Night) return;
        morningStart?.Invoke();
        globalLight.Set(globalLight.morning, 0.01f);

        // 시간 설정 (흐름)
        text_time.enabled = true;
        timeFlow = true;
        dateTime = dateTime.Date + TimeSpan.FromDays(1) + StringToTime(morningTime);
        last = DateTime.Now;

        // GameTime 및 Image 넘김
        gameTime++;
        if (!Enum.IsDefined(typeof(GameTime), gameTime)) gameTime = 0;
        ChangeImage();

        // 스포너 중지
        if(spawner != null)
        {
            spawner.StopCoroutine(spawner.lastCor);
            spawner.waveEnd = true;
        }

        TimeSpan total = dateTime - DateTime.MinValue;
        eventMorning?.Invoke((int)(total.TotalDays + 1));
    }
    /// <summary>
    /// 모든 몬스터를 죽입니다.
    /// </summary>
    public void KillMob()
    {
        foreach (IEnemy iEnemy in iEnemies)
        {
            dynamic enemy = iEnemy;
            if (enemy.hp.HP_current > 0)
            {
                enemy.hp.Damage(enemy.hp.Hp_max);
                //enemy.StartCoroutine(enemy.CorDeath(0.3f));
            }
        }
    }

    /// <summary>
    /// 밤에 낮잠을 자려하면 메시지를 출력합니다.
    /// </summary>
    public void ViewCheck()
    {
        if (gameTime == GameTime.Night)
        {
            message.On("지금은 낮이 아니에요 !", 2f);
            eventSkipFail.Invoke();
        }
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
    /// 시각에 따라 이미지를 변경합니다.
    /// </summary>
    private void ChangeImage()
    {
        if (gameTime == GameTime.Morning)
        {
            image.sprite = sprite_morning; animationClick.OnClick();
            skipImage.sprite = sprite_skipMorning;
        }
        if (gameTime == GameTime.Night)
        {
            image.sprite = sprite_night; animationClick.OnClick();
            skipImage.sprite = sprite_skipNight;
        }
    }
    /// <summary>
    /// 게임 시간을 현실 시간으로 변환합니다.
    /// </summary>
    private double GameTimeToRealTime(double seconds)
    {
        double dayDiff = (StringToTime(nightTime) - StringToTime(morningTime)).TotalSeconds;
        return (seconds * secondsPerDay) / dayDiff;
    }
}