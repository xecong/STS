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

    private bool isAnimating = false; // ✅ 애니메이션 진행 여부 확인 변수

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

    private void Start()
    {
        Debug.Log("HandManager started. Drawing starting hand.");
        DrawStartingHand();
        StartCoroutine(DelayedPlayCardsSequentially()); // ✅ 일정 시간 기다린 후 플레이 시작
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    /// <summary>
    /// ✅ 덱에서 처음 시작할 때 5장 드로우
    /// </summary>
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
                DrawCard(deck[0]); // ✅ 카드 드로우 메서드 활용
                deck.RemoveAt(0);
            }
        }
        Debug.Log($"Hand size after drawing: {hand.Count}");
    }

    /// <summary>
    /// ✅ 덱에서 카드 한 장 드로우해서 핸드에 추가
    /// </summary>
    public void DrawCard(Card card)
    {
        if (hand.Count < maxHandSize)
        {
            AddCardToHand(card); // ✅ `AddCardToHand()`를 직접 호출!
        }
        else
        {
            Debug.Log("Hand is full. Cannot draw more cards.");
        }
    }

    /// <summary>
    /// ✅ 카드를 핸드에 추가하고 UI 생성
    /// </summary>
    public void AddCardToHand(Card card)
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

    /// <summary>
    /// ✅ 턴마다 5장 드로우 (덱이 부족하면 가능한 만큼만)
    /// </summary>
    public void DrawCards()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            if (DeckManager.Instance.GetDeck().Count == 0)
            {
                DeckManager.Instance.MoveGraveyardToDeck(); // ✅ 묘지에서 덱으로 이동
            }

            if (DeckManager.Instance.GetDeck().Count > 0)
            {
                Card drawnCard = DeckManager.Instance.GetDeck()[0];
                DeckManager.Instance.GetDeck().RemoveAt(0);
                DrawCard(drawnCard); // ✅ DrawCard() 호출!
            }
            else
            {
                Debug.Log("No more cards to draw.");
                break;
            }
        }
    }

    /// <summary>
    /// ✅ 2초 대기 후 자동 플레이 실행
    /// </summary>
    private IEnumerator DelayedPlayCardsSequentially()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(PlayCardsSequentially());
    }

    /// <summary>
    /// ✅ 핸드의 모든 카드를 자동으로 사용하고 무덤으로 이동
    /// </summary>
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

            // ✅ UI에서도 삭제 & 애니메이션 적용
            if (handUI.childCount > 0)
            {
                CardUI cardUI = handUI.GetChild(0).GetComponent<CardUI>();
                if (cardUI != null)
                {
                    cardUI.PlayUseAnimation();
                    yield return new WaitForSeconds(2f); // ✅ 카드 애니메이션 후 다음 카드 사용
                }
            }

            DeckManager.Instance.AddToGraveyard(card);
            Debug.Log($"Moved {card.cardName} to graveyard. Current graveyard size: {DeckManager.Instance.graveyard.Count}");
        }

        isAnimating = false; // ✅ 애니메이션 종료
        Debug.Log("All cards played from hand.");
    }

    /// <summary>
    /// ✅ 게임 종료 시 덱 저장
    /// </summary>
    void OnApplicationQuit()
    {
        DeckManager.Instance.SaveDeck();
    }
}
