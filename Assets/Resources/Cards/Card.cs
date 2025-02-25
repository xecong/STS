using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card System/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public string cardDescription; // 카드 설명 추가
    public Sprite cardImage;
    public CardType cardType;
    public int damage;
    public int block;
    public int bleed;

    public ICardEffect cardEffect;

    public void Use()
    {
        Debug.Log($"{cardName} used! {cardDescription}");
        cardEffect?.ApplyEffect(); // target 제거
    }
}

// 카드 유형 Enum 복원
public enum CardType
{
    Attack,
    Defense,
    Buff,
    Special
}
