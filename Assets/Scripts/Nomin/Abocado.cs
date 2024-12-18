using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abocado : MonoBehaviour
{
    // 0 = ����, 1 = ����, 2 = ����
    private int level = 0;
    private SpriteRenderer spr = new SpriteRenderer();

    public void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public void LevelUp()
    {
        if (level != 2)
        {
            level++;
            spr.sprite = Resources.Load<Sprite>($"Images/Level{level}");
        }
    }

    public void Harvest()
    {
        level--;
        spr.sprite = Resources.Load<Sprite>($"Images/Level{level}");
        // ���¿� �ƺ�ī��++
    }
}