using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    // 데미지 처리
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"플레이어가 {amount} 데미지를 받았어! 남은 체력: {currentHealth}/{maxHealth}");
    }
    
    // 체력 회복
    public void HealPlayer(int amount)
    {
        int oldHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // 최대 체력을 초과하지 않도록
        
        int actualHealAmount = currentHealth - oldHealth;
        Debug.Log($"플레이어가 {actualHealAmount} 체력을 회복했어! 현재 체력: {currentHealth}/{maxHealth}");
    }
    
    // 사망 처리
    private void Die()
    {
        Debug.Log("플레이어가 쓰러졌어... 게임 오버...");
        // 게임 오버 로직
    }
}
