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
	public static void SaveDeck(List<Card> deck)
	{
		// 빈 덱은 저장하지 않도록 검사 추가
		if (deck == null || deck.Count == 0)
		{
			Debug.LogWarning("빈 덱을 저장하려고 했습니다! 무시합니다...");
			return;
		}
		
		DeckData data = new DeckData();
		data.cardNames = new List<string>();
		
		foreach (Card card in deck)
		{
			data.cardNames.Add(card.cardName);
		}
		
		string json = JsonUtility.ToJson(data);
		File.WriteAllText(savePath, json);
		Debug.Log($"덱 저장 완료: {deck.Count}장의 카드!");
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