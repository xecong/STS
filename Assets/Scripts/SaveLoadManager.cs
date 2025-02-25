using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DeckData
{
    public List<string> cardNames; // 카드 이름 저장
}

public class SaveLoadManager : MonoBehaviour
{
    private static string savePath => Application.persistentDataPath + "/deck_save.json";

    // 매개변수가 없는 SaveDeck 메소드 - 현재 덱을 저장
    public static void SaveDeck()
    {
        SaveDeck(CardManager.Instance.GetCurrentDeckNames());
    }
    
    // 카드 이름 리스트를 매개변수로 받는 SaveDeck 메소드
    public static void SaveDeck(List<string> cardNames)
    {
        DeckData data = new DeckData
        {
            cardNames = cardNames
        };
        
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log($"Deck saved with {cardNames.Count} cards!");
    }
    
    // Card 객체 리스트를 매개변수로 받는 SaveDeck 메소드
    public static void SaveDeck(List<Card> cards)
    {
        List<string> cardNames = new List<string>();
        foreach (Card card in cards)
        {
            cardNames.Add(card.cardName);
        }
        SaveDeck(cardNames);
    }

    public static DeckData LoadDeck()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            DeckData data = JsonUtility.FromJson<DeckData>(json);
            Debug.Log("Deck Loaded!");
            return data;
        }
        else
        {
            Debug.LogWarning("No saved deck found!");
            return null;
        }
    }
}