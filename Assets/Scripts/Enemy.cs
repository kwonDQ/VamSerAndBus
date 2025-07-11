using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MaxHealth = 50f;
    public float CurrentHealth;
    public float DamageMultiplier = 0.5f;
    public float KnockbackForce = 5f;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D busRb = collision.collider.attachedRigidbody;
            if (busRb != null)
            {
                float relativeVelocity = collision.relativeVelocity.magnitude;
                float mass = busRb.mass;
                float damage = relativeVelocity * mass * DamageMultiplier;

                // 넉백 적용
                Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
                Rigidbody2D enemyRb = GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(knockbackDir * damage * KnockbackForce, ForceMode2D.Impulse);
                }

                ApplyDamage(damage); // 적 데미지

                BusHealth busHealth = collision.collider.GetComponent<BusHealth>(); // 버스 데미지
                if (busHealth != null)
                {
                    busHealth.TakeDamage(damage);
                }
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
        Debug.Log("적 파괴됨");
        EnemyPoolManager.Instance.ReturnEnemy(gameObject, gameObject);
    }
}