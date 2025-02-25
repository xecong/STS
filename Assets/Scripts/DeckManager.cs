using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<Card> loadedDeck = new List<Card>(); // ✅ JSON에서 불러온 원본 덱

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

    private void Start()
    {
        LoadDeckFromSave(); // ✅ JSON에서 덱 불러오기
    }

    public void LoadDeckFromSave()
    {
        DeckData loadedData = SaveLoadManager.LoadDeck();
        if (loadedData != null)
        {
            loadedDeck.Clear();
            foreach (string cardName in loadedData.cardNames)
            {
                Card card = Resources.Load<Card>("Cards/" + cardName);
                if (card != null)
                {
                    loadedDeck.Add(card);
                }
                else
                {
                    Debug.LogError($"❌ Card not found in Resources: {cardName}");
                }
            }
        }
        CardManager.Instance.InitializeDeck(loadedDeck); // ✅ CardManager에 초기 덱 전달
    }
}
