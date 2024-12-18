using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : Movement
{
    public float Drag = 5f; // ���� ȿ���� ���� �巡�� ��
    public Animator BodyAnimator;
    public Animator ClothesAnimator;
    public Animator HeadAnimator;

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
        if (_movement != Vector2.zero)
        {
            BodyAnimator.SetTrigger("Run");
            ClothesAnimator.SetTrigger("Run");
            HeadAnimator.SetTrigger("Run");
        }
        if (_movement == Vector2.zero)
        {
            BodyAnimator.SetTrigger("Default");
            ClothesAnimator.SetTrigger("Default");
            HeadAnimator.SetTrigger("Default");
        }

        MoveDirection = _movement;
    }
}