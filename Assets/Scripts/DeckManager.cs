using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    private List<Card> loadedDeck = new List<Card>(); // JSON에서 불러온 원본 덱

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad 제거 - 씬 전환 시 유지되지 않도록 변경
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadDeckFromSave(); // JSON에서 덱 불러오기
    }

    // 덱 설정하기
	// 수정된 코드
	public void SetDeck(List<Card> newDeck)
	{
		if (newDeck != null && newDeck.Count > 0)
		{
			loadedDeck = new List<Card>(newDeck); // 'loadedDeck' 변수 사용
			Debug.Log($"덱 설정 완료: {loadedDeck.Count}장의 카드");
		}
		// ...
	}
    // 현재 덱 가져오기
    public List<Card> GetDeck()
    {
        return loadedDeck;
    }

    // 덱 저장하기
    public void SaveDeck()
    {
        List<string> deckNames = new List<string>();
        foreach (Card card in loadedDeck)
        {
            deckNames.Add(card.cardName);
        }
        SaveLoadManager.SaveDeck(deckNames);
        Debug.Log("Deck saved through DeckManager");
    }

    // DeckManager.cs의 LoadDeckFromSave() 메서드 수정
    public void LoadDeckFromSave()
    {
        DeckData loadedData = SaveLoadManager.LoadDeck();
        if (loadedData != null && loadedData.cardNames != null && loadedData.cardNames.Count > 0)
        {
            loadedDeck.Clear();
            foreach (string cardName in loadedData.cardNames)
            {
                // 디버그 로그 추가
                Debug.Log($"🃏 카드 로드 시도: {cardName}");

                Card card = Resources.Load<Card>("Cards/" + cardName);
                if (card != null)
                {
                    loadedDeck.Add(card);
                    Debug.Log($"✅ 카드 로드 성공: {cardName}");
                }
                else
                {
                    Debug.LogError($"❌ 카드를 찾을 수 없음: {cardName}");
                }
            }
            
            // CardManager가 존재하면 덱 초기화
            if (CardManager.Instance != null)
            {
                List<string> cardNames = loadedDeck.ConvertAll(card => card.cardName);
                CardManager.Instance.InitializeDeck(cardNames);
                Debug.Log($"🎉 덱 초기화 완료. 총 {cardNames.Count}장의 카드");
            }
        }
        else
        {
            Debug.LogError("😱 덱 데이터가 없거나 비어있어요!");
        }
    }
}