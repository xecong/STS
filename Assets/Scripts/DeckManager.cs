using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; } // 싱글턴 인스턴스 추가

    public List<CardData> deck = new List<CardData>();
    public List<CardData> graveyard = new List<CardData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    public void SetDeck(List<CardData> newDeck)
    {
        deck = new List<CardData>(newDeck);
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public List<CardData> DrawCards(int count)
    {
        List<CardData> drawnCards = new List<CardData>();

        for (int i = 0; i < count; i++)
        {
            if (deck.Count == 0)
            {
                if (graveyard.Count == 0) break;
                RefillDeckFromGraveyard();
            }

            drawnCards.Add(deck[0]);
            deck.RemoveAt(0);
        }

        return drawnCards;
    }

    public void RefillDeckFromGraveyard()
    {
        Debug.Log("🔄 덱이 비었습니다! 묘지에서 덱을 다시 채웁니다.");
        deck.AddRange(graveyard);
        graveyard.Clear();
        ShuffleDeck();
    }
}
