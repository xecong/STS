using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private List<Card> deck = new List<Card>();
    private List<Card> graveyard = new List<Card>();
    
    // UI 텍스트 컴포넌트 참조 추가
    public TMP_Text deckCountText;
    public TMP_Text graveyardCountText;

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

    private void Update()
    {
        // UI 텍스트 업데이트
        UpdateCountTexts();
    }

    // 덱과 묘지 카운트 텍스트 업데이트
    private void UpdateCountTexts()
    {
        if (deckCountText != null)
        {
            deckCountText.text = $"덱: {deck.Count}장";
        }
        
        if (graveyardCountText != null)
        {
            graveyardCountText.text = $"묘지: {graveyard.Count}장";
        }
    }

    public void InitializeDeck(List<string> cardNames)
    {
        deck.Clear();
        foreach (string cardName in cardNames)
        {
            Card card = Resources.Load<Card>("Cards/" + cardName);
            if (card != null)
            {
                deck.Add(card);
            }
            else
            {
                Debug.LogError($"❌ Card not found in Resources: {cardName}");
            }
        }
        graveyard.Clear();
        ShuffleDeck();
        
        // 덱 초기화 후 UI 업데이트
        UpdateCountTexts();
    }

    public int GetDeckSize()
    {
        return deck.Count;
    }

    public int GetGraveyardSize()
    {
        return graveyard.Count;
    }

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
        
        // 카드 드로우 후 UI 업데이트
        UpdateCountTexts();
        return drawnCard;
    }

    public void MoveToGraveyard(Card card)
    {
        graveyard.Add(card);
        // 묘지에 카드 추가 후 UI 업데이트
        UpdateCountTexts();
    }

    public void RecycleGraveyard()
    {
        int graveyardSize = graveyard.Count;
        Debug.Log($"묘지에서 {graveyardSize}장의 카드를 덱으로 재활용합니다.");
        
        deck.AddRange(graveyard);
        graveyard.Clear();
        ShuffleDeck();
        
        Debug.Log($"묘지 재활용 완료. 현재 덱 크기: {deck.Count}, 묘지 크기: {graveyard.Count}");
        
        // 묘지 재활용 후 UI 업데이트
        UpdateCountTexts();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public List<string> GetCurrentDeckNames()
    {
        List<string> deckNames = new List<string>();
        foreach (Card card in deck)
        {
            deckNames.Add(card.cardName);
        }
        return deckNames;
    }
}