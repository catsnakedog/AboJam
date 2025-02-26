using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    /* Dependency */
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite yesSprite;
    [SerializeField] Sprite noSprite;
    Grid grid => Grid.instance;

    /* Field & Property */
    public static Highlight instance;
    Tile lastTile;

    /* Initializer & Finalizer & Updater */
    void Start()
    {
        instance = this;
        transform.localScale = Vector3.one;
        Off();
    }

    /* Public void */
    /// <summary>
    /// 특정 타일을 하이라이팅 합니다.
    /// </summary>
    public void On(Tile tile, bool isReverse = false)
    {
        if (lastTile == tile) return;

        // 하이라이트 시각화
        gameObject.SetActive(true);
        gameObject.transform.position = tile.pos;

        // 이미지 선택 & 크기 조정
        AnimationClick animationClick = null;
        try { animationClick = tile.Go.GetComponent<AnimationClick>(); } catch { }
        if (animationClick != null) { animationClick.OnClick(); spriteRenderer.sprite = null; }
        else if (tile.Go != null) spriteRenderer.sprite = noSprite;
        else if (tile.Go == null)
        {
            if(isReverse) spriteRenderer.sprite = noSprite;
            if(!isReverse) spriteRenderer.sprite = yesSprite;
        }
        if (spriteRenderer.sprite != null) SetSize(spriteRenderer, new Vector2(grid.CellWidth, grid.CellHeight));

        lastTile = tile;
    }
    /// <summary>
    /// 하이라이트를 종료합니다.
    /// </summary>
    public void Off()
    {
        gameObject.SetActive(false);
    }

    /* Private Method */
    /// <summary>
    /// 스프라이트 크기를 지정한 크기로 조정합니다.
    /// </summary>
    void SetSize(SpriteRenderer spriteRenderer, Vector2 size)
    {
        transform.localScale = new Vector3(size.x / spriteRenderer.sprite.bounds.size.x, size.y / spriteRenderer.sprite.bounds.size.y, 1);
    }
}