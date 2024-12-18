using UnityEngine;

public class MonsterChaseWithScale : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform
    public float speed = 1f; // ������ �̵� �ӵ�
    public float stoppingDistance = 0.5f; // �÷��̾���� �ּ� �Ÿ�

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned!");
            return;
        }

        // �÷��̾�� ������ �Ÿ� ���
        float distance = Vector2.Distance(transform.position, player.position);

        // �÷��̾���� �ּ� �Ÿ� �̻��� �� ����
        if (distance > stoppingDistance)
        {
            // ���Ͱ� �÷��̾ ���� �̵�
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }

        // ��������Ʈ�� �÷��̾ ���� ������
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1); // �⺻ ����
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // ������
        }
    }
}
