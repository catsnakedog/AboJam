using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : Movement
{
    public float Drag = 5f; // ���� ȿ���� ���� �巡�� ��

    private Vector2 _movement;
    private Vector2 _velocity; // ���� �ӵ�

    public override void InitFirst()
    {
        base.InitFirst();
        MoveAction += SetCharacterDirection;
    }


    public void SetCharacterDirection()
    {
        // ���� ������ ����� ���� �ӵ��� ��ǥ �ӵ��� ������ ���� (���� ȿ��)
        MoveDirection = Vector2.Lerp(_velocity, _movement * MoveSpeed, Drag * Time.fixedDeltaTime);
    }
}