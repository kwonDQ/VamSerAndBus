using UnityEngine;

public class EnemyAIShooter : MonoBehaviour
{
    public float ArrivalDistance = 5f; // 공격 시작 거리
    public float RetreatDistance = 7f; // 재추적 시작 거리
    public float StopDistance = 4f; // 최소 추적 거리
    public float MoveSpeed = 20f; // 추적 속도
    public float MaxSpeed = 30f;
    //public float FollowSpeed = 5f;
    public float AttackCooldown = 1.5f; // 공격 속도
    public float ResponseLag = 0.2f; // 반응속도 지연

    private Transform TargetTransform; // 버스
    private float AttackTimer; // 공격 속도 재는 타이머
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

        // 매 프레임마다 반응 지연된 타겟 위치 계산
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
                    MoveTowardTarget(MoveSpeed * (distance - StopDistance) / (distance + 1)); // 분모 0 안되게
                }
                break;
        }
    }

    void MoveTowardTarget(float Speed)
    {
        Vector2 direction = (DelayedTargetPosition - (Vector2)transform.position).normalized;
        Vector2 velocity = direction * Speed;

        // 최대 속도 제한
        if (velocity.magnitude > MaxSpeed)
            velocity = velocity.normalized * MaxSpeed;

        Rb.velocity = velocity;
    }

    void Attack()
    {
        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackCooldown)
        {
            Debug.Log("사격!");
            AttackTimer = 0f;
        }
    }
}
