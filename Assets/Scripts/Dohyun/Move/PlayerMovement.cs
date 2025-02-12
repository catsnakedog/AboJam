using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : Movement, IObserver
{
    public Animator BodyAnimator;
    public Animator ClothesAnimator;
    public Animator HeadAnimator;
    public Vector2 _movement;

    public void OnNotify(string state)
    {

    }

    public override void InitFirst()
    {
        base.InitFirst();
        // MoveAction = ProcessInput;
        MoveAction += SetCharacterDirection;
    }

    public void ProcessInput()
    {
        // 입력 처리
        _movement.x = Input.GetAxisRaw("Horizontal"); // 왼쪽/오른쪽 입력 (A, D 또는 ←, →)
        _movement.y = Input.GetAxisRaw("Vertical");   // 위/아래 입력 (W, S 또는 ↑, ↓)

        // 입력 벡터를 정규화하여 대각선 이동 속도 일관성 유지
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