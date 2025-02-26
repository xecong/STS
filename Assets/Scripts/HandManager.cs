using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class HandManager : MonoBehaviour
{
    public List<Card> hand = new List<Card>(); // 현재 핸드에 있는 카드 리스트
    public Transform handUI; // 핸드 UI 패널 (Horizontal Layout Group 사용)
    public GameObject cardUIPrefab; // 카드 UI 프리팹
    public int maxHandSize = 5; // 최대 핸드 크기
    public static HandManager Instance;

    private bool isAnimating = false; // 애니메이션 진행 여부 확인 변수
    private CardSequenceHandler sequenceHandler;
	
    /// <summary>
    /// 애니메이션 상태 확인 메서드
    /// </summary>
	public bool IsAnimating()
	{
		return isAnimating;
	}
    
    /// <summary>
    /// 애니메이션 상태를 false로 설정하는 메서드
    /// </summary>
    public void SetAnimatingFalse()
    {
        isAnimating = false;
        Debug.Log("카드 애니메이션 상태를 종료로 설정했어요~");
    }

    /// <summary>
    /// 핸드의 모든 카드를 비우는 메서드
    /// </summary>
    public void ClearHand()
    {
        // 핸드에 있는 모든 카드 UI 오브젝트 제거
        foreach (Transform child in handUI)
        {
            Destroy(child.gameObject);
        }
        
        // 핸드 리스트 비우기 (묘지로 보내지 않고 그냥 비움)
        hand.Clear();
        Debug.Log("핸드를 비웠습니다.");
    }

    /// <summary>
    /// 덱에서 카드 한 장 드로우해서 핸드에 추가
    /// </summary>
    public void DrawCard(Card card)
    {
        if (hand.Count < maxHandSize)
        {
            AddCardToHand(card);
        }
        else
        {
            Debug.Log("핸드가 가득 찼습니다. 더 이상 카드를 뽑을 수 없어요~");
        }
    }

    /// <summary>
    /// 카드를 핸드에 추가하고 UI 생성
    /// </summary>
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        GameObject cardUI = Instantiate(cardUIPrefab, handUI);

        CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
        if (cardUIComponent != null)
        {
            cardUIComponent.SetCardData(card);
            Debug.Log($"카드 UI 업데이트 완료: {card.cardName}");
        }
        else
        {
            Debug.LogError("카드 UI 컴포넌트를 찾을 수 없어요~ 프리팹을 확인해주세요!");
        }

        Debug.Log($"핸드에 카드 추가: {card.cardName}. 현재 핸드 크기: {hand.Count}");
    }

	// 이미 있는 DrawCards 메서드가 public으로 선언되어 있는지 확인
	public void DrawCards()
	{
		// 기존 핸드를 먼저 비우기
		ClearHand();
		
		Debug.Log($"덱 드로우 시작. 현재 덱 크기: {CardManager.Instance.GetDeckSize()}, 묘지 크기: {CardManager.Instance.GetGraveyardSize()}");

		// 턴 시작 시 덱이 완전히 비어있는 경우에만 묘지에서 카드 가져오기
		if (CardManager.Instance.GetDeckSize() == 0)
		{
			// 묘지에 카드가 있으면 덱으로 가져오기
			if (CardManager.Instance.GetGraveyardSize() > 0)
			{
				Debug.Log("턴 시작 시 덱이 비었습니다. 묘지의 카드를 덱으로 가져옵니다.");
				CardManager.Instance.RecycleGraveyard();
				Debug.Log($"묘지에서 덱으로 이동 후 덱 크기: {CardManager.Instance.GetDeckSize()}");
			}
			else
			{
				Debug.Log("덱과 묘지 모두 비었습니다. 더 이상 드로우할 수 없습니다.");
				return; // 드로우할 카드가 없으므로 함수 종료
			}
		}

		// 새로운 핸드 채우기 - 최대 5장 또는 덱의 남은 카드 수 중 작은 값
		int cardsToDrawCount = Mathf.Min(maxHandSize, CardManager.Instance.GetDeckSize());
		Debug.Log($"이번 턴에 {cardsToDrawCount}장의 카드를 뽑습니다.");

		for (int i = 0; i < cardsToDrawCount; i++)
		{
			// 카드 드로우
			Card drawnCard = CardManager.Instance.DrawCard();
			if (drawnCard != null)
			{
				DrawCard(drawnCard);
				Debug.Log($"카드 드로우 #{i+1}: {drawnCard.cardName}. 남은 덱 크기: {CardManager.Instance.GetDeckSize()}");
			}
		}
		
		Debug.Log($"드로우 완료. 현재 핸드 크기: {hand.Count}, 덱 크기: {CardManager.Instance.GetDeckSize()}, 묘지 크기: {CardManager.Instance.GetGraveyardSize()}");
	}
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
    
    // 시퀀스 핸들러 초기화
    private void InitializeSequenceHandler()
    {
        sequenceHandler = GetComponent<CardSequenceHandler>();
        if (sequenceHandler == null)
        {
            sequenceHandler = gameObject.AddComponent<CardSequenceHandler>();
        }
    }

    private void Start()
    {
        Debug.Log("HandManager started. Ready to draw cards.");
        // 게임 씬 시작 시 필요한 초기화 수행
        InitializeSequenceHandler();
    }

    /// <summary>
    /// 핸드의 모든 카드를 플레이하는 메소드 - GameManager에서 호출됨
    /// </summary>
    public void PlayAllCardsInHand()
    {
        if (sequenceHandler == null)
        {
            InitializeSequenceHandler();
        }
        
        if (hand.Count > 0)
        {
            Debug.Log($"핸드의 모든 카드({hand.Count}장) 플레이 시작");
            StartCoroutine(PlayCardsSequentially());
        }
        else
        {
            Debug.Log("플레이할 카드가 없습니다.");
            isAnimating = false;
        }
    }
	private IEnumerator PlayCardsSequentially()
	{
		Debug.Log("카드 순차 플레이 시작");
		isAnimating = true;

		// 카드를 하나씩 플레이
		while (hand.Count > 0)
		{
			Card card = hand[0];
			Debug.Log($"카드 사용: {card.cardName}");

			// 카드 효과 적용
			card.Use();

			// 카드 UI 애니메이션 재생
			if (handUI.childCount > 0)
			{
				CardUI cardUI = handUI.GetChild(0).GetComponent<CardUI>();
				if (cardUI != null)
				{
					cardUI.PlayUseAnimation();
					yield return new WaitForSeconds(0.5f); // 애니메이션 대기 시간
				}
			}

			// 카드를 핸드에서 제거하고 묘지로 이동
			hand.RemoveAt(0);
			CardManager.Instance.MoveToGraveyard(card);
			Debug.Log($"카드 {card.cardName}을(를) 묘지로 이동. 현재 묘지 크기: {CardManager.Instance.GetGraveyardSize()}");
		}

		isAnimating = false;
		Debug.Log("모든 카드 플레이 완료");
	}

    /// <summary>
    /// 게임 종료 시 덱 저장
    /// </summary>
    void OnApplicationQuit()
    {
        if (CardManager.Instance != null)
        {
            SaveLoadManager.SaveDeck(CardManager.Instance.GetCurrentDeckNames());
            Debug.Log("게임 종료 시 덱 저장 완료");
        }
    }
}