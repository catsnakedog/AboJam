using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : Movement
{
    public float Drag = 5f; // ���� ȿ���� ���� �巡�� ��

    private Vector2 _movement;

    public override void InitFirst()
    {
        base.InitFirst();
        MoveAction = ProcessInput;
        MoveAction += SetCharacterDirection;
    }

    public void ProcessInput()
    {
        // �Է� ó��
        _movement.x = Input.GetAxisRaw("Horizontal"); // ����/������ �Է� (A, D �Ǵ� ��, ��)
        _movement.y = Input.GetAxisRaw("Vertical");   // ��/�Ʒ� �Է� (W, S �Ǵ� ��, ��)

        // �Է� ���͸� ����ȭ�Ͽ� �밢�� �̵� �ӵ� �ϰ��� ����
        _movement = _movement.normalized;
    }

    public void SetCharacterDirection()
    {
        // ���� ������ ����� ���� �ӵ��� ��ǥ �ӵ��� ������ ���� (���� ȿ��)
        MoveDirection = _movement;
    }
}