using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public List<GameObject> audioSources;                           // 각 엔터티의 오디오 소스 집합 게임 오브젝트
    private List<AudioSource> sources = new List<AudioSource>();    // 모든 오디오 소스를 평탄화 한 리스트

    /// <summary>
    /// 각 유닛의 이벤트에 사운드 재생을 바운드합니다.
    /// </summary>
    private void Bound()
    {
        // Player
        ShotGun.eventAttack = null;
        ShotGun.eventAttack += () => PlayClip("shotgun");
        Knife.eventAttack = null;
        Knife.eventAttack += () => PlayClip("knife");
        Bat.eventAttack = null;
        Bat.eventAttack += () => PlayClip("bat");
        Gun.eventAttack = null;
        Gun.eventAttack += () => PlayClip("gun");
        Sniper.eventAttack = null;
        Sniper.eventAttack += () => PlayClip("sniper");
        Spear.eventAttack = null;
        Spear.eventAttack += () => PlayClip("spear");
        Receiver.eventMove = null;
        Receiver.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);
        Farming.eventMove = null;
        Farming.eventMove += (isPlay) => SetSourcePlay("walk", isPlay);

        // UI
        Date.eventMorning = null;
        Date.eventMorning += () => PlayClip("morning_change", true);
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
        Debug.Log("실행!");
        Init();
        Bound();
    }

    /* Private Method */
    /// <summary>
    /// 오디오 클립을 특정 위치에서 재생합니다.
    /// </summary>
    private void PlayClip(string clipName, bool global = false)
    {
        foreach (AudioSource source in sources)
        {
            if (source.clip != null && source.clip.name == clipName)
            {
                if (!global)
                {
                    AudioSource.PlayClipAtPoint(source.clip, source.gameObject.transform.position);
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
