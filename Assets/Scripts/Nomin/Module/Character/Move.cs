using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 임시 움직임 (이 스크립트는 지워도 됩니다)
/// </summary>
public class Move : MonoBehaviour
{
    public Gunner gunner;
    public float moveSpeed = 5f; // 이동 속도 (m/s)
    public bool isMove = true;

    void Update()
    {
        /* 타겟 지정 */
        Vector3 destination = gunner.launcher.targeter.Targetting(Targeter.TargetType.Near, new string[] { "Player", "Towers" }, 999).transform.position;

        /* 타겟 - 내 위치 */
        Vector3 vector = destination - transform.position;

        /* 거리 */
        float distance = vector.magnitude;

        /* 방향 벡터*/
        Vector3 direction = (destination - transform.position).normalized;

        /* 임시 이동 & 애니메이션 */
        if (distance > gunner.detection * 0.9f && isMove == true)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 애니메이션
            AnimatorStateInfo animatorStateInfo = gunner.animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.normalizedTime > 0.99) gunner.animator.Play("Move");
        }
    }
}