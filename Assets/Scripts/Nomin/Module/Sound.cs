using Synty.Interface.FantasyWarriorHUD.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    public static Sound instance;
    public AudioMixer mixer;
    public UnityEngine.UI.Slider bgmSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public List<GameObject> audioSourcesGroup;
    public List<GameObject> loopAudioSourcesGroup;
    private List<AudioSource> sources = new List<AudioSource>();
    private List<LoopAudioSource> loopSources = new List<LoopAudioSource>();

    /// <summary>
    /// 각 유닛의 이벤트에 사운드 재생을 바운드합니다.
    /// </summary>
    private void Bound()
    {
        // BGM
        Date.eventMorning = null;
        Date.eventMorning += (day) =>
        {
            PlayClip("morning_change", Vector3.zero, true);
            SetSourcePlay("bgm_night", false);

            if (1 <= day && day <= 10)
            {
                SetSourcePlay("bgm_day1", true);
            }
            else if (11 <= day && day <= 20)
            {
                SetSourcePlay("bgm_day1", false);
                SetSourcePlay("bgm_day11", true);
            }
            else
            {

            }
        };

        Date.eventNight = null;
        Date.eventNight += () =>
        {
            SetSourcePlay("bgm_day1", false);
            SetSourcePlay("bgm_day11", false);
            SetSourcePlay("bgm_night", true);
        };

        // Player
        Receiver.eventMove = null;
        Receiver.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Farming.eventMove = null;
        Farming.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Skill.eventSkill = null;
        Skill.eventSkill += () => PlayClip("magic_circle", Vector3.zero, true);

        // Launcher
        Launcher.eventFire = null;
        Launcher.eventFire += (ID, pos) => PlayClip(ID, pos);
        Projectile.eventClash = null;
        Projectile.eventClash = (pos) => PlayClip("projectile_clash", pos);

        // Explosion
        Explosion.eventExplode = null;
        Explosion.eventExplode += (explosionID, pos) => PlayClip(explosionID, pos);

        // Melee
        Melee.eventAttack = null;
        Melee.eventAttack += (ID, pos) => { if (!ID.Contains("Chainsaw")) PlayClip(ID, pos); };
        Melee.eventHit = null;
        Melee.eventHit += (ID, pos) => PlayClip(ID + "_Hit", pos);

        // ChainSaw
        ChainSaw.eventStart = null;
        ChainSaw.eventStart += () => SetLoopSource("Melee_Player_Chainsaw", true);
        ChainSaw.eventEnd = null;
        ChainSaw.eventEnd += () => SetLoopSource("Melee_Player_Chainsaw", false);

        // UI
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
        Grow.eventPromotionSuccess = null;
        Grow.eventPromotionSuccess += () => PlayClip("promotion_tree", Vector3.zero, true);
        Grow.eventPromotionFail = null;
        Grow.eventPromotionFail += () => PlayClip("disallow", Vector3.zero, true);
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
        Demolition.eventPopUp = null;
        Demolition.eventPopUp += () => PlayClip("pop_up", Vector3.zero, true);
        Demolition.eventPopDown = null;
        Demolition.eventPopDown += () => PlayClip("button", Vector3.zero, true);
        Farming.eventDig = null;
        Farming.eventDig += (isPlay) => SetSourcePlay("dig", isPlay);
        ITower.eventDestroy = null;
        ITower.eventDestroy += (pos) => PlayClip("demolish", pos, true);
    }

    /* Initialize */
    /// <summary>
    /// audioSources 를 평탄화하여 sources 에 연결합니다.
    /// </summary>
    private void Init()
    {
        sources.Clear();
        loopSources.Clear();

        foreach (var item in audioSourcesGroup)
        {
            foreach (var source in item.GetComponents<AudioSource>())
            {
                sources.Add(source);
            }
        }

        foreach (var item in loopAudioSourcesGroup)
        {
            foreach (var source in item.GetComponents<LoopAudioSource>())
            {
                loopSources.Add(source);
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
        LoadVolume();
    }

    /* Public Mehtod */
    public void LoadVolume()
    {
        float bgmValue = PlayerPrefs.GetFloat("BGM", 0.5f);
        float sfxValue = PlayerPrefs.GetFloat("SFX", 0.5f);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        SetVolume();
    }

    public void SetVolume()
    {
        float bgmValue = Mathf.Log10(Mathf.Clamp(bgmSlider.value, 0.0001f, 1f)) * 20;
        float sfxValue = Mathf.Log10(Mathf.Clamp(sfxSlider.value, 0.0001f, 1f)) * 20;

        mixer.SetFloat("BGM", bgmValue);
        mixer.SetFloat("SFX", sfxValue);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("BGM", bgmSlider.value);
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
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
                // 거리에 따른 볼륨 조절 ON
                if (!global)
                {
                    GameObject go = new GameObject("OneShot3DAudio");
                    go.transform.position = pos;
                    AudioSource newSource = go.AddComponent<AudioSource>();

                    newSource.clip = source.clip;
                    newSource.spatialBlend = 1f; // 3D
                    newSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
                    newSource.volume = source.volume;
                    newSource.pitch = source.pitch;
                    newSource.Play();

                    Destroy(go, newSource.clip.length);
                }
                // 거리에 따른 볼륨 조절 OFF
                else
                {
                    GameObject go = new GameObject("OneShot2DAudio");
                    go.transform.position = pos;
                    AudioSource newSource = go.AddComponent<AudioSource>();

                    newSource.clip = source.clip;
                    newSource.spatialBlend = 0f; // 2D
                    newSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
                    newSource.volume = source.volume;
                    newSource.pitch = source.pitch;
                    newSource.Play();

                    Destroy(go, newSource.clip.length);
                }

                break;
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
    /// <summary>
    /// 루프 오디오 소스의 시동을 걸거나 종료합니다.
    /// </summary>
    private void SetLoopSource(string clipName, bool isActive)
    {
        foreach (LoopAudioSource loopSource in loopSources)
        {
            if (loopSource.audioSource.clip != null && loopSource.audioSource.clip.name == clipName)
            {
                switch (isActive)
                {
                    case true:
                        loopSource.audioSource.time = 0;
                        loopSource.isOn = true;
                        loopSource.audioSource.Play();
                        break;
                    case false:
                        loopSource.isOn = false;
                        break;
                }
            }
        }
    }
}