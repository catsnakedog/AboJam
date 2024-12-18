using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    public float Drag = 5f; // 관성 효과를 위한 드래그 값

    private Vector2 _movement;
    private Vector2 _velocity; // 현재 속도

    public override void InitFirst()
    {
        base.InitFirst();
        MoveAction = ProcessInput;
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
        // 선형 보간을 사용해 현재 속도를 목표 속도로 서서히 변경 (관성 효과)
        MoveDirection = Vector2.Lerp(_velocity, _movement * MoveSpeed, Drag * Time.fixedDeltaTime);
    }
}