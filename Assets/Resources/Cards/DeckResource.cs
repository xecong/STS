using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeckResource", menuName = "Card System/Deck Resource")]
public class DeckResource : ScriptableObject
{
    [Header("덱 정보")]
    public string deckName = "기본 덱";
    [TextArea(2, 4)]
    public string deckDescription = "기본 카드로 구성된 덱입니다.";
    
    [Header("카드 목록")]
    [Tooltip("이 덱에 포함된 카드들")]
    public List<Card> availableCards = new List<Card>();
    
    [Header("시작 덱 설정")]
    [Tooltip("이 카드들이 게임 시작 시 기본 덱으로 설정됩니다.")]
    public List<Card> startingDeck = new List<Card>();
    
    // 카드 이름 목록 반환
    public List<string> GetCardNames()
    {
        List<string> cardNames = new List<string>();
        foreach (Card card in startingDeck)
        {
            if (card != null)
                cardNames.Add(card.cardName);
        }
        return cardNames;
    }
}