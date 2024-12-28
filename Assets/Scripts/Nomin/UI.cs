using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI Abocado;
    public TextMeshProUGUI Garu;
    public GameObject HP;

    private void Start()
    {
        StartCoroutine(CorUpdate());
    }

    public IEnumerator CorUpdate()
    {
        while (true)
        {
            Abocado.text = StaticData.Abocado.ToString();
            Garu.text = StaticData.Garu.ToString();

            yield return new WaitForSeconds(0.1f);
        }
    }
}
