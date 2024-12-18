using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // ĳ���� �̵� �ӵ�
    public float drag = 5f; // ���� ȿ���� ���� �巡�� ��

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 velocity; // ���� �ӵ�

    void Start()
    {
        // Rigidbody2D ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �Է� ó��
        movement.x = Input.GetAxisRaw("Horizontal"); // ����/������ �Է� (A, D �Ǵ� ��, ��)
        movement.y = Input.GetAxisRaw("Vertical");   // ��/�Ʒ� �Է� (W, S �Ǵ� ��, ��)

        // �Է� ���͸� ����ȭ�Ͽ� �밢�� �̵� �ӵ� �ϰ��� ����
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // ��ǥ �ӵ� ���
        Vector2 targetVelocity = movement * moveSpeed;

        // ���� ������ ����� ���� �ӵ��� ��ǥ �ӵ��� ������ ���� (���� ȿ��)
        velocity = Vector2.Lerp(velocity, targetVelocity, drag * Time.fixedDeltaTime);

        // Rigidbody2D�� ����� ĳ���� �̵� ó��
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}