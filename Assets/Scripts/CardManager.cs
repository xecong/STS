using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>(); // 덱
    public List<Card> hand = new List<Card>(); // 핸드
    public List<Card> graveyard = new List<Card>(); // 무덤
    
    public int handSize = 5; // 매 턴 뽑을 카드 수

    void Start()
    {
        ShuffleDeck(); // 시작할 때 덱 섞기
        DrawCards();   // 첫 턴 카드 뽑기
    }

    // 덱을 섞는 함수
    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // 카드 뽑기
    public void DrawCards()
    {
        for (int i = 0; i < handSize; i++)
        {
            if (deck.Count == 0)
            {
                RecycleDeck(); // 덱이 비면 무덤에서 다시 채우기
            }
            
            if (deck.Count > 0)
            {
                Card drawnCard = deck[0];
                deck.RemoveAt(0);
                hand.Add(drawnCard);
            }
        }
    }

    // 핸드의 카드 자동 사용
    public void PlayCards()
    {
        foreach (Card card in new List<Card>(hand))
        {
            card.Use();
            graveyard.Add(card);
        }
        hand.Clear();
    }

    // 무덤을 다시 덱으로 이동시키는 함수
    public void RecycleDeck()
    {
        if (graveyard.Count > 0)
        {
            deck.AddRange(graveyard);
            graveyard.Clear();
            ShuffleDeck();
        }
    }
}
