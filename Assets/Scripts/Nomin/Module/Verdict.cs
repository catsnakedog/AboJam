using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Verdict : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private HP playerHP;
    [SerializeField] private GameObject enemyPool;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject ui;
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
            if (date.isNight == true && // 밤이며
                spawner.waveEnd == true && // 웨이브가 종료되었고
                CheckMob() == false) // 남은 몬스터가 없으면
            {
                spawner.waveEnd = false;
                date.SkipNight(); // 밤 종료
            }

            yield return waitForSeconds;
        }

        bool CheckMob()
        {
            foreach (Transform child in enemyPool.transform) if (child.gameObject.activeSelf) return true;
            return false;
        }
    }

    /* Public Method */
    /// <summary>
    /// History 를 갱신합니다.
    /// </summary>
    public void SaveHistory()
    {
        StaticData.gameData.dateTime = Date.instance.dateTime.ToString("o"); ;
        //StaticData.gameData.kill;
        //StaticData.gameData.abocado;
        //StaticData.gameData.tower;
        //StaticData.gameData.garu;
    }

    /* Private Method */
    /// <summary>
    /// 게임 클리어를 감지합니다.
    /// </summary>
    private void CheckGameClear()
    {
        if (spawner == null || database_abojam == null) return;
        if (spawner.waveIndex >= database_abojam.Wave.Count)
        {
            message.On("축하합니다. 게임을 클리어하였습니다 !", 3f);
            date.timeFlow = false;
        }
    }
    /// <summary>
    /// 플레이어 HP 가 0 에 도달하면 작동합니다.
    /// </summary>
    private void Lose()
    {
        player.enabled = false;
        SaveHistory();
        GameObject lose = Instantiate(this.lose, ui.transform);
        TextMeshProUGUI resultText = lose.GetComponentInChildren<TextMeshProUGUI>();
        DateTime dateTime = DateTime.Parse(StaticData.gameData.dateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        resultText.text = $"[ 게임 결과 ]\n 나이: {dateTime: y살 d일}";
    }
}