using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<CardData> handCards = new List<CardData>(); // 현재 핸드
    public List<CardData> graveyard = new List<CardData>(); // 사용된 카드

    public bool HasCards()
    {
        return handCards.Count > 0;
    }

    public void DrawCards(List<CardData> newCards)
    {
        handCards.AddRange(newCards);
        Debug.Log($"🃏 {newCards.Count}장 핸드에 추가됨!");
    }

    public IEnumerator PlayHand()
    {
        while (handCards.Count > 0)
        {
            CardData nextCard = UseNextCard();
            if (nextCard != null)
            {
                Debug.Log($"✨ {nextCard.cardName} 사용됨!");
                yield return new WaitForSeconds(1f); // 카드 사용 간격
            }
        }
        Debug.Log("✅ 모든 카드 사용 완료!");
		GameManager.Instance.EndPlayerTurn(); // 턴 종료 확실히 실행
    }

    public CardData UseNextCard()
    {
        if (handCards.Count == 0) return null;

        CardData nextCard = handCards[0];
        handCards.RemoveAt(0);
		DeckManager.Instance.graveyard.Add(nextCard); // 묘지에 카드 추가
        return nextCard;
    }
}
