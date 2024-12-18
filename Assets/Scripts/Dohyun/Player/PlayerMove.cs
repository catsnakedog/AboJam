using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 캐릭터 이동 속도
    public float drag = 5f; // 관성 효과를 위한 드래그 값

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 velocity; // 현재 속도

    void Start()
    {
        // Rigidbody2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal"); // 왼쪽/오른쪽 입력 (A, D 또는 ←, →)
        movement.y = Input.GetAxisRaw("Vertical");   // 위/아래 입력 (W, S 또는 ↑, ↓)

        // 입력 벡터를 정규화하여 대각선 이동 속도 일관성 유지
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // 목표 속도 계산
        Vector2 targetVelocity = movement * moveSpeed;

        // 선형 보간을 사용해 현재 속도를 목표 속도로 서서히 변경 (관성 효과)
        velocity = Vector2.Lerp(velocity, targetVelocity, drag * Time.fixedDeltaTime);

        // Rigidbody2D를 사용해 캐릭터 이동 처리
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}