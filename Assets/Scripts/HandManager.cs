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
    public static HandManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool IsAnimating()
    {
        return isAnimating;
    }

    private void Start()
    {
        Debug.Log("HandManager started. Drawing starting hand.");
        DrawStartingHand();
        StartCoroutine(DelayedPlayCardsSequentially()); // ✅ 일정 시간 기다린 후 플레이 시작
    }
    public void AddCardToHand(Card card)
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

        Debug.Log($"Added card to hand: {card.cardName}. Current hand size: {hand.Count}");
    }
    else
    {
        Debug.Log("Hand is full. Cannot add more cards.");
    }
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

    private IEnumerator DelayedPlayCardsSequentially()
    {
        yield return new WaitForSeconds(2f); // ✅ 2초 대기 후 플레이 시작
        StartCoroutine(PlayCardsSequentially());
    }

private bool isAnimating = false; // ✅ 애니메이션 진행 여부 확인 변수

    private IEnumerator PlayCardsSequentially()
    {
        Debug.Log("Starting sequential card play.");
        isAnimating = true; // ✅ 애니메이션 시작
        while (hand.Count > 0)
        {
            Card card = hand[0];
            Debug.Log($"Using card: {card.cardName}");
            card.Use();
            hand.RemoveAt(0);

            CardUI cardUI = handUI.GetChild(0).GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.PlayUseAnimation();
                yield return new WaitForSeconds(2f);
            }

            DeckManager.Instance.AddToGraveyard(card);
            Debug.Log($"Moved {card.cardName} to graveyard. Current graveyard size: {DeckManager.Instance.graveyard.Count}");
        }
        isAnimating = false; // ✅ 애니메이션 종료
        Debug.Log("All cards played from hand.");
    }

}
