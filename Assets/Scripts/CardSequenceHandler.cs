using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// 카드 시퀀스 처리 및 효과 관리를 위한 클래스
public partial class CardSequenceHandler : MonoBehaviour
{
    [Header("참조")]
    private EnemyStatus enemyStatus;
    private PlayerStatus playerStatus;
    
    [Header("이펙트 프리팹")]
    public GameObject slashEffectPrefab;         // 칼날 효과
    public GameObject enhanceEffectPrefab;       // 강화 효과
    public GameObject explosionEffectPrefab;     // 폭발 효과
    public GameObject healEffectPrefab;          // 회복 효과
    
    // 카드 타입에 따른 애니메이션 대기 시간
    private Dictionary<CardAnimationType, float> animationDurations = new Dictionary<CardAnimationType, float>()
    {
        { CardAnimationType.Quick, 0.8f },
        { CardAnimationType.Standard, 1.3f },
        { CardAnimationType.Enhanced, 2.0f },
        { CardAnimationType.Core, 3.5f }
    };
    
    // 시작 시 초기화
    private void Start()
    {
        InitializeReferences();
        LoadEffectPrefabs();
    }
    
    // 필요한 참조 초기화
    private void InitializeReferences()
    {
        // FindObjectOfType 대신 권장되는 FindFirstObjectByType 사용
        enemyStatus = FindFirstObjectByType<EnemyStatus>();
        playerStatus = FindFirstObjectByType<PlayerStatus>();
        
        if (enemyStatus == null || playerStatus == null)
        {
            Debug.LogError("❌ EnemyStatus 또는 PlayerStatus를 찾을 수 없어!");
        }
    }
    
    // 이펙트 프리팹 로드 (Inspector에서 할당하지 않은 경우)
    private void LoadEffectPrefabs()
    {
        if (slashEffectPrefab == null)
            slashEffectPrefab = Resources.Load<GameObject>("Effects/SlashEffect");
            
        if (enhanceEffectPrefab == null)
            enhanceEffectPrefab = Resources.Load<GameObject>("Effects/EnhanceEffect");
            
        if (explosionEffectPrefab == null)
            explosionEffectPrefab = Resources.Load<GameObject>("Effects/ExplosionEffect");
            
        if (healEffectPrefab == null)
            healEffectPrefab = Resources.Load<GameObject>("Effects/HealEffect");
    }

    // 카드들을 효과순으로 실행하는 메서드
    public void ProcessCardSequence(List<Card> cards)
    {
        Debug.Log($"🃏 카드 시퀀스 처리 시작! 카드 수: {cards.Count}");
        
        for (int i = 0; i < cards.Count; i++)
        {
            Card currentCard = cards[i];
            Card nextCard = (i < cards.Count - 1) ? cards[i + 1] : null;
            Card prevCard = (i > 0) ? cards[i - 1] : null;
            
            // 이전 카드와 현재 카드의 상호작용 체크
            CheckCardInteractions(prevCard, currentCard);
            
            // 현재 카드의 효과를 가져와 초기화
            CardEffect cardEffect = currentCard.cardEffect as CardEffect;
            if (cardEffect != null)
            {
                // 다음 카드 정보와 상태 참조 설정
                cardEffect.SetNextCard(nextCard);
                cardEffect.Initialize(enemyStatus, playerStatus);
                
                // 효과 적용
                Debug.Log($"🎮 {i+1}번째 카드 '{currentCard.cardName}' 효과 발동!");
                currentCard.Use();
            }
            else
            {
                Debug.LogWarning($"⚠️ {currentCard.cardName}의 효과가 CardEffect 타입이 아니거나 null이야...");
            }
        }
        
        // 턴 종료 시 출혈 데미지 적용
        if (enemyStatus != null)
        {
            enemyStatus.ApplyBleedDamage();
        }
        
        Debug.Log("✨ 카드 시퀀스 처리 완료!");
        
        // HandManager의 애니메이션 상태를 false로 설정하여 턴이 끝날 수 있게 함
        if (HandManager.Instance != null)
        {
            HandManager.Instance.SetAnimatingFalse();
        }
    }
    
    // 카드 간 상호작용 체크
    private void CheckCardInteractions(Card prevCard, Card currentCard)
    {
        if (prevCard == null || currentCard == null) return;
        
        // 이전 카드가 숫돌이면 현재 카드의 출혈 효과 증가
        if (prevCard.cardName.Contains("숫돌") && currentCard.bleed > 0)
        {
            int originalBleed = currentCard.bleed;
            currentCard.bleed = Mathf.RoundToInt(currentCard.bleed * 1.5f);
            Debug.Log($"🔪 숫돌 효과: {currentCard.cardName}의 출혈 데미지가 {originalBleed}에서 {currentCard.bleed}로 증가!");
        }
        
        // 연속으로 면도날 카드가 나오면 추가 효과
        if (prevCard.cardName.Contains("면도날") && currentCard.cardName.Contains("면도날"))
        {
            int bonusDamage = 2;
            currentCard.damage += bonusDamage;
            Debug.Log($"⚔️ 연속 면도날 효과: {currentCard.cardName}의 데미지가 {bonusDamage} 증가했어!");
        }
        
        // 이전에 깨물기 카드가 사용되고 현재 카드가 피폭발이면 추가 효과
        if (prevCard.cardName.Contains("깨물기") && currentCard.cardName.Contains("피폭발"))
        {
            Debug.Log($"💥 깨물기+피폭발 콤보: 피폭발 데미지 2배 증가!");
            // 구현은 피폭발 효과 내에서 처리
        }
    }
    
    // 애니메이션 타입 결정
    public CardAnimationType DetermineCardAnimationType(Card card)
    {
        if (card == null) return CardAnimationType.Standard;
        
        // 카드에 직접 애니메이션 타입이 설정되어 있는지 확인
        // (Card 클래스에 animationType 프로퍼티가 있을 경우)
        if (typeof(Card).GetProperty("animationType") != null)
        {
            var property = typeof(Card).GetProperty("animationType");
            if (property != null)
            {
                var value = property.GetValue(card);
                if (value != null && value is CardAnimationType)
                {
                    return (CardAnimationType)value;
                }
            }
        }
        
        // 아니면 카드 이름으로 판단
        string cardName = card.cardName.ToLower();
        
        if (cardName.Contains("면도날"))
        {
            return CardAnimationType.Quick;
        }
        else if (cardName.Contains("숫돌"))
        {
            return CardAnimationType.Enhanced;
        }
        else if (cardName.Contains("피폭발"))
        {
            return CardAnimationType.Core;
        }
        else if (cardName.Contains("깨물기"))
        {
            return CardAnimationType.Standard;
        }
        
        return CardAnimationType.Standard;
    }
    
    // 각 카드 애니메이션 재생 후 적절한 대기 시간 적용
    // CardSequenceHandler.cs의 PlayCardWithTiming 메서드 수정
    public IEnumerator PlayCardWithTiming(CardUI cardUI, Card card)
    {
        if (cardUI == null || card == null) yield break;
        
        // 카드 애니메이션 타입 결정
        CardAnimationType animType = DetermineCardAnimationType(card);
        
        // 카드 애니메이션 재생 - 단순히 기본 메서드만 호출
        cardUI.PlayUseAnimation();
        
        // 카드 타입에 따른 대기 시간 설정
        float waitTime = animationDurations[animType];
        
        // 디버그 로그
        Debug.Log($"카드 {card.cardName} 재생 중, {waitTime}초 대기");
        
        // 대기
        yield return new WaitForSeconds(waitTime);
    }
    
    // 이펙트 생성 헬퍼 메서드
    public GameObject CreateEffect(GameObject effectPrefab, Vector3 position, float destroyDelay = 2f)
    {
        if (effectPrefab == null) return null;
        
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(effect, destroyDelay);
        return effect;
    }
    
    // 카드 특수 효과 애니메이션 재생 (외부에서 호출 가능)
    public void PlaySpecialEffectForCard(Card card, Vector3 position)
    {
        if (card == null) return;
        
        string cardName = card.cardName.ToLower();
        
        if (cardName.Contains("면도날") && slashEffectPrefab != null)
        {
            CreateEffect(slashEffectPrefab, position);
        }
        else if (cardName.Contains("숫돌") && enhanceEffectPrefab != null)
        {
            CreateEffect(enhanceEffectPrefab, position);
        }
        else if (cardName.Contains("피폭발") && explosionEffectPrefab != null)
        {
            CreateEffect(explosionEffectPrefab, position);
        }
        else if (cardName.Contains("깨물기") && healEffectPrefab != null)
        {
            CreateEffect(healEffectPrefab, position);
        }
    }
    
    // 카드 시퀀스 미리보기 (UI에서 카드 순서 보여줄 때 사용)
    public string GetCardSequencePreview(List<Card> cards)
    {
        if (cards == null || cards.Count == 0)
            return "카드가 없습니다.";
            
        string preview = "카드 시퀀스 미리보기:\n";
        
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            preview += $"{i+1}. {card.cardName}";
            
            // 카드 타입에 따른 효과 힌트 추가
            if (card.cardType == CardType.Attack)
                preview += " 🗡️";
            else if (card.cardType == CardType.Defense)
                preview += " 🛡️";
            else if (card.cardType == CardType.Buff)
                preview += " ✨";
            else if (card.cardType == CardType.Special)
                preview += " 🔮";
                
            preview += "\n";
        }
        
        return preview;
    }
    
    // 카드 시퀀스 최적화 제안 (덱 빌딩 단계에서 활용)
    public string GetSequenceOptimizationTips(List<Card> cards)
    {
        if (cards == null || cards.Count < 2)
            return "카드가 충분하지 않습니다.";
            
        List<string> tips = new List<string>();
        
        // 숫돌 카드 위치 확인
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardName.Contains("숫돌") && i == cards.Count - 1)
            {
                tips.Add("숫돌 카드가 마지막에 있어 효과가 발휘되지 않아요.");
            }
            else if (cards[i].cardName.Contains("숫돌"))
            {
                Card nextCard = cards[i + 1];
                if (nextCard.bleed == 0)
                {
                    tips.Add($"숫돌 다음에 {nextCard.cardName}이(가) 있어 출혈 강화 효과가 발휘되지 않아요.");
                }
            }
        }
        
        // 피폭발 카드 전에 출혈 효과 확인
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].cardName.Contains("피폭발") && i == 0)
            {
                tips.Add("피폭발 카드가 첫 번째에 있어 출혈이 쌓이기 전에 사용돼요.");
            }
        }
        
        // 면도날 카드 연속 배치 확인
        bool hasConsecutiveRazors = false;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i].cardName.Contains("면도날") && cards[i + 1].cardName.Contains("면도날"))
            {
                hasConsecutiveRazors = true;
                break;
            }
        }
        
        if (!hasConsecutiveRazors && cards.FindAll(c => c.cardName.Contains("면도날")).Count >= 2)
        {
            tips.Add("면도날 카드를 연속으로 배치하면 추가 데미지를 줄 수 있어요.");
        }
        
        // 결과 포맷팅
        if (tips.Count == 0)
            return "최적의 카드 순서입니다!";
            
        string result = "카드 순서 최적화 팁:\n";
        for (int i = 0; i < tips.Count; i++)
        {
            result += $"- {tips[i]}\n";
        }
        
        return result;
    }
}