using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<Card> deck = new List<Card>();
    private List<Card> graveyard = new List<Card>(); // ✅ 묘지(사용된 카드 목록)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDeck(List<Card> newDeck)
    {
        deck = new List<Card>(newDeck);
        ShuffleDeck(); // ✅ 덱을 저장할 때 자동으로 셔플
        Debug.Log("Deck saved and shuffled! Cards: " + string.Join(", ", deck));
    }

    public List<Card> GetDeck()
    {
        return new List<Card>(deck);
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
        Debug.Log("Deck shuffled!");
    }

    public void MoveGraveyardToDeck()
    {
        if (graveyard.Count > 0)
        {
            deck.AddRange(graveyard);
            graveyard.Clear();
            ShuffleDeck(); // ✅ 묘지를 덱으로 되돌릴 때 셔플
            Debug.Log("Graveyard shuffled back into deck!");
        }
    }

    public void AddToGraveyard(Card card)
    {
        graveyard.Add(card);
        Debug.Log($"{card.cardName} moved to graveyard.");
    }
}
