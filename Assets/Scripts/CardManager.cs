using System.Collections.Generic;
using UnityEngine;
using TMPro;

public partial class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private List<Card> deck = new List<Card>();
    private List<Card> graveyard = new List<Card>();
    
    // UI 텍스트 컴포넌트 참조 추가
    public TMP_Text deckCountText;
    public TMP_Text graveyardCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // UI 텍스트 업데이트
        UpdateCountTexts();
    }

    // 덱과 묘지 카운트 텍스트 업데이트
    // CardManager.cs의 UpdateCountTexts 메서드 수정
    public void UpdateCountTexts() {
        if (deckCountText != null) {
            deckCountText.text = $"덱: {deck.Count}장";
        } 
        if (graveyardCountText != null) {
            graveyardCountText.text = $"묘지: {graveyard.Count}장";
        }
        // 핸드 텍스트도 필요하다면 비슷하게 업데이트
    }
    // CardManager.cs의 InitializeDeck 메서드 수정
	public void InitializeDeck(List<string> cardNames)
	{
		deck.Clear();
		Debug.Log($"덱 초기화 시작! 카드 이름 목록: {string.Join(", ", cardNames)}");

		// 한글 이름 -> 영어 이름 매핑 추가
		Dictionary<string, string> koreanToEnglish = new Dictionary<string, string>()
		{
			{ "깨물기", "Bite" },
			{ "피폭발", "BloodExplosion" },
			{ "면도날", "Razor" },
			{ "숫돌", "Stone" }
		};
		
		foreach (string cardName in cardNames)
		{
			bool cardLoaded = false;
			
			// 1. 먼저 이름 그대로 시도
			Card card = Resources.Load<Card>("Cards/" + cardName);
			
			if (card != null)
			{
				deck.Add(card);
				Debug.Log($"✅ 카드 로드 성공: {cardName}");
				cardLoaded = true;
			}
			// 2. 카드를 찾지 못했고, 한글 이름이 매핑에 있다면 영어 이름으로 시도
			else if (koreanToEnglish.ContainsKey(cardName))
			{
				string englishName = koreanToEnglish[cardName];
				card = Resources.Load<Card>("Cards/" + englishName);
				
				if (card != null)
				{
					deck.Add(card);
					Debug.Log($"✅ 한글 이름({cardName})을 영어 이름({englishName})으로 변환해서 로드 성공!");
					cardLoaded = true;
				}
			}
			
			// 3. 그래도 카드를 찾지 못했다면 경고 표시
			if (!cardLoaded)
			{
				Debug.LogError($"❌ 카드를 찾을 수 없어... 이름: {cardName}");
			}
		}
		
		graveyard.Clear();
		ShuffleDeck();
		
		// 덱 초기화 후 UI 업데이트
		UpdateCountTexts();
		
		Debug.Log($"덱 초기화 완료! 현재 덱 크기: {deck.Count}");
	}

    public int GetDeckSize()
    {
        return deck.Count;
    }

    public int GetGraveyardSize()
    {
        return graveyard.Count;
    }

    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            if (graveyard.Count > 0)
            {
                RecycleGraveyard();
            }
            else
            {
                Debug.LogWarning("⚠ No cards left in deck or graveyard!");
                return null;
            }
        }

        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        
        // 카드 드로우 후 UI 업데이트
        UpdateCountTexts();
        return drawnCard;
    }

    public void MoveToGraveyard(Card card)
    {
        graveyard.Add(card);
        // 묘지에 카드 추가 후 UI 업데이트
        UpdateCountTexts();
    }



    public void RecycleGraveyard() {
        int graveyardSize = graveyard.Count;
        Debug.Log($"묘지에서 {graveyardSize}장의 카드를 덱으로 재활용합니다.");

        // 묘지의 모든 카드를 덱으로 이동
        deck.AddRange(graveyard);
        graveyard.Clear();  // 묘지 리스트 비우기

        ShuffleDeck();  // 덱 셔플
        Debug.Log($"묘지 재활용 완료. 현재 덱 크기: {deck.Count}, 묘지 크기: {graveyard.Count}");

        // 재활용 후 UI 업데이트
        UpdateCountTexts();
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
    }

    public List<string> GetCurrentDeckNames()
    {
        List<string> deckNames = new List<string>();
        foreach (Card card in deck)
        {
            deckNames.Add(card.cardName);
        }
        return deckNames;
    }
}