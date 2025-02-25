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

    public static void SaveDeck(List<Card> deck)
    {
        DeckData data = new DeckData();
        data.cardNames = deck.ConvertAll(card => card.cardName);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Deck Saved! " + savePath);
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
