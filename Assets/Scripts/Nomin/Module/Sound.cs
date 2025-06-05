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
        // Player
        ShotGun.eventAttack = null;
        ShotGun.eventAttack += (pos) => PlayClip("shotgun", pos);
        Knife.eventAttack = null;
        Knife.eventAttack += (pos) => PlayClip("knife", pos);
        Bat.eventAttack = null;
        Bat.eventAttack += (pos) => PlayClip("bat", pos);
        Gun.eventAttack = null;
        Gun.eventAttack += (pos) => PlayClip("gun", pos);
        Sniper.eventAttack = null;
        Sniper.eventAttack += (pos) => PlayClip("sniper", pos);
        Spear.eventAttack = null;
        Spear.eventAttack += (pos) => PlayClip("spear", pos);
        Receiver.eventMove = null;
        Receiver.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Farming.eventMove = null;
        Farming.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);

        // Building
        Auto.eventFire = null;
        Auto.eventFire += (pos) => PlayClip("auto_fire", pos);
        Heal.eventFire = null;
        Heal.eventFire += (pos) => PlayClip("heal_fire", pos);

        // UI
        Date.eventMorning = null;
        Date.eventMorning += () => PlayClip("morning_change", Vector3.zero, true);
        OnOff.eventClick = null;
        OnOff.eventClick += () => PlayClip("button", Vector3.zero, true);
        Menu.eventClick = null;
        Menu.eventClick += () => PlayClip("button", Vector3.zero, true);
        Farming.eventFarming = null;
        Farming.eventFarming += (pos) => PlayClip("abocado_seed", pos);
        Abocado.eventFarming = null;
        Abocado.eventFarming += (pos) => PlayClip("abocado_seed", pos);
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
