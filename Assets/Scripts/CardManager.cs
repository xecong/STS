using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private List<Card> deck = new List<Card>(); // ✅ 동적으로 관리할 덱
    private List<Card> graveyard = new List<Card>(); // ✅ 묘지

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

    /// <summary>
    /// ✅ DeckManager에서 불러온 덱을 저장하고 초기화
    /// </summary>
    public void InitializeDeck(List<Card> loadedDeck)
    {
        deck = new List<Card>(loadedDeck);
        graveyard.Clear();
        ShuffleDeck();
        Debug.Log($"✅ Deck initialized with {deck.Count} cards.");
    }

    /// <summary>
    /// ✅ 덱에서 카드 한 장 드로우 (덱이 부족하면 묘지 리사이클)
    /// </summary>
    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            if (graveyard.Count > 0)
            {
                RecycleGraveyard();
            }
            else
            {
                Debug.LogWarning("⚠ No cards left in deck or graveyard!");
                return null;
            }
        }

        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        Debug.Log($"🃏 Drew card: {drawnCard.cardName}. Remaining deck size: {deck.Count}");
        return drawnCard;
    }

    /// <summary>
    /// ✅ 사용한 카드를 묘지로 이동
    /// </summary>
    public void MoveToGraveyard(Card card)
    {
        graveyard.Add(card);
        Debug.Log($"☠️ Moved {card.cardName} to graveyard. Graveyard size: {graveyard.Count}");
    }

    /// <summary>
    /// ✅ 묘지를 다시 덱으로 이동 후 셔플
    /// </summary>
    public void RecycleGraveyard()
    {
        if (graveyard.Count > 0)
        {
            deck.AddRange(graveyard);
            graveyard.Clear();
            ShuffleDeck();
            Debug.Log("♻️ Graveyard shuffled back into deck!");
        }
    }

    /// <summary>
    /// ✅ 덱을 섞음
    /// </summary>
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("🔀 Deck shuffled!");
    }
}
