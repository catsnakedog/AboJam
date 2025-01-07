using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGageBar : MonoBehaviour
{
    public Vector3 GagePos = new(0, 0.9f, 0);

    private GameObject _player;
    private MaterialPropertyBlock _property;
    private SpriteRenderer _spriteRenderer;

    public void Init()
    {
        _player = GameObject.FindWithTag("Player");
        _property = new();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        transform.position = _player.transform.position + GagePos;
    }

    public void SetGageValue(float gage)
    {
        _property.SetFloat("_Fill", gage);
        _spriteRenderer.SetPropertyBlock(_property);
    }
}
