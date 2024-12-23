using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Abocado : MonoBehaviour
{
    // 0 = 개간, 1 = 씨앗, 2 = 나무, 3 = 열매
    public int level = 0;
    private SpriteRenderer spr = new SpriteRenderer();
    public Slider slider;

    private static List<Abocado> abocados = new List<Abocado>();

    public void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        slider = gameObject.GetComponent<Slider>();
        this.spr.sprite = Resources.Load<Sprite>("Images/Level0");
        abocados.Add(this);
    }

    /// <summary>
    /// 아보카도를 레벨업 시킵니다. forced = true 면 조건에 관계없이 성장합니다.
    /// </summary>
    public void LevelUp(bool forced = false)
    {
        // 개간 상태, 열매 상태는 레벨업 X
        if ((level == 0 || level == 3) && forced == false) return;

        level++;
        this.spr.sprite = Resources.Load<Sprite>($"Images/Level{level}");
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
            this.spr.sprite = Resources.Load<Sprite>($"Images/Level{level}");
            StaticData.Abocado++;
        }
    }
}