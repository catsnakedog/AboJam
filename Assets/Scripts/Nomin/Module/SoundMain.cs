using Synty.Interface.FantasyWarriorHUD.Samples;
using System.Collections.Generic;
using UnityEngine;

public class SoundMain : MonoBehaviour
{
    public static SoundMain instance;
    public List<GameObject> audioSources;                           // 각 엔터티의 오디오 소스 집합 게임 오브젝트
    private List<AudioSource> sources = new List<AudioSource>();    // 모든 오디오 소스를 평탄화 한 리스트

    /// <summary>
    /// 각 유닛의 이벤트에 사운드 재생을 바운드합니다.
    /// </summary>
    private void Bound()
    {
        // UI
        Record.eventClick = null;
        Record.eventClick += () => PlayClip("button", Vector3.zero, true);
        Record.eventHover = null;
        Record.eventHover += () => PlayClip("hover", Vector3.zero, true);
        History.eventNetwork = null;
        History.eventNetwork += () => PlayClip("network", Vector3.zero, true);
        History.eventHover = null;
        History.eventHover += () => PlayClip("hover", Vector3.zero, true);
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
