using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [HideInInspector]
    public Movement Movement; // �̵� ����

    public virtual void Init()
    {
        // �̵� �ʱ�ȭ
        Movement?.InitMovement(gameObject.GetComponent<Rigidbody2D>());
    }
}