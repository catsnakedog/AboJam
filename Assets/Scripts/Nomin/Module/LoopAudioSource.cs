using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.AccessControl;
using System.Diagnostics.Eventing.Reader;

[System.Serializable]
public class LoopAudioSource : MonoBehaviour
{
    public AudioSource audioSource;
    public float loopStartTime;
    public float endStartTime;
    public bool? isOn;

    private void Update()
    {
        switch (isOn)
        {
            case true:
                if (audioSource.time >= endStartTime)
                    audioSource.time = loopStartTime;
                break;
            case false:
                if (audioSource.time >= loopStartTime)
                {
                    audioSource.time = endStartTime;
                    isOn = null;
                }
                break;
            case null:
                break;
        }
    }
}