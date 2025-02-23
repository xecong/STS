using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckBuilder : MonoBehaviour
{
    public List<Card> availableCards; // 선택 가능한 카드 목록
    public List<Card> selectedDeck = new List<Card>(); // 플레이어가 선택한 덱
    
    public Transform cardDisplayParent;
    public GameObject cardUIPrefab; // 카드 UI 프리팹
    public Button startButton; // 시작 버튼
    public int minDeckSize = 10; // 최소 덱 크기
    public int maxDeckSize = 40; // 최대 덱 크기

    void Start()
    {
        DisplayAvailableCards();
        startButton.onClick.AddListener(StartGame); // 시작 버튼 이벤트 등록
        startButton.interactable = false; // 덱이 준비되기 전까지 비활성화
    }

    void DisplayAvailableCards()
    {
        foreach (Card card in availableCards)
        {
            GameObject cardUI = Instantiate(cardUIPrefab, cardDisplayParent);
            cardUI.GetComponentInChildren<Text>().text = card.cardName;
            Button button = cardUI.GetComponent<Button>();
            button.onClick.AddListener(() => SelectCard(card));
        }
    }

    public void SelectCard(Card card)
    {
        if (selectedDeck.Count < maxDeckSize)
        {
            selectedDeck.Add(card);
            Debug.Log($"{card.cardName} added to deck. Current deck size: {selectedDeck.Count}");
        }
        else
        {
            Debug.Log("Deck is full! Cannot add more cards.");
        }
        
        CheckDeckReady();
    }

    void CheckDeckReady()
    {
        startButton.interactable = selectedDeck.Count >= minDeckSize; // 덱이 최소 10장 이상이면 활성화
    }

    public void ConfirmDeck()
    {
        DeckManager.Instance.SetDeck(selectedDeck); // 선택한 덱 저장
        Debug.Log("Deck confirmed and saved! Cards: " + string.Join(", ", selectedDeck));
    }

    public void StartGame()
    {
        ConfirmDeck(); // ✅ 게임 시작 전에 덱을 저장하도록 변경
        Debug.Log("Game Started with selected deck!");
        SceneManager.LoadScene("GameScene"); // 실제 게임 씬으로 이동
    }
}
