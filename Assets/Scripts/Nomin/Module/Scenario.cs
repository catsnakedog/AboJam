using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Image = UnityEngine.UI.Image;

public class Scenario : MonoBehaviour
{
    /* Dependency */
    private Date date => Date.instance;
    private Message message => Message.instance;
    private GlobalLight globalLight => GlobalLight.instance;
    private LocalData localData => LocalData.instance;

    /* Field & Property */
    public static Scenario instance;
    private float checkTime = 0.5f;
    private WaitForSeconds waitForSeconds;

    /* Initializer & Finalizer & Updater */
    private void Awake() { instance = this; waitForSeconds = new WaitForSeconds(checkTime); }
    private void Start() { if (localData.LoadScenario() < 1) StartCoroutine(CorScenario()); }

    /// <summary>
    /// 시나리오 데몬입니다.
    /// </summary>
    public IEnumerator CorScenario()
    {
        StaticData.Abocado = 1;
        StaticData.Garu = 0;
        StaticData.Water = 0;
        date.dateTime = DateTime.MinValue;
        date.timeFlow = false;
        message.On($"반가워요 ! 아보카도 농장에 오신걸 환영해요.", 2f);
        yield return new WaitForSeconds(2f);
        message.On($"F 를 꾸욱 누르고 땅을 클릭해 보세요 !", 999f);
        while (Abocado.instances.Count < 1) yield return waitForSeconds;

        message.On("잘 했어요 ! 다시 F + 클릭으로 아보카도를 심어보세요.", 999f);
        while (Abocado.instances[0].Level != EnumData.Abocado.Seed) yield return waitForSeconds;

        message.On("하루가 지나면 아보카도가 성장합니다.", 999f);
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();

        message.On("이틀이 지나면 수확할 수 있습니다.", 999f);
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();

        message.On("다시 F + 클릭으로 아보카도를 수확하세요 !", 999f);
        while (StaticData.Abocado < 1) yield return waitForSeconds;

        message.On("Shop 을 눌러 아보카도를 판매해보세요.", 999f);
        while (StaticData.Garu < 1) yield return waitForSeconds;

        message.On("다시 아보카도를 눌러 타워로 성장시키세요 !", 999f);
        while (ITower.instances.Count < 1) yield return waitForSeconds;

        message.On("타워를 이용해 몰려오는 갱단으로부터 살아남으세요.", 3f);
        date.timeFlow = true;

        while (date.dateTime.Day < 2) yield return waitForSeconds;
        localData.SaveScenario(1);
        message.On("축하합니다 ! 튜토리얼을 클리어하셨습니다.", 1.5f);
        yield return new WaitForSeconds(1.5f);
        message.On("잠시후 메인 화면으로 이동합니다.", 1.5f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main");
    }
}