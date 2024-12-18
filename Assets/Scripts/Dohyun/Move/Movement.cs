using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public float MoveSpeed = 5f; // �̵� �ӵ�
    public Vector2 MoveDirection = Vector2.zero; // �̵� ����
    public Action MoveAction; // �������ʹ� �ش� ���ֿ��� ����

    protected Rigidbody2D _rb; // �ش� ���� Rigidbody2d


    public virtual void InitFirst()
    {

    }

    public virtual void InitMovement(Rigidbody2D rb)
    {
        InitFirst();
        _rb = rb;
        MoveAction += Move;
        InitAdditional();
    }

    public virtual void InitAdditional()
    {

    }

    public void Move()
    {
        _rb?.MovePosition(_rb.position + MoveDirection * MoveSpeed * Time.fixedDeltaTime);
    }
}