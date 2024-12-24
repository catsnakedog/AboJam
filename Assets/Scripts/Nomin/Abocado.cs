using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Abocado : MonoBehaviour
{
    /* Dependency */
    [Header("Dependency")] public SpriteRenderer spriteRenderer;

    /* Field */
    public static List<Abocado> instances = new List<Abocado>(); // 모든 아보카도 인스턴스
    public EnumData.Abocado level { get; private set; } // 아보카도 레벨
    private Sprite[] spr_level; // 레벨에 대응하는 스프라이트

    /* Intializer & Finalizer */
    private void Start()
    {
        int length_level = Enum.GetValues(typeof(EnumData.Abocado)).Length;
        spr_level = new Sprite[length_level];

        // 레벨에 맞는 이미지를 spr_level 에 바인딩합니다.
        for (int i = 0; i < length_level; i++)
        {
            Sprite temp = Resources.Load<Sprite>("Images/Abocado/" + (EnumData.Abocado)i);
            if(temp != null) spr_level[i] = temp;
            else spr_level[i] = Resources.Load<Sprite>("Images/Abocado/Default");
        }

        spriteRenderer.sprite = spr_level[0];
        instances.Add(this);
    }
    private void OnDestroy()
    {
        instances.Remove(this);
    }

    /* Public Method */
    /// <summary>
    /// <br>아보카도가 성장합니다.</br>
    /// </summary>
    /// <param name="forced">Cultivated >> Seed 성장 On / Off</param>
    public void GrowUp(bool forced = false)
    {
        LevelUp(forced);
    }
    /// <summary>
    /// <br>아보카도를 수확합니다.</br>
    /// </summary>
    public void Harvest()
    {
        if (level == EnumData.Abocado.Fruited)
        {
            StaticData.Abocado++;
            LevelDown();
        }
    }

    /* Private Method */
    private void LevelUp(bool forced = false)
    {
        // 개간 상태, 열매 상태는 레벨 업 X
        if ((level == EnumData.Abocado.Cultivated || level == EnumData.Abocado.Fruited) && forced == false) return;

        level++;
        spriteRenderer.sprite = spr_level[(int)level];
    }
    private void LevelDown()
    {
        // 개간 상태, 씨앗 상태는 레벨 다운 X
        if ((level == EnumData.Abocado.Cultivated || level == EnumData.Abocado.Seed)) return;

        level--;
        spriteRenderer.sprite = spr_level[((int)level)];
    }
}