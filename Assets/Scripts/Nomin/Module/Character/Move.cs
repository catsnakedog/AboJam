using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 임시 움직임 (이 스크립트는 지워도 됩니다)
/// </summary>
public class Move : MonoBehaviour
{
    public Targeter targeter;
    public Gunner gunner;
    public Leopard leopard;
    public Thug thug;
    public float moveSpeed = 5f; // 이동 속도 (m/s)
    public bool isMove = true;

    float detection;
    Animator animator;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (gunner != null)
        {
            detection = gunner.detection;
            animator = gunner.animator;
        }
        if (leopard != null)
        {
            detection = leopard.detection;
            animator = leopard.animator;
        }
        if (thug != null)
        {
            detection = thug.detection;
            animator = thug.animator;
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetMove(bool OnOff)
    {
        isMove = OnOff;
    }

    void Update()
    {
        /* 타겟 지정 */
        GameObject target = targeter.Targetting(Targeter.TargetType.Near, new string[] { "Player", "Towers", "Abocados" }, 999);
        Vector3 destination;
        if (target != null) destination = target.transform.position;
        else return;

        /* 타겟 - 내 위치 */
        Vector3 vector = destination - transform.position;

        /* 거리 */
        float distance = vector.magnitude;

        /* 방향 벡터*/
        Vector3 direction = (destination - transform.position).normalized;

        /* 스프라이트 방향 변경 (좌우 반전) */
        if (spriteRenderer != null) spriteRenderer.flipX = direction.x > 0;

        /* 임시 이동 & 애니메이션 */
        if (distance > detection * 0.7f && isMove == true)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 애니메이션
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.normalizedTime > 0.99) animator.Play("Move");
        }
    }
}