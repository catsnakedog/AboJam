using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public float MoveSpeed = 2f; // �̵� �ӵ�
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
        if(_rb != null)
        {
            if(MoveDirection != Vector2.zero)
            {
                if (MoveDirection.x > 0)
                    _rb.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                else
                    _rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            _rb.MovePosition(_rb.position + MoveSpeed * Time.fixedDeltaTime * MoveDirection);
        }
    }
}