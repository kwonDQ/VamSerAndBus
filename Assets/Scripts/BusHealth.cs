using UnityEngine;

public class BusHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        Debug.Log("���� ����! ���� ü��: " + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("���� �ı���! ���� ���� ó�� ����.");
        // TODO: ���� ���� ó��, UI, �� ��ȯ ��
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
}
