using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckBuild : MonoBehaviour
{
    public List<Card> availableCards; // 선택 가능한 카드 목록
    public List<Card> selectedDeck = new List<Card>(); // 플레이어가 선택한 덱
    
    public Transform cardDisplayParent; // 카드 표시 영역
    public Transform selectedDeckParent; // 선택된 덱 표시 영역
    public GameObject cardUIPrefab; // 카드 UI 프리팹
    public Button startButton; // 시작 버튼
    public TextMeshProUGUI deckInfoText; // 덱 정보 텍스트
    
    public int minDeckSize = 10; // 최소 덱 크기
    public int maxDeckSize = 40; // 최대 덱 크기

    void Start()
    {
        InitializeCardList(); // 테스트용 카드 목록 초기화
        DisplayAvailableCards();
        startButton.onClick.AddListener(StartGame); // 시작 버튼 이벤트 등록
        startButton.interactable = false; // 덱이 준비되기 전까지 비활성화
        UpdateDeckInfo();
    }

    // 테스트용 카드 목록 초기화
    void InitializeCardList()
    {
        if (availableCards == null || availableCards.Count == 0)
        {
            availableCards = new List<Card>();
            
            // Resources 폴더에서 카드 로드
            Card[] cards = Resources.LoadAll<Card>("Cards");
            if (cards != null && cards.Length > 0)
            {
                availableCards.AddRange(cards);
                Debug.Log($"로드된 카드 수: {availableCards.Count}");
            }
            else
            {
                Debug.LogWarning("Resources/Cards 폴더에서 카드를 찾을 수 없습니다.");
                // 카드가 로드되지 않으면 테스트용 카드 생성
                CreateTestCards();
            }
        }
    }

    // 테스트용 카드 생성 (Resources에 카드가 없을 때 사용)
    void CreateTestCards()
    {
        // 테스트용 카드들 생성 (실제로는 카드 에셋을 로드하는 것이 좋음)
        for (int i = 0; i < 4; i++)
        {
            Card razorCard = ScriptableObject.CreateInstance<Card>();
            razorCard.cardName = "면도날";
            razorCard.cardDescription = "적에게 출혈 3을 부여합니다.";
            razorCard.cardType = CardType.Attack;
            razorCard.damage = 2;
            razorCard.bleed = 3;
            availableCards.Add(razorCard);
        }

        for (int i = 0; i < 3; i++)
        {
            Card whetstoneCard = ScriptableObject.CreateInstance<Card>();
            whetstoneCard.cardName = "숫돌";
            whetstoneCard.cardDescription = "다음 카드의 출혈 효과가 50% 증가합니다.";
            whetstoneCard.cardType = CardType.Buff;
            availableCards.Add(whetstoneCard);
        }

        for (int i = 0; i < 3; i++)
        {
            Card biteCard = ScriptableObject.CreateInstance<Card>();
            biteCard.cardName = "깨물기";
            biteCard.cardDescription = "적의 현재 출혈 효과의 80%만큼 체력을 회복합니다.";
            biteCard.cardType = CardType.Special;
            biteCard.damage = 1;
            availableCards.Add(biteCard);
        }

        for (int i = 0; i < 2; i++)
        {
            Card explosionCard = ScriptableObject.CreateInstance<Card>();
            explosionCard.cardName = "피폭발";
            explosionCard.cardDescription = "적의 모든 출혈 효과를 한번에 터뜨려 2배의 데미지를 입힙니다.";
            explosionCard.cardType = CardType.Attack;
            availableCards.Add(explosionCard);
        }
    }

    void DisplayAvailableCards()
    {
        // 기존 카드 UI 모두 제거
        foreach (Transform child in cardDisplayParent)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log($"표시할 카드 수: {availableCards.Count}");
        
        foreach (Card card in availableCards)
        {
            GameObject cardUI = Instantiate(cardUIPrefab, cardDisplayParent);
            
            // EnhancedCardUI 컴포넌트 사용 (있는 경우)
            EnhancedCardUI enhancedCardUI = cardUI.GetComponent<EnhancedCardUI>();
            if (enhancedCardUI != null)
            {
                enhancedCardUI.SetCardData(card);
                Debug.Log($"EnhancedCardUI로 카드 표시: {card.cardName}");
            }
            else
            {
                // 기본 텍스트 업데이트 (EnhancedCardUI가 없는 경우)
                TMPro.TextMeshProUGUI cardNameText = cardUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                    Debug.Log($"TextMeshProUGUI로 카드 표시: {card.cardName}");
                }
                else
                {
                    Text legacyText = cardUI.GetComponentInChildren<Text>();
                    if (legacyText != null)
                    {
                        legacyText.text = card.cardName;
                        Debug.Log($"Legacy Text로 카드 표시: {card.cardName}");
                    }
                    else
                    {
                        Debug.LogError($"카드 '{card.cardName}'에 텍스트 컴포넌트를 찾을 수 없습니다.");
                    }
                }
            }
            
            Button button = cardUI.GetComponent<Button>();
            if (button != null)
            {
                Card capturedCard = card; // 클로저 문제 방지를 위해 카드 캡처
                button.onClick.AddListener(() => SelectCard(capturedCard));
                Debug.Log($"카드 '{card.cardName}'에 버튼 이벤트 추가됨");
            }
            else
            {
                Debug.LogError($"카드 '{card.cardName}'에 Button 컴포넌트가 없습니다.");
            }
        }
    }
    public void SelectCard(Card card)
    {
        if (selectedDeck.Count < maxDeckSize)
        {
            selectedDeck.Add(card);
            Debug.Log($"{card.cardName} added to deck. Current deck size: {selectedDeck.Count}");
            
            // 선택된 덱에 카드 UI 추가
            GameObject cardUI = Instantiate(cardUIPrefab, selectedDeckParent);
            
            // EnhancedCardUI 컴포넌트 사용 (있는 경우)
            EnhancedCardUI enhancedCardUI = cardUI.GetComponent<EnhancedCardUI>();
            if (enhancedCardUI != null)
            {
                enhancedCardUI.SetCardData(card);
                // 카드 순서 번호 표시
            }
            else
            {
                // 기본 텍스트 업데이트 (EnhancedCardUI가 없는 경우)
                Text cardNameText = cardUI.GetComponentInChildren<Text>();
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                }
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
            Debug.Log("Deck is full! Cannot add more cards.");
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
            Debug.Log($"{removedCard.cardName} removed from deck. Current deck size: {selectedDeck.Count}");
            
            // UI 갱신 - 모든 카드를 제거하고 다시 표시
            foreach (Transform child in selectedDeckParent)
            {
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < selectedDeck.Count; i++)
            {
                Card card = selectedDeck[i];
                GameObject cardUI = Instantiate(cardUIPrefab, selectedDeckParent);
                
                // EnhancedCardUI 컴포넌트 사용 (있는 경우)
                EnhancedCardUI enhancedCardUI = cardUI.GetComponent<EnhancedCardUI>();
                if (enhancedCardUI != null)
                {
                    enhancedCardUI.SetCardData(card);
                    // 카드 순서 번호 표시
                }
                else
                {
                    // 기본 텍스트 업데이트 (EnhancedCardUI가 없는 경우)
                    Text cardNameText = cardUI.GetComponentInChildren<Text>();
                    if (cardNameText != null)
                    {
                        cardNameText.text = card.cardName;
                    }
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
        if (DeckManager.Instance != null)
        {
            DeckManager.Instance.SetDeck(selectedDeck);
        }
        SaveLoadManager.SaveDeck(selectedDeck);
        Debug.Log("Deck confirmed and saved!");
    }

    public void StartGame()
    {
        ConfirmDeck(); // 게임 시작 전에 덱을 저장
        Debug.Log("Game Started with selected deck!");
        SceneManager.LoadScene("GameScene"); // 실제 게임 씬으로 이동
    }
}