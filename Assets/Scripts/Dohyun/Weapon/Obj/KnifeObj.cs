using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

public class KnifeObj : MonoBehaviour
{
    private Type _knifeObjType = typeof(KnifeObj);
    private MaterialPropertyBlock _property;
    private SpriteRenderer _spriteRenderer;

    public void Init(float effectTime, float speed, Quaternion rot)
    {
        _property = new();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        transform.rotation = Quaternion.Euler(rot.eulerAngles - new Vector3(0, 0, 90));
        StartCoroutine(ShowEffect(effectTime, speed, rot));
    }

    private IEnumerator ShowEffect(float effectTime, float speed, Quaternion rot)
    {
        float time = 0;
        float angleRad = rot.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0).normalized;

        while (time < effectTime)
        {
            transform.position = transform.position + direction * speed * Time.deltaTime;
            float process = time / effectTime;
            _property.SetFloat("_Opacity", 1 - process);
            _property.SetFloat("_CorrectY", Mathf.Lerp(1.3f, 0.3f, process));
            _spriteRenderer.SetPropertyBlock(_property);
            yield return null;
            time += Time.deltaTime;
        }

        _property.SetFloat("_Opacity", 0);
        _property.SetFloat("_CorrectY", 0.3f);
        _spriteRenderer.SetPropertyBlock(_property);

        ObjectPool.Instance.Return(_knifeObjType, gameObject);
    }
}
