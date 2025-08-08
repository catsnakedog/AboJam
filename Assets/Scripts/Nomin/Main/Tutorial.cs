using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> stars;
    [SerializeField] private Sprite starFull;

    private int scenarioLevel { get => LocalData.instance.LoadScenario(); }

    private void Start()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (i >= scenarioLevel) break;

            stars[i].GetComponent<Image>().sprite = starFull;
        }
    }
}
