using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float MaxHealth = 50f;
    public float CurrentHealth;
    public float DamageMultiplier = 0.5f; // �ӵ��������� ���� ���ط� ���� ���

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾�� �ε����� ��
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.attachedRigidbody;

            if (playerRb != null)
            {
                // ��� �ӵ� �� �������� ������ ���
                float relativeVelocity = collision.relativeVelocity.magnitude;
                float mass = playerRb.mass;

                float damage = relativeVelocity * mass * DamageMultiplier;

                ApplyDamage(damage);

                // �������Ե� ���ظ� �� (�ɼ�)
                BusHealth busHealth = collision.collider.GetComponent<BusHealth>();
                if (busHealth != null)
                {
                    busHealth.TakeDamage(damage);
                }

                Debug.Log($"��ֹ� ����: {damage}");
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
        Debug.Log("��ֹ� �ı���");
        Destroy(gameObject); // ��ֹ��� Ǯ�� ����� �ƴϹǷ� Destroy ����
    }
}
