using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

internal class TradeCountPreset : MonoBehaviour
{
    public TMP_InputField inputField;

    public void Set()
    {
        int.TryParse(inputField.text, out int result);
        Change.instance.TradeCount = result;
        Change.instance.OnValueChange();
    }
}
