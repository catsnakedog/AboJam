using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

internal class DayPreset : MonoBehaviour
{
    public TMP_InputField inputField;

    public void Set()
    {
        int.TryParse(inputField.text, out int result);
        Date.instance.Skip(result - 1);
        Spawner.instance.StopWave();
        Spawner.instance.waveIndex = result;
    }
}
