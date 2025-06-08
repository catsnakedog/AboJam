using Synty.Interface.FantasyWarriorHUD.Samples;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static Sound instance;
    public List<GameObject> audioSources;                           // 각 엔터티의 오디오 소스 집합 게임 오브젝트
    private List<AudioSource> sources = new List<AudioSource>();    // 모든 오디오 소스를 평탄화 한 리스트

    /// <summary>
    /// 각 유닛의 이벤트에 사운드 재생을 바운드합니다.
    /// </summary>
    private void Bound()
    {
        // BGM
        Date.eventDay11 = null;
        Date.eventDay11 += () => SetSourcePlay("bgm_day1", false);
        Date.eventDay11 += () => SetSourcePlay("bgm_day11", true);

        // Player
        ShotGun.eventAttack = null;
        ShotGun.eventAttack += (pos) => PlayClip("shotgun", pos, true);
        Knife.eventAttack = null;
        Knife.eventAttack += (pos) => PlayClip("knife", pos, true);
        Bat.eventAttack = null;
        Bat.eventAttack += (pos) => PlayClip("bat", pos, true);
        Gun.eventAttack = null;
        Gun.eventAttack += (pos) => PlayClip("gun", pos, true);
        Sniper.eventAttack = null;
        Sniper.eventAttack += (pos) => PlayClip("sniper", pos, true);
        Spear.eventAttack = null;
        Spear.eventAttack += (pos) => PlayClip("spear", pos, true);
        Receiver.eventMove = null;
        Receiver.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Farming.eventMove = null;
        Farming.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Skill.eventSkillExplosion = null;
        Skill.eventSkillExplosion += (pos) => PlayClip("magic_explosion", pos, true);
        Skill.eventSkillSuccess = null;
        Skill.eventSkillSuccess += () => PlayClip("magic_circle", Vector3.zero, true);

        // Building
        Auto.eventFire = null;
        Auto.eventFire += (pos) => PlayClip("auto_fire", pos);
        Heal.eventFire = null;
        Heal.eventFire += (pos) => PlayClip("heal_fire", pos);
        ITower.eventDestroy = null;
        ITower.eventDestroy += (pos) => PlayClip("demolish", pos, true);

        // UI
        Date.eventMorning = null;
        Date.eventMorning += () => PlayClip("morning_change", Vector3.zero, true);
        Date.eventSkipSuccess = null;
        Date.eventSkipSuccess += () => PlayClip("skip", Vector3.zero, true);
        Date.eventSkipFail = null;
        Date.eventSkipFail += () => PlayClip("disallow", Vector3.zero, true);
        OnOff.eventClick = null;
        OnOff.eventClick += () => PlayClip("button", Vector3.zero, true);
        Menu.eventClick = null;
        Menu.eventClick += () => PlayClip("button", Vector3.zero, true);
        Farming.eventFarming = null;
        Farming.eventFarming += (pos) => PlayClip("abocado_seed", pos);
        Abocado.eventFarming = null;
        Abocado.eventFarming += (pos) => PlayClip("abocado_seed", pos);
        Abocado.eventHarvest = null;
        Abocado.eventHarvest += () => PlayClip("harvest", Vector3.zero, true);
        Abocado.eventOnClick = null;
        Abocado.eventOnClick += () => PlayClip("abocado_onclick", Vector3.zero, true);
        ITower.eventOnClick = null;
        ITower.eventOnClick += () => PlayClip("tower_onclick", Vector3.zero, true);
        Grow.eventPromotion = null;
        Grow.eventPromotion += () => PlayClip("promotion_tree", Vector3.zero, true);
        Promotion.eventPromotionSuccess = null;
        Promotion.eventPromotionSuccess += () => PlayClip("promotion_tower", Vector3.zero, true);
        Promotion.eventPromotionFail = null;
        Promotion.eventPromotionFail += () => PlayClip("disallow", Vector3.zero, true);
        Reinforcement.eventReinforceSuccess = null;
        Reinforcement.eventReinforceSuccess += () => PlayClip("reinforcement", Vector3.zero, true);
        Reinforcement.eventReinforceFail = null;
        Reinforcement.eventReinforceFail += () => PlayClip("disallow", Vector3.zero, true);
        BTN_Upgrade.eventUpgradeSuccess = null;
        BTN_Upgrade.eventUpgradeSuccess += () => PlayClip("upgrade", Vector3.zero, true);
        BTN_Upgrade.eventUpgradeFail = null;
        BTN_Upgrade.eventUpgradeFail += () => PlayClip("disallow", Vector3.zero, true);
        BTN_HP.eventUpgradeSuccess = null;
        BTN_HP.eventUpgradeSuccess += () => PlayClip("upgrade", Vector3.zero, true);
        BTN_HP.eventUpgradeFail = null;
        BTN_HP.eventUpgradeFail += () => PlayClip("disallow", Vector3.zero, true);
        BTN_Weapons.eventBuySuccess = null;
        BTN_Weapons.eventBuySuccess += () => PlayClip("purchase_weapon", Vector3.zero, true);
        BTN_Weapons.eventBuyFail = null;
        BTN_Weapons.eventBuyFail += () => PlayClip("disallow", Vector3.zero, true);
        BTN_Weapons.eventEquip = null;
        BTN_Weapons.eventEquip += (weapon) => PlayClip("Equip_" + weapon.ToString(), Vector3.zero, true);
        Change.eventTradeSuccess = null;
        Change.eventTradeSuccess += () => PlayClip("exchange", Vector3.zero, true);
        Change.eventTradeFail = null;
        Change.eventTradeFail += () => PlayClip("disallow", Vector3.zero, true);
        SampleButtonAction.eventSkillFail = null;
        SampleButtonAction.eventSkillFail += () => PlayClip("disallow", Vector3.zero, true);
        Demolition.eventDemolish = null;
        Demolition.eventDemolish += () => PlayClip("demolish", Vector3.zero, true);
        Farming.eventDig = null;
        Farming.eventDig += (isPlay) => SetSourcePlay("dig", isPlay);
    }

    /* Initialize */
    /// <summary>
    /// audioSources 를 평탄화하여 sources 에 연결합니다.
    /// </summary>
    private void Init()
    {
        sources.Clear();

        foreach (var item in audioSources)
        {
            foreach (var source in item.GetComponents<AudioSource>())
            {
                sources.Add(source);
            }
        }
    }
    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        Init();
        Bound();
    }

    /* Private Method */
    /// <summary>
    /// 오디오 클립을 특정 위치에서 재생합니다.
    /// </summary>
    private void PlayClip(string clipName, Vector3 pos, bool global = false)
    {
        foreach (AudioSource source in sources)
        {
            if (source.clip != null && source.clip.name == clipName)
            {
                if (!global)
                {
                    AudioSource.PlayClipAtPoint(source.clip, pos);
                    break;
                }
                else
                {
                    source.spatialBlend = 0f;
                    source.Play();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 오디오 소스를 재생하거나 중단합니다.
    /// </summary>
    private void SetSourcePlay(string clipName, bool isPlay)
    {
        foreach (AudioSource source in sources)
        {
            if (source.clip != null && source.clip.name == clipName)
            {
                if (isPlay) { if (!source.isPlaying) source.Play(); }
                else source.Stop();
                break;
            }
        }
    }
}
