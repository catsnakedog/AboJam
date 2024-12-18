using UnityEngine;

public class MonsterChaseWithScale : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 1f; // 몬스터의 이동 속도
    public float stoppingDistance = 0.5f; // 플레이어와의 최소 거리

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            return;
        }

        // 플레이어와 몬스터의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        // 플레이어와의 최소 거리 이상일 때 추적
        if (distance > stoppingDistance)
        {
            // 몬스터가 플레이어를 향해 이동
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }

        // 스프라이트를 플레이어를 향해 뒤집기
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // 기본 방향
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // 뒤집기
        }
    }
}
