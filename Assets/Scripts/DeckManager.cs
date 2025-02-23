using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public List<Card> selectedDeck = new List<Card>(); // 플레이어가 선택한 덱 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDeck(List<Card> deck)
    {
        selectedDeck = new List<Card>(deck); // 덱 저장
        Debug.Log("Deck saved! Cards: " + string.Join(", ", selectedDeck));
    }

    public List<Card> GetDeck()
    {
        return selectedDeck; // 저장된 덱 반환
    }
}
