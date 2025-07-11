using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float MaxHealth = 50f;
    public float CurrentHealth;
    public float DamageMultiplier = 0.5f; // 속도·질량에 따른 피해량 조절 계수

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 부딪혔을 때
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.attachedRigidbody;

            if (playerRb != null)
            {
                // 상대 속도 및 질량으로 데미지 계산
                float relativeVelocity = collision.relativeVelocity.magnitude;
                float mass = playerRb.mass;

                float damage = relativeVelocity * mass * DamageMultiplier;

                ApplyDamage(damage);

                // 버스에게도 피해를 줌 (옵션)
                BusHealth busHealth = collision.collider.GetComponent<BusHealth>();
                if (busHealth != null)
                {
                    busHealth.TakeDamage(damage);
                }

                Debug.Log($"장애물 피해: {damage}");
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("장애물 파괴됨");
        Destroy(gameObject); // 장애물은 풀링 대상이 아니므로 Destroy 가능
    }
}
