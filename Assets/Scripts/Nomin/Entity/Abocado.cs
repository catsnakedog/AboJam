using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static EnumData;

public class Abocado : MonoBehaviour
{
    /* Dependency */
    public SpriteRenderer spriteRenderer; // 하이라키 연결

    /* Field & Property */
    public static List<Abocado> instances = new List<Abocado>(); // 모든 아보카도 인스턴스
    public EnumData.Abocado level { get; private set; } // 아보카도 레벨
    public int quality { get; private set; } = 0; // 아보카도 품질 (Promotion)
    public static int quality_max = 1; // 아보카도 최고 품질 (Promotion)
    public int harvest = 1; // 수확량
    private string path = "Images/Abocado/"; // 아보카도 이미지 Resources 경로
    private Sprite[] spr_level; // 레벨에 대응하는 스프라이트

    /* Intializer & Finalizer & Updater */
    private void Start()
    {
        int length_level = Enum.GetValues(typeof(EnumData.Abocado)).Length;
        spr_level = new Sprite[length_level];

        // 레벨에 맞는 이미지를 spr_level 에 바인딩합니다.
        for (int i = 0; i < length_level; i++)
        {
            Sprite temp = Resources.Load<Sprite>(path + (EnumData.Abocado)i);
            if (temp != null) spr_level[i] = temp;
            else spr_level[i] = Resources.Load<Sprite>(path + "Default");
        }

        spriteRenderer.sprite = spr_level[0];
        instances.Add(this);

        // 아침이 밝을 때 성장시킵니다.
        Date.instance.morningStart.AddListener(() => { this.GrowUp(); });
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
            StaticData.Abocado += harvest;
            LevelDown();
        }
    }
    /// <summary>
    /// <br>아보카도의 품질을 향상시킵니다.</br>
    /// <br>수확량이 증가하며, 이미지가 바뀝니다.</br>
    /// </summary>
    public void Promote()
    {
        if (quality == quality_max)
        {
            Debug.Log("이미 최고 품질입니다.");
            return;
        }

        quality++;
        harvest++;

        spr_level[(int)EnumData.Abocado.Tree] = Resources.Load<Sprite>($"{path}{EnumData.Abocado.Tree}{string.Concat(Enumerable.Repeat("+", quality))}");
        spr_level[(int)EnumData.Abocado.Fruited] = Resources.Load<Sprite>($"{path}{EnumData.Abocado.Fruited}{string.Concat(Enumerable.Repeat("+", quality))}");
        if (spr_level[(int)EnumData.Abocado.Tree] == null || spr_level[(int)EnumData.Abocado.Fruited] == null) Debug.Log("아보카도 품질에 맞는 이미지가 없습니다.");

        spriteRenderer.sprite = spr_level[(int)level];
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