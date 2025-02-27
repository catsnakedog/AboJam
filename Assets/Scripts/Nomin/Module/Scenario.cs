using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static EnumData;
using static ObjectPool;
using static UnityEngine.EventSystems.EventTrigger;
using Image = UnityEngine.UI.Image;

public class Scenario : MonoBehaviour
{
    /* Dependency */
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject shop_onOff;
    [SerializeField] private GameObject shop_change;
    [SerializeField] private GameObject shop_close;
    [SerializeField] private GameObject promotion;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject btn_skill;
    [SerializeField] private GameObject swap;
    [SerializeField] private GameObject skip;
    [SerializeField] private GameObject buy;
    [SerializeField] private AnimationClick animationClick;
    [SerializeField] private Player player;
    [SerializeField] private HP playerHP;
    private Date date => Date.instance;
    private Message message => Message.instance;
    private Mark mark => Mark.instance;
    private GlobalLight globalLight => GlobalLight.instance;
    private LocalData localData => LocalData.instance;
    private Spawner spawner => Spawner.instance;
    private Grid grid => Grid.instance;
    private Pool pool => Pool.instance;
    private Reinforcement reinforcement => Reinforcement.instance;
    private Zoom zoom => Zoom.instance;
    private Verdict verdict => Verdict.instance;
    private AnimationCameraShake animationCameraShake => AnimationCameraShake.instance;
    private List<Abocado> abocados => Abocado.instances;
    private List<BTN_Weapons> btn_weapons_range => BTN_Weapons.instances_range;
    private List<BTN_Weapons> btn_weapons_melee => BTN_Weapons.instances_melee;
    private List<BTN_Upgrade> btn_upgrades => BTN_Upgrade.instances;

    /* Field & Property */
    public static Scenario instance;
    private float checkTime = 0.03f;
    private WaitForSeconds waitForSeconds;
    private string[] startMessage =
    {
        "황금빛 들판이 넘쳐나길 바랍니다!",
        "올해도 풍년이 들어 기쁨이 가득하길!",
        "땀 흘린 만큼 풍성한 결실을 맺길 바랍니다!",
        "씨앗 하나가 백 배의 결실로 돌아오길!",
        "하늘은 스스로 돕는 자를 돕는다! 대풍을 기원합니다!",
        "올해 농사는 대박! 풍년만 가득하길!",
        "정성 들인 만큼 풍요로운 결실을 맺길 기원합니다!",
        "비는 적당히, 햇볕은 넉넉히! 농사일이 순탄하길!",
        "올 한 해도 풍년! 힘내세요!",
        "좋은 날씨와 함께 풍작의 기쁨이 함께하길!",
        "풍년이 들어 모든 이가 넉넉한 한 해가 되길!",
        "한 톨의 씨앗이 천 배의 결실로 돌아오길!",
        "바람도 순하고, 비도 적당히 내려 풍년이 들길!",
        "올해는 병충해 없이 건강한 농작물이 자라길!",
        "수확의 계절이 기쁨으로 가득하길 바랍니다!",
        "노력한 만큼 풍성한 결실을 맺길 기원합니다!",
        "대지의 축복이 가득한 한 해가 되길 바랍니다!",
        "땅이 비옥하고 햇살이 따스하여 풍작이 들길!",
        "올해도 풍년이 들어 웃음꽃이 가득하길!",
        "모든 농부들의 노력이 결실을 맺는 한 해가 되길!",
        "비옥한 땅과 정성 어린 손길로 가득 찬 한 해가 되길!",
        "흉년 없이 모든 작물이 건강하게 자라길 바랍니다!",
        "한 해 동안 노력한 만큼 풍성한 수확을 거두길!",
        "황금빛 곡식들이 넘실거리는 풍경이 펼쳐지길!",
        "농사의 보람을 느낄 수 있는 한 해가 되길!",
        "하늘도 돕고 땅도 돕는 풍년이 되길 바랍니다!",
        "씨 뿌린 대로 거두는 기쁨을 누리는 한 해가 되길!",
        "햇빛은 따뜻하고 비는 넉넉하여 풍년을 이루길!",
        "올해는 가뭄 없이 모든 것이 순조롭길 바랍니다!",
        "농작물 하나하나가 튼튼하고 실하게 자라길!",
        "가을 들판이 황금빛으로 물드는 풍경을 볼 수 있길!",
        "올해도 풍작이 들어 모두가 넉넉하길 바랍니다!",
        "노력한 만큼 좋은 결실을 맺는 기쁜 한 해가 되길!",
        "햇살과 비가 적당히 내려 최고의 수확을 이루길!",
        "모든 작물이 무럭무럭 자라 풍성한 수확이 있길!",
        "농부들의 땀이 헛되지 않는 대풍의 해가 되길!",
        "씨 뿌린 땅에서 싹이 튼튼하게 올라오길 바랍니다!",
        "들녘이 곡식으로 가득 차는 풍년을 기원합니다!",
        "추수의 계절이 넉넉한 행복을 가져오길 바랍니다!",
        "농사일이 순조롭게 흘러가고 풍작이 들길 바랍니다!",
        "씨앗 하나하나가 알찬 결실을 맺는 한 해가 되길!",
        "땅과 하늘이 농부들에게 축복을 주는 한 해가 되길!",
        "수확의 계절이 기쁨과 감사로 가득 차길 바랍니다!",
        "올해는 기후가 순조롭고 농사도 무사히 끝나길 바랍니다!",
        "한 해 동안 고생한 만큼 좋은 결실을 맺길 바랍니다!",
        "풍작이 들어 모두가 넉넉하고 행복한 한 해가 되길!",
        "노력의 결실이 풍성한 곡식으로 돌아오길 바랍니다!",
        "올해도 가득 찬 창고와 기쁨 가득한 수확이 있길 바랍니다!",
        "정성 들여 키운 농작물이 모두 건강하게 자라길 바랍니다!",
        "기후가 농사에 적합하여 최고의 결실을 맺길 바랍니다!",
        "올해 농사는 가뭄도 홍수도 없이 순조롭게 되길 바랍니다!",
        "농부들의 손길이 닿는 곳마다 풍성한 결실이 있길 바랍니다!",
        "기쁨이 가득한 수확의 계절을 맞이하길 바랍니다!",
        "비와 바람이 알맞게 내려 풍년을 이루길 바랍니다!",
        "넉넉한 수확과 함께 행복한 한 해가 되길 바랍니다!",
        "씨를 뿌린 만큼 풍성한 결실을 맺는 기쁜 한 해가 되길!",
        "좋은 흙과 적당한 비가 만나 풍성한 곡식을 키우길 바랍니다!",
        "수확의 기쁨이 넉넉한 웃음으로 이어지길 바랍니다!",
        "올해 농사는 어려움 없이 무사히 잘 마무리되길 바랍니다!",
        "땅이 농부의 정성을 알아 풍성한 수확을 내어주길 바랍니다!",
        "모든 것이 순조롭고 농사일이 수월하게 이루어지길 바랍니다!",
        "올해는 누구나 풍요로운 한 해가 되길 바랍니다!",
        "농부들의 노력이 헛되지 않는 한 해가 되길 바랍니다!",
        "모든 농작물이 튼튼하고 실하게 자라길 바랍니다!",
        "기쁨 가득한 수확의 계절을 맞이하길 바랍니다!",
        "날씨가 순조롭고 농사가 풍년이 되길 바랍니다!",
        "넉넉한 수확과 함께 기쁨이 가득한 한 해가 되길 바랍니다!",
        "씨를 뿌린 대로 풍성한 수확을 거두는 한 해가 되길 바랍니다!",
        "농부들의 정성이 헛되지 않는 풍작의 해가 되길 바랍니다!",
        "농사의 기쁨이 널리 퍼지는 한 해가 되길 바랍니다!",
        "올해는 병해충 없이 농작물이 건강하게 자라길 바랍니다!",
        "수확의 기쁨이 더욱 커지는 한 해가 되길 바랍니다!",
        "대지의 축복이 가득한 해가 되길 바랍니다!",
        "농부들의 땀방울이 결실을 맺는 한 해가 되길 바랍니다!",
        "한 해의 노력이 풍요로운 결실로 돌아오길 바랍니다!",
        "모든 농작물이 튼튼하고 건강하게 자라길 바랍니다!",
        "기후가 순조롭고 농사가 대풍을 이루길 바랍니다!",
        "정성 들여 가꾼 농작물이 풍성하게 열리길 바랍니다!",
        "올해는 풍년이 들어 모두가 넉넉한 한 해가 되길 바랍니다!",
        "햇빛과 비가 조화를 이루어 풍요로운 수확을 이루길 바랍니다!",
        "한 해 동안의 노력과 정성이 아름다운 결실로 돌아오길 바랍니다!",
        "비와 바람이 순조롭게 불어 농사가 성공적으로 마무리되길 바랍니다!",
        "올해는 모든 작물이 건강하고 튼튼하게 자라길 바랍니다!",
        "한 해 동안 애쓴 농부들의 노력이 결실을 맺길 바랍니다!",
        "농부들의 정성이 빛을 발하는 풍년이 되길 바랍니다!",
        "밭에서 자란 모든 작물이 건강하고 알차게 열리길 바랍니다!",
        "풍요로운 한 해, 넉넉한 수확이 있길 바랍니다!"
    }; // Generated By GPT (스크롤 압박 주의)

    /* Initializer & Finalizer & Updater */
    private void Awake() { instance = this; waitForSeconds = new WaitForSeconds(checkTime); }
    private void Start()
    {
        ITower.instances.Clear();
        IEnemy.instances.Clear();

        switch (localData.LoadScenario())
        {
            case 0:
                StartCoroutine(CorScenario_0());
                break;
            case 1:
                StartCoroutine(CorScenario_1());
                break;
            case 2:
                StartCoroutine(CorScenario_2());
                break;
            default: // 본 게임 시작
                message.On(startMessage[UnityEngine.Random.Range(0, startMessage.Length)], 3f);
                break;
        }
    }

    /// <summary>
    /// 시나리오 0 데몬입니다.
    /// </summary>
    public IEnumerator CorScenario_0()
    {
        // 초기 자원
        StaticData.Abocado = 1;
        StaticData.Garu = 5;
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

        // 경작
        message.On($"반가워요 ! 아보카도 농장에 오신걸 환영해요.", 2f, true);
        yield return new WaitForSeconds(2f);
        message.On($"WASD 로 움직일 수 있어요.", 2f, true);
        yield return new WaitForSeconds(2f);
        message.On($"F 를 꾸욱 누르고 땅을 클릭해 보세요 !", 999f, true);
        while (Abocado.instances.Count < 1) yield return waitForSeconds;

        // 심기
        message.On("잘 했어요 ! 다시 F + 클릭으로 아보카도를 심어보세요.", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (Abocado.instances[0].Level != EnumData.Abocado.Seed) yield return waitForSeconds;

        // 성장
        message.On("하루가 지나면 아보카도가 성장합니다.", 999f, true);
        mark.Off();
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();
        message.On("이틀이 지나면 수확할 수 있습니다.", 999f, true);
        globalLight.Set(globalLight.night, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.sunset, 0.01f);
        yield return new WaitForSeconds(1.5f);
        globalLight.Set(globalLight.morning, 0.01f);
        yield return new WaitForSeconds(1.5f);
        Abocado.instances[0].GrowUp();

        // 수확
        message.On("F + 클릭으로 아보카도를 수확하세요 !", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (StaticData.Abocado < 1) yield return waitForSeconds;

        // 상점
        message.On("Shop 을 눌러보세요.", 999f, true);
        shop.SetActive(true);
        mark.On(shop_onOff, 999f);
        while (!shop_change.activeInHierarchy) yield return waitForSeconds;

        // 판매
        message.On("아보카도를 판매하세요.", 999f, true);
        mark.On(shop_change, 999f);
        RemoveButtonEventRecursive(buy);
        UnityAction action = () => message.On("다른 버튼은 튜토리얼 이후 누를 수 있어요.", 2f, true);
        AddButtonEventRecursive(buy, action);
        while (StaticData.Garu < 1)
        {
            if (StaticData.Abocado < 1)
            {
                message.On("어? 아보카도 어쨌어요...", 999f, true);
                yield return new WaitForSeconds(2f);
                message.On("진짜 딱 하나만 더 줄게요 !", 999f, true);
                yield return new WaitForSeconds(2f);
                StaticData.Abocado++;
                message.On("아보카도를 판매하세요.", 999f, true);
            }

            yield return waitForSeconds;
        }
        mark.On(shop_close, 999f);
        while (shop_close.activeInHierarchy) yield return waitForSeconds;

        // 타워
        message.On("F + 클릭으로 아보카도를 다시 눌러보세요 !", 999f, true);
        mark.On(Abocado.instances[0].gameObject, 999f);
        while (!promotion.activeInHierarchy) yield return waitForSeconds;
        message.On("아보카도를 타워로 성장시키세요 !", 999f, true);
        WaitForSeconds tempWaitForSeconds = new WaitForSeconds(0.3f);
        CheckTowerCount(out int towerCount);
        while (towerCount < 1)
        {
            if (StaticData.Garu < 100) StaticData.Garu = 100;
            if (promotion.activeInHierarchy)
                foreach (Transform child in promotion.transform)
                {
                    yield return tempWaitForSeconds;
                    if (ITower.instances.Count > 0) break;
                    mark.On(child.gameObject, 1f);
                }
            else mark.On(Abocado.instances[0].gameObject, 999f);

            yield return tempWaitForSeconds;
            CheckTowerCount(out towerCount);
        }
        message.On("모든 종류의 타워를 건설해보세요.", 999f, true);
        mark.Off();
        StaticData.Abocado++;
        StaticData.Garu = 999;
        tempWaitForSeconds = new WaitForSeconds(0.1f);
        Coroutine corDayNight = StartCoroutine(CorDayNight());
        while (!CheckTowerCount(out int notUse))
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

        // 교전
        message.On("타워를 이용해 몰려오는 갱단으로부터 살아남으세요.", 3f, true);
        StopCoroutine(corDayNight);
        mark.Off();
        globalLight.Set(globalLight.night, 0.01f);
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, enemies.Length);
            spawner.Spawn(enemies[random], new Vector3(i * 2, 13, 0));
            spawner.Spawn(enemies[random], new Vector3(-i * 2, -13, 0));
        }

        // 클리어
        while (CheckEnemyAlive() && playerHP.HP_current > 0) yield return waitForSeconds;
        globalLight.Set(globalLight.morning, 0.01f);
        yield return StartCoroutine(Clear());
    }
    /// <summary>
    /// 시나리오 1 데몬입니다.
    /// </summary>
    public IEnumerator CorScenario_1()
    {
        // 초기 자원
        StaticData.Abocado = 0;
        StaticData.Garu = 999;
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

        message.On($"오늘은 각종 무기 사용법을 알아봐요.", 2f, true);
        yield return new WaitForSeconds(2f);

        // 상점
        message.On("Shop 을 눌러보세요.", 999f, true);
        shop.SetActive(true);
        mark.On(shop_onOff, 999f);
        while (!shop_change.activeInHierarchy) yield return waitForSeconds;

        // 구매
        message.On("모든 무기를 구매하세요 !", 999f, true);
        WaitForSeconds tempWaitForSeconds = new WaitForSeconds(0.3f);
        List<BTN_Weapons> weapons = new();
        weapons.AddRange(btn_weapons_range);
        weapons.AddRange(btn_weapons_melee);
        EnumData.Weapon[] weaponOrder = { EnumData.Weapon.Gun, EnumData.Weapon.ShotGun, EnumData.Weapon.Sniper,
                                           EnumData.Weapon.Riple, EnumData.Weapon.Knife, EnumData.Weapon.Bat,
                                           EnumData.Weapon.Spear, EnumData.Weapon.ChainSaw };
        weapons = weapons.OrderBy(weapon => Array.IndexOf(weaponOrder, weapon.Weapon)).ToList();
        while (!CheckBuyAllWeapons())
        {
            if (shop_change.activeInHierarchy)
                foreach (BTN_Weapons weapon in weapons)
                {
                    if (CheckBuyAllWeapons()) break;
                    if (weapon.Purchase) continue;
                    yield return tempWaitForSeconds;
                    mark.On(weapon.gameObject, 999f);
                }
            else mark.On(shop_onOff, 2f);

            yield return waitForSeconds;
        }

        // 장착
        message.On("근거리 / 원거리 무기를 각각 장착해보세요.", 999f, true);
        swap.SetActive(true);
        while (!CheckEquip())
        {
            if (shop_change.activeInHierarchy)
                foreach (BTN_Weapons weapon in weapons)
                {
                    if (CheckEquip()) break;
                    yield return tempWaitForSeconds;
                    mark.On(weapon.gameObject, 999f);
                }
            else mark.On(shop_onOff, 2f);

            yield return waitForSeconds;
        }

        // 강화
        message.On("모든 강화를 완료하세요 !", 999f, true);
        string[] upgradeOrder = { "Upgrade_Damage", "Upgrade_Rate", "Upgrade_Range", "Upgrade_Knockback" };
        List<BTN_Upgrade> upgrades = btn_upgrades.OrderBy(upgrade => Array.IndexOf(upgradeOrder, upgrade.ID)).ToList();
        while (!CheckBuyAllUpgrades())
        {
            if (shop_change.activeInHierarchy)
                foreach (BTN_Upgrade upgrade in upgrades)
                {
                    if (CheckBuyAllUpgrades()) break;
                    if (upgrade.Level < Mathf.Min(upgrade.MaxLevel, 3) == false) continue;
                    yield return tempWaitForSeconds;
                    mark.On(upgrade.gameObject, 999f);
                }
            else mark.On(shop_onOff, 2f);

            yield return waitForSeconds;
        }

        // 교체
        swap.SetActive(true);
        message.On("근접 / 원거리 무기로 교체할 수 있어요.", 3f, true);
        mark.On(swap, 999f);
        while (player.Hand.CurrentSlotIndex == Hand.WeaponSlot.FirstRanged) yield return waitForSeconds;

        // 교전
        message.On($"3 킬 이상 성공하세요 !", 999f, true);
        mark.Off();
        globalLight.Set(globalLight.night, 0.01f);
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, enemies.Length);
            spawner.Spawn(enemies[random], new Vector3(i * 2, 13, 0));
            spawner.Spawn(enemies[random], new Vector3(-i * 2, -13, 0));
        }
        while (StaticData.gameData.kill <= 2) yield return waitForSeconds;

        // 스킬
        message.On($"기도하면 비가 올지도 몰라요 !", 5f, true);
        btn_skill.SetActive(true);
        mark.On(btn_skill, 3f);

        // 클리어
        while (CheckEnemyAlive() && playerHP.HP_current > 0) yield return waitForSeconds;
        globalLight.Set(globalLight.morning, 0.01f);
        yield return StartCoroutine(Clear());
    }
    /// <summary>
    /// 시나리오 2 데몬입니다.
    /// </summary>
    public IEnumerator CorScenario_2()
    {
        // 초기 자원
        StaticData.Abocado = 0;
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

        // 타워 설치
        grid.GetTile((15, 17)).Bind(pool.Get("Abocado"), EnumData.TileIndex.AboCado);
        grid.GetTile((15, 19)).Bind(pool.Get("Auto"), EnumData.TileIndex.Auto);

        // 철거
        message.On($"마지막 튜토리얼입니다 !", 2f, true);
        yield return new WaitForSeconds(2f);
        message.On($"G + 클릭으로 타워와 아보카도를 철거해보세요.", 999f, true);
        while (grid.GetTile((15, 17)).Go != null | grid.GetTile((15, 19)).Go != null) yield return waitForSeconds;
        message.On($"잘했어요 !", 1f, true);
        yield return new WaitForSeconds(1f);

        // 타워 업그레이드
        globalLight.Set(globalLight.sunset, 0.01f);
        message.On($"곧 적들이 몰려올 것 같은데..", 2f, true);
        yield return new WaitForSeconds(2f);
        message.On("타워를 하나 드릴게요. 고치면 쓸 만 할거에요.", 2f, true);
        grid.GetTile((15, 19)).Bind(pool.Get("Auto"), EnumData.TileIndex.Auto);
        HP hpAuto = HP.FindHP(grid.GetTile((15, 19)).Go);
        hpAuto.Damage(hpAuto.Hp_max * 0.5f);
        yield return new WaitForSeconds(2f);
        message.On($"F + 클릭으로 타워를 눌러보세요.", 999f, true);
        if(grid.GetTile((15, 19)).Go != null) mark.On(grid.GetTile((15, 19)).Go, 999f);
        while (!reinforcement.gameObject.activeSelf)
        {
            if(grid.GetTile((15, 19)).Go == null)
            {
                mark.Off();

                message.On("그.. 그걸... 철거해버리면...", 2f, true);
                yield return new WaitForSeconds(2f);
                message.On("큰 맘 먹고 사준건데.........", 2f, true);
                yield return new WaitForSeconds(2f);
                message.On("딱밤 한 대만 때릴게요....", 2f, true);
                yield return new WaitForSeconds(0.5f);
                animationCameraShake.StartShake();
                playerHP.Damage(playerHP.HP_current * 0.5f);
                yield return new WaitForSeconds(1.5f);
                grid.GetTile((15, 19)).Bind(pool.Get("Auto"), EnumData.TileIndex.Auto);
                HP tempHP = HP.FindHP(grid.GetTile((15, 19)).Go);
                tempHP.Damage(hpAuto.Hp_max * 0.5f);

                mark.On(grid.GetTile((15, 19)).Go, 999f);
            }
            yield return waitForSeconds;
        }

        message.On($"타워를 2 번 이상 업그레이드 하세요.", 999f, true);
        StaticData.Garu = 999;
        WaitForSeconds tempWaitForSeconds = new WaitForSeconds(0.3f);
        while (true)
        {
            if (grid.GetTile((15, 19)).Go == null)
            {
                mark.Off();

                message.On("그.. 그걸... 철거해버리면...", 2f, true);
                yield return new WaitForSeconds(2f);
                message.On("큰 맘 먹고 사준건데.........", 2f, true);
                yield return new WaitForSeconds(2f);
                message.On("딱밤 한 대만 때릴게요....", 2f, true);
                yield return new WaitForSeconds(0.5f);
                animationCameraShake.StartShake();
                playerHP.Damage(playerHP.HP_current * 0.5f);
                yield return new WaitForSeconds(1.5f);
                grid.GetTile((15, 19)).Bind(pool.Get("Auto"), EnumData.TileIndex.Auto);
                HP tempHP = HP.FindHP(grid.GetTile((15, 19)).Go);
                tempHP.Damage(hpAuto.Hp_max * 0.5f);

                mark.On(grid.GetTile((15, 19)).Go, 999f);
            }

            bool checkTowerUpgrade = true;

            // 타워 or 업그레이드 버튼 마킹
            if (reinforcement.gameObject.activeSelf) mark.On(reinforcement.BTN_reinforce.gameObject, 999f);
            else mark.On(grid.GetTile((15, 19)).Go, 999f);

            // 업그레이드 했는지 검사
            if (grid.GetTile((15, 19)).Go.GetComponent<Auto>().Level < 2) checkTowerUpgrade = false;
            if (checkTowerUpgrade) break;

            yield return tempWaitForSeconds;
        }

        // 카메라 줌
        mark.Off();
        globalLight.Set(globalLight.night, 0.01f);
        message.On("곧 적들이 몰려옵니다...", 1.5f);
        yield return new WaitForSeconds(1.5f);
        message.On("마우스 휠을 스크롤 해 시야를 넓혀두세요 !", 999f);
        while (zoom.Target_zoom > zoom.min_zoom) yield return waitForSeconds;

        // 교전
        message.On($"모든 적을 무찌르세요.", 2f, true);
        globalLight.Set(globalLight.night, 0.01f);
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, enemies.Length);
            spawner.Spawn(enemies[random], new Vector3(i * 2, 13, 0));
            spawner.Spawn(enemies[random], new Vector3(-i * 2, -13, 0));
        }
        while (verdict.CheckMob()) yield return waitForSeconds;

        // 클리어
        globalLight.Set(globalLight.morning, 0.01f);
        message.On("해냈군요 ! 다음부턴 진짜 실전이에요 !", 2f);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Clear());
    }

    /* Private Method */
    /// <summary>
    /// 현재 시나리오를 클리어합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Clear()
    {
        localData.SaveScenario(localData.LoadScenario() + 1);
        message.On("축하합니다 ! 튜토리얼을 클리어하셨습니다.", 1.5f, true);
        yield return new WaitForSeconds(1.5f);
        message.On("잠시후 메인 화면으로 이동합니다.", 1.5f, true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Main");
    }
    /// <summary>
    /// 낮과 밤을 교차시키며 아보카도를 성장시킵니다.
    /// </summary>
    private IEnumerator CorDayNight()
    {
        while (true)
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
    /// 모든 무기를 구매했는지 여부를 반환합니다.
    /// </summary>
    private bool CheckBuyAllWeapons()
    {
        bool flag = true;

        List<BTN_Weapons> weapons = new();
        weapons.AddRange(btn_weapons_melee);
        weapons.AddRange(btn_weapons_range);
        foreach (BTN_Weapons weapon in weapons) if (!weapon.Purchase) { flag = false; break; }

        return flag;
    }
    /// <summary>
    /// 모든 업그레이드를 완료했는지 여부를 반환합니다.
    /// </summary>
    private bool CheckBuyAllUpgrades()
    {
        bool flag = true;

        foreach (BTN_Upgrade upgrade in btn_upgrades) if (upgrade.Level < upgrade.MaxLevel) { flag = false; break; }

        return flag;
    }
    /// <summary>
    /// 기본 이외 무기를 장착했는지 여부를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool CheckEquip()
    {
        bool flag = true;

        if (player.Hand.FirstSlot == EnumData.Weapon.Gun) flag = false;
        if (player.Hand.SecondSlot == EnumData.Weapon.Knife) flag = false;

        return flag;
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
    private bool CheckTowerCount(out int towerCount)
    {
        int autoCount = 0;
        int splashCount = 0;
        int healCount = 0;
        int guardCount = 0;
        int productionCount = 0;

        // 각 타워 개수 세기
        foreach (var item in abocados) if (item.Quality > 0) productionCount++;
        foreach (ITower tower in ITower.instances)
        {
            if (ReferenceEquals(tower, null)) continue;
            if (tower is Tower<Table_Auto, Record_Auto>) { Tower<Table_Auto, Record_Auto> auto = tower as Tower<Table_Auto, Record_Auto>; if (auto.gameObject.activeSelf) autoCount++; }
            if (tower is Tower<Table_Splash, Record_Splash>) { Tower<Table_Splash, Record_Splash> splash = tower as Tower<Table_Splash, Record_Splash>; if (splash.gameObject.activeSelf) splashCount++; }
            if (tower is Tower<Table_Heal, Record_Heal>) { Tower<Table_Heal, Record_Heal> heal = tower as Tower<Table_Heal, Record_Heal>; if (heal.gameObject.activeSelf) healCount++; }
            if (tower is Tower<Table_Guard, Record_Guard>) { Tower<Table_Guard, Record_Guard> gurad = tower as Tower<Table_Guard, Record_Guard>; if (gurad.gameObject.activeSelf) guardCount++; }
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
        towerCount = autoCount + splashCount + healCount + guardCount + productionCount;
        if (autoCount > 0 && splashCount > 0 && healCount > 0 && guardCount > 0 && productionCount > 0) return true;
        else return false;
    }
    /// <summary>
    /// 필드에 남은 적이 있는지 검사합니다.
    /// </summary>
    private bool CheckEnemyAlive()
    {
        int gunnerCount = 0;
        int thugCount = 0;
        int leopardCount = 0;

        // 각 몬스터 개수 세기
        foreach (IEnemy enemy in IEnemy.instances)
        {
            if (ReferenceEquals(enemy, null)) continue;
            if (enemy is Enemy<Table_Gunner, Record_Gunner>) { Enemy<Table_Gunner, Record_Gunner> gunner = enemy as Enemy<Table_Gunner, Record_Gunner>; if (gunner.gameObject.activeSelf) gunnerCount++; }
            if (enemy is Enemy<Table_Thug, Record_Thug>) { Enemy<Table_Thug, Record_Thug> thug = enemy as Enemy<Table_Thug, Record_Thug>; if (thug.gameObject.activeSelf) thugCount++; }
            if (enemy is Enemy<Table_Leopard, Record_Leopard>) { Enemy<Table_Leopard, Record_Leopard> leopard = enemy as Enemy<Table_Leopard, Record_Leopard>; if (leopard.gameObject.activeSelf) leopardCount++; }
        }

        int enemyCount = gunnerCount + thugCount + leopardCount;
        if (enemyCount == 0) return false;
        else return true;
    }
}