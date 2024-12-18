using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abocado : MonoBehaviour
{
    // 0 = ¾¾¾Ñ, 1 = ³ª¹«, 2 = ¿­¸Å
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
        // °èÁÂ¿¡ ¾Æº¸Ä«µµ++
    }
}