using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    public List<CardData> availableCards; // 선택 가능한 카드들 (Inspector에서 등록)
    public List<CardData> selectedDeck = new List<CardData>(); // 플레이어가 선택한 카드들
    public GameObject cardSelectionPanel; // 카드 버튼들이 있는 UI 패널
    public HandManager playerHand; // 플레이어 핸드
    public DeckManager playerDeck; // 플레이어 덱
	public GameObject startGameButton; // ✅ GameObject 타입으로 변경

    private void Start()
    {
        startGameButton.SetActive(false); // 처음에는 비활성화
    }

	public void SelectCard(int cardIndex)
	{
		if (cardIndex < 0 || cardIndex >= availableCards.Count) return;

		CardData selectedCard = availableCards[cardIndex];

		selectedDeck.Add(selectedCard); // 중복 추가 허용
		Debug.Log($"✔ {selectedCard.cardName} 덱에 추가됨! (총 {selectedDeck.Count}장)");

		if (selectedDeck.Count >= 5) // 최소 5장 이상 선택해야 시작 가능
		{
			startGameButton.SetActive(true);
		}
	}


	public void StartGame()
	{
		if (selectedDeck.Count < 5) return;

		playerDeck.SetDeck(selectedDeck);
		playerDeck.ShuffleDeck();

		cardSelectionPanel.SetActive(false);
		startGameButton.SetActive(false);

		List<CardData> drawnCards = playerDeck.DrawCards(5);
		playerHand.DrawCards(drawnCards);

		Debug.Log("🎮 게임 시작! 뽑은 카드 목록:");
		foreach (CardData card in drawnCards)
		{
			Debug.Log($"🃏 {card.cardName}");
		}

		// 카드 자동 사용 시작
		StartCoroutine(playerHand.PlayHand());
	}

}
