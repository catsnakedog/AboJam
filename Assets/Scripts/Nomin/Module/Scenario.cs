using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Image = UnityEngine.UI.Image;

public class Scenario : MonoBehaviour
{
    /* Dependency */
    [SerializeField] GameObject shop_onOff;
    [SerializeField] GameObject shop_change;
    [SerializeField] GameObject shop_close;
    [SerializeField] GameObject promotion;
    [SerializeField] GameObject shop;
    [SerializeField] GameObject btn_skill;
    [SerializeField] GameObject swap;
    [SerializeField] GameObject skip;
    [SerializeField] GameObject buy;
    [SerializeField] AnimationClick animationClick;
    private Date date => Date.instance;
    private Message message => Message.instance;
    private Mark mark => Mark.instance;
    private GlobalLight globalLight => GlobalLight.instance;
    private LocalData localData => LocalData.instance;
    private List<Abocado> abocados => Abocado.instances;

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
        // 초기 자원
        StaticData.Abocado = 1;
        StaticData.Garu = 0;
        StaticData.Water = 0;

        // 시간 초기화
        date.dateTime = DateTime.MinValue;
        date.timeFlow = false;
        date.text_day.text = $"DAY - 1";
        date.text_time.text = $"0:0";

        // UI 조정
        shop.SetActive(false);
        btn_skill.SetActive(false);
        swap.SetActive(false);
        skip.SetActive(false);

        //
        message.On($"반가워요 ! 아보카도 농장에 오신걸 환영해요.", 2f, true);
        yield return new WaitForSeconds(2f);
        message.On($"F 를 꾸욱 누르고 땅을 클릭해 보세요 !", 999f, true);
        while (Abocado.instances.Count < 1) yield return waitForSeconds;

        //
        message.On("잘 했어요 ! 다시 F + 클릭으로 아보카도를 심어보세요.", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (Abocado.instances[0].Level != EnumData.Abocado.Seed) yield return waitForSeconds;

        //
        message.On("하루가 지나면 아보카도가 성장합니다.", 999f, true);
        mark.Off();
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();

        //
        message.On("이틀이 지나면 수확할 수 있습니다.", 999f, true);
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();

        //
        message.On("F + 클릭으로 아보카도를 수확하세요 !", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (StaticData.Abocado < 1) yield return waitForSeconds;

        //
        message.On("Shop 을 눌러보세요.", 999f, true);
        shop.SetActive(true);
        mark.On(shop_onOff, 999f);
        while (!shop_change.activeInHierarchy) yield return waitForSeconds;

        //
        message.On("아보카도를 판매하세요.", 999f, true);
        mark.On(shop_change, 999f);
        RemoveButtonEventRecursive(buy);
        UnityAction action = () => message.On("다른 버튼은 튜토리얼 이후 누를 수 있어요.", 2f, true);
        AddButtonEventRecursive(buy, action);
        while (StaticData.Garu < 1) yield return waitForSeconds;

        //
        mark.On(shop_close, 999f);
        while (shop_close.activeInHierarchy) yield return waitForSeconds;

        //
        message.On("F + 클릭으로 아보카도를 다시 눌러보세요 !", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (!promotion.activeInHierarchy) yield return waitForSeconds;

        //
        message.On("아보카도를 타워로 성장시키세요 !", 999f, true);
        WaitForSeconds tempWaitForSeconds = new WaitForSeconds(0.3f);
        while (ITower.instances.Count < 1)
        {
            foreach (Transform child in promotion.transform)
            {
                yield return tempWaitForSeconds;
                if (ITower.instances.Count > 0) break;
                mark.On(child.gameObject, 999f);
            }
            yield return waitForSeconds;
        }

        message.On("모든 종류의 타워를 건설해보세요.", 999f, true);
        mark.Off();
        StaticData.Abocado++;
        tempWaitForSeconds = new WaitForSeconds(0.1f);

        Coroutine corDayNight = StartCoroutine(CorDayNight());
        while (!CheckTowerCount())
        {
            if (CheckPoor())
            {
                message.On("이런.. 씨앗을 다 써버리셨군요 !", 999f, true);
                yield return new WaitForSeconds(2f);
                message.On("처음이니 하나만 더 드릴게요.", 999f, true);
                yield return new WaitForSeconds(2f);
                StaticData.Abocado++;
                message.On("모든 종류의 타워를 건설해보세요.", 999f, true);
            }

            yield return tempWaitForSeconds;
        }

        //
        message.On("타워를 이용해 몰려오는 갱단으로부터 살아남으세요.", 3f, true);
        StopCoroutine(corDayNight);
        globalLight.Set(globalLight.morning, 0.01f);
        mark.Off();
        date.timeFlow = true;
        int origin = date.secondsPerDay;
        date.secondsPerDay = 10;
        while (date.gameTime != Date.GameTime.Night) yield return waitForSeconds;
        yield return new WaitForSeconds(3f);

        //
        swap.SetActive(true);
        message.On("근접 무기로 교체할 수도 있습니다.", 4f, true);
        mark.On(swap, 2f);
        yield return new WaitForSeconds(2f);

        //
        btn_skill.SetActive(true);
        message.On("버거우면 마법진을 펼쳐보세요 !", 4f, true);
        mark.On(btn_skill, 2f);

        //
        while (date.dateTime.Day < 2) yield return waitForSeconds;
        localData.SaveScenario(1);
        message.On("축하합니다 ! 튜토리얼을 클리어하셨습니다.", 1.5f, true);
        yield return new WaitForSeconds(1.5f);
        message.On("잠시후 메인 화면으로 이동합니다.", 1.5f, true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main");
    }

    /* Private Method */
    /// <summary>
    /// 특정 게임오브젝트 하위의 모든 버튼에서 이벤트를 제거합니다.
    /// </summary>
    private void RemoveButtonEventRecursive(GameObject parent)
    {
        if (parent == null) return;
        UnityEngine.UI.Button button = parent.GetComponent<UnityEngine.UI.Button>();
        if (button != null) button.onClick.RemoveAllListeners();

        foreach (Transform child in parent.transform) RemoveButtonEventRecursive(child.gameObject);
    }
    /// <summary>
    /// 특정 게임오브젝트 하위의 모든 버튼에 이벤트를 추가합니다.
    /// </summary>
    private void AddButtonEventRecursive(GameObject parent, UnityEngine.Events.UnityAction callback)
    {
        if (parent == null || callback == null) return;

        UnityEngine.UI.Button button = parent.GetComponent<UnityEngine.UI.Button>();
        if (button != null) button.onClick.AddListener(callback);

        foreach (Transform child in parent.transform) AddButtonEventRecursive(child.gameObject, callback);
    }
    /// <summary>
    /// 아보카도를 생산할 수 있는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool CheckPoor()
    {
        // 남은 아보카도가 있으면 false 리턴
        if (StaticData.Abocado > 0) return false;

        // 생산 가능한 아보카도가 있으면 false 리턴
        foreach (Abocado abocado in Abocado.instances.Where(abocado => abocado.gameObject.activeSelf))
            if (abocado.Level == EnumData.Abocado.Seed ||
                abocado.Level == EnumData.Abocado.Tree ||
                abocado.Level == EnumData.Abocado.Fruited ||
                abocado.Quality > 0)
                return false;

        // 둘 다 없으면 true 리턴
        return true;
    }
    /// <summary>
    /// 모든 종류의 타워를 건설했는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool CheckTowerCount()
    {
        int autoCount = 0;
        int splashCount = 0;
        int healCount = 0;
        int guardCount = 0;
        int productionCount = 0;

        // 각 타워 개수 세기
        foreach (ITower tower in ITower.instances)
        {
            if (tower is Tower<Table_Auto, Record_Auto>) { Tower<Table_Auto, Record_Auto> auto = tower as Tower<Table_Auto, Record_Auto>; if (auto.gameObject.activeSelf) autoCount++; }
            if (tower is Tower<Table_Splash, Record_Splash>) { Tower<Table_Splash, Record_Splash> splash = tower as Tower<Table_Splash, Record_Splash>; if (splash.gameObject.activeSelf) splashCount++; }
            if (tower is Tower<Table_Heal, Record_Heal>) { Tower<Table_Heal, Record_Heal> heal = tower as Tower<Table_Heal, Record_Heal>; if (heal.gameObject.activeSelf) healCount++; }
            if (tower is Tower<Table_Guard, Record_Guard>) { Tower<Table_Guard, Record_Guard> gurad = tower as Tower<Table_Guard, Record_Guard>; if (gurad.gameObject.activeSelf) guardCount++; }
            foreach (var item in abocados) if (item.Quality > 0) productionCount++;
        }

        // 건설된 타워는 Promotion 에서 해당 버튼 제거
        if (autoCount > 0)
            foreach (Transform child in promotion.transform)
                if (child.gameObject.GetComponent<Image>().sprite.name == "Auto") child.gameObject.SetActive(false);
        if (splashCount > 0)
            foreach (Transform child in promotion.transform)
                if (child.gameObject.GetComponent<Image>().sprite.name == "Splash") child.gameObject.SetActive(false);
        if (healCount > 0)
            foreach (Transform child in promotion.transform)
                if (child.gameObject.GetComponent<Image>().sprite.name == "Heal") child.gameObject.SetActive(false);
        if (guardCount > 0)
            foreach (Transform child in promotion.transform)
                if (child.gameObject.GetComponent<Image>().sprite.name == "Guard") child.gameObject.SetActive(false);
        if (productionCount > 0)
            foreach (Transform child in promotion.transform)
                if (child.gameObject.GetComponent<Image>().sprite.name == "Production") child.gameObject.SetActive(false);

        // 모든 타워가 건설되었으면 true 반화
        if (autoCount > 0 && splashCount > 0 && healCount > 0 && guardCount > 0 && productionCount > 0) return true;
        else return false;
    }
    /// <summary>
    /// 낮과 밤을 교차시키며 아보카도를 성장시킵니다.
    /// </summary>
    private IEnumerator CorDayNight()
    {
        while(true)
        {
            globalLight.Set(globalLight.night, 0.01f);
            yield return new WaitForSeconds(1.5f);
            globalLight.Set(globalLight.sunset, 0.01f);
            yield return new WaitForSeconds(1.5f);
            globalLight.Set(globalLight.morning, 0.01f);
            yield return new WaitForSeconds(1.5f);
            foreach (Abocado abocado in Abocado.instances) abocado.GrowUp();
        }
    }
}