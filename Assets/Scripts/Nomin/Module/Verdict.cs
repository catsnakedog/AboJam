using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class Verdict : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private HP playerHP;
    [SerializeField] private GameObject enemyPool;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject skip;
    [SerializeField] private Player player;
    private Date date => Date.instance;
    private Spawner spawner => Spawner.instance;
    private Database_AboJam database_abojam => Database_AboJam.instance;
    private Message message => Message.instance;

    /* Field & Property */
    public static Verdict instance;
    private WaitForSeconds waitForSeconds;

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        instance = this;
        waitForSeconds = new WaitForSeconds(0.3f);
        date.morningStart.AddListener(() => CheckGameClear());
        playerHP.death.AddListener(() => Lose());

        StartCoroutine(UpdateCheckNightClear());
    }
    /// <summary>
    /// <br>밤의 클리어를 감지합니다.</br>
    /// </summary>
    private IEnumerator UpdateCheckNightClear()
    {
        while (true)
        {
            if (date.gameTime == Date.GameTime.Night && // 밤이며
                spawner.waveEnd == true && // 웨이브가 종료되었고
                CheckMob() == false && // 남은 몬스터가 없으며
                playerHP.HP_current > 0) // 플레이어가 죽지 않았으면
            {
                spawner.waveEnd = false;
                date.Skip(); // 밤 종료
            }

            yield return waitForSeconds;
        }
    }

    /* Public Method */
    /// <summary>
    /// History 를 갱신합니다.
    /// </summary>
    public void SaveHistory()
    {
        StaticData.gameData.dateTime = Date.instance.dateTime.ToString("o"); ;
    }
    /// <summary>
    /// History 를 갱신합니다.
    /// </summary>
    public void SaveHistoryOnClear()
    {
        StaticData.gameData.dateTime = Date.instance.dateTime.AddDays(1).ToString("o"); ;
    }
    /// <summary>
    /// 남은 몬스터가 있는지 여부를 반환합니다.
    /// </summary>
    public bool CheckMob()
    {
        foreach (Transform child in enemyPool.transform) if (child.gameObject.activeSelf) return true;
        return false;
    }

    /* Private Method */
    /// <summary>
    /// 게임 클리어를 감지합니다.
    /// </summary>
    private void CheckGameClear()
    {
        if (spawner == null || database_abojam == null) return;

        int maxWaveNumber = 0;

        foreach (var wave in database_abojam.Wave)
        {
            Match match = Regex.Match(wave.ID, @"\d+");
            if (match.Success)
            {
                int num = int.Parse(match.Value);
                if (num > maxWaveNumber) maxWaveNumber = num;
            }
        }

        if (spawner.waveIndex > maxWaveNumber)
        {
            skip.SetActive(false);
            SaveHistoryOnClear();
            date.timeFlow = false;
            StartCoroutine(CorClearMessage());
        }
    }
    /// <summary>
    /// 클리어 메시지를 출력하고, 씬을 이동하니다.
    /// </summary>
    private IEnumerator CorClearMessage()
    {
        message.On("적들이 약탈을 포기한 것 같군요 !", 2f);
        yield return new WaitForSeconds(2f);
        message.On("축하해요 ! 당신은 끝까지 살아남았습니다.", 2f);
        yield return new WaitForSeconds(2f);
        message.On("잠시 후 메인 화면으로 돌아갑니다.", 2f);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main");
    }
    /// <summary>
    /// 플레이어 HP 가 0 에 도달하면 작동합니다.
    /// </summary>
    private void Lose()
    {
        player.enabled = false;
        player.gameObject.tag = "Untagged";
        SaveHistory();
        GameObject lose = Instantiate(this.lose, ui.transform);
        TextMeshProUGUI resultText = lose.GetComponentInChildren<TextMeshProUGUI>();
        DateTime dateTime = DateTime.Parse(StaticData.gameData.dateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        resultText.text = $"나이: {dateTime:y살 d일}\n" +
                          $"처치: {StaticData.gameData.kill}\n" +
                          $"심은 아보카도: {StaticData.gameData.abocado}\n" +
                          $"건설한 타워: {StaticData.gameData.tower}\n" +
                          $"사용한 가루: {StaticData.gameData.garu}";
    }
}