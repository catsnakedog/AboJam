using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using static Spear;

public class SpearObj : MonoBehaviour
{
    private Type _spearObjType = typeof(SpearObj);
    private MaterialPropertyBlock _property;
    private SpriteRenderer _spriteRenderer;

    public void Init(SpearEffectData data, Quaternion rot)
    {
        _property = new();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        transform.rotation = rot;
        StartCoroutine(ShowEffect(data, rot));
    }

    private float GetLerpValue(StartEndData data, float process)
    {
        return Mathf.Lerp(data.Start, data.End, process);
    }

    private IEnumerator ShowEffect(SpearEffectData data, Quaternion rot)
    {
        float time = 0;

        while (time < data.Time)
        {
            time += Time.deltaTime;
            float process = time / data.Time;
            _property.SetFloat("_HeadWidth", GetLerpValue(data.HeadWidth, process));
            _property.SetFloat("_HeadHeight", GetLerpValue(data.HeadHeight, process));
            _property.SetFloat("_ShaftWidth", GetLerpValue(data.ShaftWidth, process));
            _property.SetFloat("_ShaftHeight", GetLerpValue(data.ShaftHeight, process));
            _property.SetFloat("_NoiseX", GetLerpValue(data.NoiseX, process));
            _property.SetFloat("_NoiseY", GetLerpValue(data.NoiseY, process));
            _property.SetFloat("_OffsetY", GetLerpValue(data.OffsetY, process));
            _spriteRenderer.SetPropertyBlock(_property);
            yield return null;
        }

        _property.SetFloat("_HeadWidth", data.HeadWidth.End);
        _property.SetFloat("_HeadHeight", data.HeadHeight.End);
        _property.SetFloat("_ShaftWidth", data.ShaftWidth.End);
        _property.SetFloat("_ShaftHeight", data.ShaftHeight.End);
        _property.SetFloat("_NoiseX", data.NoiseX.End);
        _property.SetFloat("_NoiseY", data.NoiseY.End);
        _property.SetFloat("_OffsetY", data.OffsetY.End);
        _spriteRenderer.SetPropertyBlock(_property);

        ObjectPool.Instance.Return(_spearObjType, gameObject);
    }
}
