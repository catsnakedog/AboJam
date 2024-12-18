using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Abocado : MonoBehaviour
{
    // 0 = ����, 1 = ����, 2 = ����, 3 = ����
    public int level = 0;
    public float HP = StaticData.HP_Abocado;
    private SpriteRenderer spr = new SpriteRenderer();
    public Slider slider;
    private static List<Abocado> abocados = new List<Abocado>();

    public void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        ChangeSprite(Resources.Load<Sprite>("Images/Level0"));
        abocados.Add(this);
        StartCoroutine(CorHP());
    }

    /// <summary>
    /// �ƺ�ī���� ������ ��ŵ�ϴ�. forced = true �� ���ǿ� ������� �����մϴ�.
    /// </summary>
    public void LevelUp(bool forced = false)
    {
        // ���� ����, ���� ���´� ������ X
        if ((level == 0 || level == 3) && forced == false) return;

        level++;
        ChangeSprite(Resources.Load<Sprite>($"Images/Level{level}"));
    }

    public static void LevelUpAll()
    {
        foreach (var item in abocados)
        {
            if (item != null)
            {
                item.LevelUp();
            }
        }
    }

    public void Harvest()
    {
        if (level == 3)
        {
            level--;
            ChangeSprite(Resources.Load<Sprite>($"Images/Level{level}"));
            StaticData.Abocado++;
        }
    }

    public void ChangeSprite(Sprite sprite)
    {
        this.spr.sprite = sprite;
    }

    private IEnumerator CorHP()
    {
        if (HP == StaticData.HP_Abocado)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if(slider.gameObject.active == false) slider.gameObject.SetActive(true);
            if (slider.value > HP / StaticData.HP_Abocado) slider.value -= (StaticData.HP_Abocado * 0.07f);
        }
        
        yield return new WaitForSeconds(0.1f);
    }
}