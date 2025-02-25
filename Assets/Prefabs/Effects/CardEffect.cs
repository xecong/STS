using UnityEngine;

// 카드 효과 구현을 위한 베이스 클래스
public abstract class CardEffect : ScriptableObject, ICardEffect
{
    // 다음 카드 정보 참조용 변수
    protected Card nextCard;
    // 적 상태 참조
    protected EnemyStatus enemyStatus;
    // 플레이어 상태 참조
    protected PlayerStatus playerStatus;

    // 효과 적용 추상 메서드
    public abstract void ApplyEffect();

    // 다음 카드 설정 메서드
    public void SetNextCard(Card card)
    {
        nextCard = card;
    }

    // 효과 발동 전 초기화
    public virtual void Initialize(EnemyStatus enemy, PlayerStatus player)
    {
        enemyStatus = enemy;
        playerStatus = player;
    }
}