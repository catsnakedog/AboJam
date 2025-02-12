using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public float MoveSpeed = 2f; // 이동 속도
    public Vector2 MoveDirection = Vector2.zero; // 이동 방향
    public Action MoveAction; // 업데이터는 해당 유닛에서 실행

    protected Rigidbody2D _rb; // 해당 유닛 Rigidbody2d


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