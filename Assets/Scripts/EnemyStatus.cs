using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    private int currentBleedAmount = 0; // 현재 출혈 데미지
    
    // 현재 출혈량 속성
    public int CurrentBleedAmount => currentBleedAmount;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    // 데미지 처리
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // 체력이 0 이하로 내려가지 않도록
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"적이 {amount} 데미지를 받았어! 남은 체력: {currentHealth}/{maxHealth}");
    }
    
    // 출혈 효과 추가
    public void AddBleedEffect(int amount)
    {
        currentBleedAmount += amount;
        Debug.Log($"적에게 출혈 {amount} 추가! 현재 총 출혈: {currentBleedAmount}");
    }
    
    // 출혈 효과 지우기
    public void ClearBleedEffect()
    {
        currentBleedAmount = 0;
        Debug.Log("모든 출혈 효과가 제거되었어!");
    }
    
    // 턴 종료 시 출혈 데미지 적용
    public void ApplyBleedDamage()
    {
        if (currentBleedAmount > 0)
        {
            TakeDamage(currentBleedAmount);
            Debug.Log($"턴 종료: 출혈에 의해 {currentBleedAmount} 데미지를 입었어!");
        }
    }
    
    // 사망 처리
    private void Die()
    {
        Debug.Log("적이 쓰러졌어!");
        // 여기에 적 사망 시 추가 로직 (경험치, 보상 등)
    }
}
