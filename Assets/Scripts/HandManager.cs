using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public List<Card> hand = new List<Card>(); // 현재 핸드에 있는 카드 리스트
    public Transform handUI; // 핸드 UI 패널 (Horizontal Layout Group 사용)
    public GameObject cardUIPrefab; // 카드 UI 프리팹
    public int maxHandSize = 5; // 최대 핸드 크기

    private void Start()
    {
        Debug.Log("HandManager started. Drawing starting hand.");
        DrawStartingHand();
    }

    void DrawStartingHand()
    {
        List<Card> deck = DeckManager.Instance.GetDeck();
        Debug.Log($"Current deck size before drawing: {deck.Count}");
        
        // ✅ 덱이 비었으면 묘지를 덱으로 되돌리고 셔플
        if (deck.Count == 0)
        {
            Debug.Log("Deck is empty. Moving graveyard to deck and shuffling.");
            DeckManager.Instance.MoveGraveyardToDeck();
            deck = DeckManager.Instance.GetDeck();
            Debug.Log($"New deck size after reshuffling: {deck.Count}");
        }

        for (int i = 0; i < maxHandSize; i++)
        {
            if (deck.Count > 0)
            {
                Debug.Log($"Drawing card {deck[0].cardName} from deck.");
                DrawCard(deck[0]);
                deck.RemoveAt(0);
            }
        }
        Debug.Log($"Hand size after drawing: {hand.Count}");
        StartCoroutine(PlayCardsSequentially()); // ✅ 모든 카드가 뽑힌 후 순차적으로 사용
    }

    public void DrawCard(Card card)
    {
        if (hand.Count < maxHandSize)
        {
            hand.Add(card);
            GameObject cardUI = Instantiate(cardUIPrefab, handUI);
            
            CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
            if (cardUIComponent != null)
            {
                cardUIComponent.SetCardData(card);
                Debug.Log($"Card UI updated for: {card.cardName}");
            }
            
            Debug.Log($"Drew card: {card.cardName}. Current hand size: {hand.Count}");
        }
        else
        {
            Debug.Log("Hand is full. Cannot draw more cards.");
        }
    }

    private IEnumerator PlayCardsSequentially()
    {
        Debug.Log("Starting sequential card play.");
        while (hand.Count > 0)
        {
            Card card = hand[0];
            Debug.Log($"Using card: {card.cardName}");
            card.Use();
            hand.RemoveAt(0);
            
            CardUI cardUI = handUI.GetChild(0).GetComponent<CardUI>(); // ✅ 첫 번째 카드의 UI 가져오기
            if (cardUI != null)
            {
                cardUI.PlayUseAnimation(); // ✅ 카드 UI 애니메이션 실행
            }
            
            DeckManager.Instance.AddToGraveyard(card); // ✅ 사용된 카드를 묘지로 이동
            //Debug.Log($"Moved {card.cardName} to graveyard. Current graveyard size: {DeckManager.Instance.GetGraveyardSize()}");
            
            yield return new WaitForSeconds(1f); // ✅ 1초 간격으로 사용
        }
        Debug.Log("All cards played from hand.");
    }
}
