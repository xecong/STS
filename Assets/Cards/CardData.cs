using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card")]
public class CardData : ScriptableObject
{
    public string cardName; // 카드 이름
    public Sprite cardImage; // 카드 이미지 (선택 사항)
    public string CardDescription; // 카드 설명

    public void ApplyEffect()
    {
        Debug.Log($"🃏 {cardName} 효과 적용!");
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        //gameManager.DealDamageToEnemy(damage);
    }

}
