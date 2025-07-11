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
        Debug.Log("버스 피해! 현재 체력: " + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("버스 파괴됨! 게임 오버 처리 예정.");
        // TODO: 게임 오버 처리, UI, 씬 전환 등
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
}
