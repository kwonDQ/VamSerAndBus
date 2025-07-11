using UnityEngine;

public class EnemyAIShooter : MonoBehaviour
{
    public float ArrivalDistance = 5f; // ���� ���� �Ÿ�
    public float RetreatDistance = 7f; // ������ ���� �Ÿ�
    public float StopDistance = 4f; // �ּ� ���� �Ÿ�
    public float MoveSpeed = 20f; // ���� �ӵ�
    public float MaxSpeed = 30f;
    //public float FollowSpeed = 5f;
    public float AttackCooldown = 1.5f; // ���� �ӵ�
    public float ResponseLag = 0.2f; // �����ӵ� ����

    private Transform TargetTransform; // ����
    private float AttackTimer; // ���� �ӵ� ��� Ÿ�̸�
    private Vector2 DelayedTargetPosition;

    private enum AIState { Chasing, Attacking }
    private AIState CurrentState = AIState.Chasing;

    private Rigidbody2D Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform target)
    {
        TargetTransform = target;
    }

    private void Update()
    {
        if (TargetTransform == null) return;

        float distance = Vector2.Distance(transform.position, TargetTransform.position);

        // �� �����Ӹ��� ���� ������ Ÿ�� ��ġ ���
        DelayedTargetPosition = Vector2.Lerp(DelayedTargetPosition, TargetTransform.position, Time.deltaTime / ResponseLag);

        switch (CurrentState)
        {
            case AIState.Chasing:
                if (distance <= ArrivalDistance)
                {
                    CurrentState = AIState.Attacking;
                    AttackTimer = 0f;
                }
                else
                {
                    MoveTowardTarget(MoveSpeed);
                }
                break;

            case AIState.Attacking:
                if (distance > RetreatDistance)
                {
                    CurrentState = AIState.Chasing;
                }
                else
                {
                    Attack();
                    MoveTowardTarget(MoveSpeed * (distance - StopDistance) / (distance + 1)); // �и� 0 �ȵǰ�
                }
                break;
        }
    }

    void MoveTowardTarget(float Speed)
    {
        Vector2 direction = (DelayedTargetPosition - (Vector2)transform.position).normalized;
        Vector2 velocity = direction * Speed;

        // �ִ� �ӵ� ����
        if (velocity.magnitude > MaxSpeed)
            velocity = velocity.normalized * MaxSpeed;

        Rb.velocity = velocity;
    }

    void Attack()
    {
        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackCooldown)
        {
            Debug.Log("���!");
            AttackTimer = 0f;
        }
    }
}
