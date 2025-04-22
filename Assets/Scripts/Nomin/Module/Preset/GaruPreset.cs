using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class GaruPreset : MonoBehaviour
{
    public TMP_InputField inputField;

    public void Set()
    {
        int.TryParse(inputField.text, out int result);
        StaticData.Garu = result;
    }
}
