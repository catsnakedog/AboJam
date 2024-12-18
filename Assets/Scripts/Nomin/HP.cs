using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public float HP_max = 100f;
    private float HP_current;
    private GameObject parent;
    private Slider slider;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        slider = transform.Find("Slider").GetComponent<Slider>();
        HP_current = HP_max;
    }

    private void Start()
    {
        StartCoroutine(CorHP());
    }

    private IEnumerator CorHP()
    {
        // HP Slider ������Ʈ
        if (HP_current == HP_max)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.active == false) slider.gameObject.SetActive(true);
            if (slider.value > HP_current / HP_max) slider.value -= (HP_max * 0.07f);
        }

        if (HP_current <= 0f)
        {
            Death();
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void Death()
    {
        Destroy(parent);
    }
}
