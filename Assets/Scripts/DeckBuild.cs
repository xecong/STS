using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckBuild : MonoBehaviour
{
    [Header("덱 리소스")]
    public DeckResource deckResource;  // 인스펙터에서 할당
    
    [Header("선택된 덱")]
    public List<Card> selectedDeck = new List<Card>(); // 플레이어가 선택한 덱
    
    [Header("UI 참조")]
    public Transform cardDisplayParent; // 카드 표시 영역
    public Transform selectedDeckParent; // 선택된 덱 표시 영역
    public GameObject cardUIPrefab; // 카드 UI 프리팹
    public Button startButton; // 시작 버튼
    public TextMeshProUGUI deckInfoText; // 덱 정보 텍스트
    
    [Header("덱 설정")]
    public int minDeckSize = 10; // 최소 덱 크기
    public int maxDeckSize = 40; // 최대 덱 크기

    void Start()
    {
        if (deckResource == null)
        {
            Debug.LogError("덱 리소스가 할당되지 않았어...! 인스펙터에서 확인해줘!!");
            
            // 대체 방법으로 리소스에서 로드 시도
            deckResource = Resources.Load<DeckResource>("DeckResource/DefaultDeck");
            
            if (deckResource == null)
            {
                Debug.LogError("기본 덱 리소스도 찾을 수 없어...! DeckResource를 만들고 인스펙터에 할당해줘!");
                return;
            }
        }
        
        DisplayAvailableCards();
        startButton.onClick.AddListener(StartGame); // 시작 버튼 이벤트 등록
        startButton.interactable = false; // 덱이 준비되기 전까지 비활성화
        UpdateDeckInfo();
    }

    void DisplayAvailableCards()
    {
        // 기존 카드 UI 모두 제거
        foreach (Transform child in cardDisplayParent)
        {
            Destroy(child.gameObject);
        }
        
        if (deckResource.availableCards.Count == 0)
        {
            Debug.LogWarning("표시할 카드가 없어... DeckResource에 카드를 추가해줘!");
            return;
        }
        
        Debug.Log($"표시할 카드 수: {deckResource.availableCards.Count}");
        
        foreach (Card card in deckResource.availableCards)
        {
            if (card == null) continue;
            
            GameObject cardUI = Instantiate(cardUIPrefab, cardDisplayParent);
            
            // CardUI 컴포넌트 확인
            CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
            if (cardUIComponent != null)
            {
                cardUIComponent.SetCardData(card);
                Debug.Log($"카드 UI 업데이트 완료: {card.cardName}");
            }
            else
            {
                // EnhancedCardUI 시도
                EnhancedCardUI enhancedCardUI = cardUI.GetComponent<EnhancedCardUI>();
                if (enhancedCardUI != null)
                {
                    enhancedCardUI.SetCardData(card);
                    Debug.Log($"EnhancedCardUI로 카드 표시: {card.cardName}");
                }
                else
                {
                    Debug.LogError($"카드 '{card.cardName}'에 UI 컴포넌트가 없어...! 프리팹을 확인해줘!");
                }
            }
            
            // 버튼 컴포넌트에 이벤트 추가
            Button button = cardUI.GetComponent<Button>();
            if (button != null)
            {
                Card capturedCard = card; // 클로저 문제 방지를 위해 카드 캡처
                button.onClick.AddListener(() => SelectCard(capturedCard));
                Debug.Log($"카드 '{card.cardName}'에 버튼 이벤트 추가됨");
            }
            else
            {
                Debug.LogError($"카드 '{card.cardName}'에 Button 컴포넌트가 없어...! 프리팹을 확인해줘!");
            }
        }
    }
    
    public void SelectCard(Card card)
    {
        if (selectedDeck.Count < maxDeckSize)
        {
            selectedDeck.Add(card);
            Debug.Log($"{card.cardName} 덱에 추가됐어! 현재 덱 크기: {selectedDeck.Count}");
            
            // 선택된 덱에 카드 UI 추가
            GameObject cardUI = Instantiate(cardUIPrefab, selectedDeckParent);
            
            // CardUI 컴포넌트 확인
            CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
            if (cardUIComponent != null)
            {
                cardUIComponent.SetCardData(card);
            }
            
            // 삭제 버튼 이벤트 추가
            Button button = cardUI.GetComponent<Button>();
            if (button != null)
            {
                int index = selectedDeck.Count - 1;
                button.onClick.AddListener(() => RemoveCardFromDeck(index));
            }
        }
        else
        {
            Debug.Log("덱이 가득 찼어! 더 이상 카드를 추가할 수 없어...");
        }
        
        CheckDeckReady();
        UpdateDeckInfo();
    }

    public void RemoveCardFromDeck(int index)
    {
        if (index >= 0 && index < selectedDeck.Count)
        {
            Card removedCard = selectedDeck[index];
            selectedDeck.RemoveAt(index);
            Debug.Log($"{removedCard.cardName} 덱에서 제거됐어! 현재 덱 크기: {selectedDeck.Count}");
            
            // UI 갱신 - 모든 카드를 제거하고 다시 표시
            foreach (Transform child in selectedDeckParent)
            {
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < selectedDeck.Count; i++)
            {
                Card card = selectedDeck[i];
                GameObject cardUI = Instantiate(cardUIPrefab, selectedDeckParent);
                
                // CardUI 컴포넌트 확인
                CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
                if (cardUIComponent != null)
                {
                    cardUIComponent.SetCardData(card);
                }
                
                // 삭제 버튼 이벤트 추가
                Button button = cardUI.GetComponent<Button>();
                if (button != null)
                {
                    int tempIndex = i;
                    button.onClick.AddListener(() => RemoveCardFromDeck(tempIndex));
                }
            }
        }
        
        CheckDeckReady();
        UpdateDeckInfo();
    }

    void CheckDeckReady()
    {
        startButton.interactable = selectedDeck.Count >= minDeckSize; // 덱이 최소 크기 이상이면 활성화
    }

    void UpdateDeckInfo()
    {
        if (deckInfoText != null)
        {
            deckInfoText.text = $"{selectedDeck.Count} / {minDeckSize} 카드 선택됨";
        }
    }

    public void ConfirmDeck()
    {
        // DeckManager가 존재하면 덱 설정
        if (DeckManager.Instance != null)
        {
            DeckManager.Instance.SetDeck(selectedDeck);
        }
        
        // 선택된 덱 저장
        List<string> cardNames = new List<string>();
        foreach (Card card in selectedDeck)
        {
            cardNames.Add(card.cardName);
        }
        SaveLoadManager.SaveDeck(cardNames);
        
        Debug.Log("덱 확정하고 저장했어!");
    }

    public void StartGame()
    {
        ConfirmDeck(); // 게임 시작 전에 덱을 저장
        Debug.Log("선택한 덱으로 게임을 시작할게!");
        SceneManager.LoadScene("GameScene"); // 실제 게임 씬으로 이동
    }
    
    // 테스트 덱 로드 (개발용)
    public void LoadTestDeck()
    {
        if (deckResource != null && deckResource.startingDeck.Count > 0)
        {
            selectedDeck.Clear();
            selectedDeck.AddRange(deckResource.startingDeck);
            
            Debug.Log($"테스트 덱 로드 완료! 카드 수: {selectedDeck.Count}");
            
            // UI 갱신
            foreach (Transform child in selectedDeckParent)
            {
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < selectedDeck.Count; i++)
            {
                Card card = selectedDeck[i];
                GameObject cardUI = Instantiate(cardUIPrefab, selectedDeckParent);
                
                CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
                if (cardUIComponent != null)
                {
                    cardUIComponent.SetCardData(card);
                }
                
                Button button = cardUI.GetComponent<Button>();
                if (button != null)
                {
                    int tempIndex = i;
                    button.onClick.AddListener(() => RemoveCardFromDeck(tempIndex));
                }
            }
            
            CheckDeckReady();
            UpdateDeckInfo();
        }
        else
        {
            Debug.LogWarning("테스트 덱을 로드할 수 없어...! DeckResource의 startingDeck이 비어있어...");
        }
    }
}