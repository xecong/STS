using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI turnIndicator;

    private enum TurnState { PlayerTurn, EnemyTurn }
    private TurnState currentTurn;

    public HandManager playerHand;
    public HandManager enemyHand;

    public PlayerHealth playerHealth;
    public EnemyHealth enemyHealth;
	public HandManager handManager;

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

        // 🔹 HandManager 자동 할당 추가
        if (playerHand == null)
        {
            playerHand = FindFirstObjectByType<HandManager>();
            if (playerHand == null)
            {
                Debug.LogError("❌ playerHand를 찾을 수 없습니다! HandManager가 씬에 있는지 확인하세요.");
            }
        }

        if (enemyHand == null)
        {
            enemyHand = FindFirstObjectByType<HandManager>();
            if (enemyHand == null)
            {
                Debug.LogError("❌ enemyHand를 찾을 수 없습니다! HandManager가 씬에 있는지 확인하세요.");
            }
        }
    }


    private void Start()
    {
        UpdateTurnUI(); // 🔹 처음 실행될 때 UI 업데이트
        Debug.Log("게임 시작 대기 중...");
    }
    private void UpdateTurnUI()
    {
        if (turnIndicator != null)
        {
            turnIndicator.text = (currentTurn == TurnState.PlayerTurn) ? "🔵 플레이어 턴" : "🔴 적 턴";
        }
        else
        {
            Debug.LogError("❌ TurnIndicator가 할당되지 않았습니다! UI에서 연결하세요.");
        }
    }

    public void StartGame()
    {
        Debug.Log("🎮 게임 시작!");
        StartPlayerTurn();
    }


	private void StartPlayerTurn()
	{
		currentTurn = TurnState.PlayerTurn;
		Debug.Log("▶ 플레이어 턴 시작!");
		UpdateTurnUI();

		if (playerHand == null)
		{
			Debug.LogError("❌ playerHand가 null입니다! HandManager가 씬에 있는지 확인하세요.");
			return;
		}

		if (DeckManager.Instance == null)
		{
			Debug.LogError("❌ DeckManager.Instance가 null입니다! DeckManager가 씬에 있는지 확인하세요.");
			return;
		}

		int drawAmount = 5; // 기본적으로 뽑을 카드 개수
		List<CardData> drawnCards = new List<CardData>();
		
		if (drawnCards.Count > 0)
		{
			playerHand.DrawCards(drawnCards);
			StartCoroutine(playerHand.PlayHand()); // 핸드에 카드가 있는 경우만 실행
		}
		else
		{
			Debug.LogWarning("⚠️ 뽑을 카드가 없어서 턴이 진행되지 않습니다.");
			EndPlayerTurn();
		}

		// 🔥 현재 덱에서 뽑을 수 있는 만큼 뽑기
		int availableCards = DeckManager.Instance.deck.Count;
		if (availableCards > 0)
		{
			int drawNow = Mathf.Min(drawAmount, availableCards);
			drawnCards.AddRange(DeckManager.Instance.DrawCards(drawNow));
			drawAmount -= drawNow;
		}

		// 🔄 덱이 부족하면 묘지를 셔플하고 다시 보충한 후 남은 카드 뽑기
		if (drawAmount > 0 && DeckManager.Instance.graveyard.Count > 0)
		{
			Debug.Log("🔄 덱이 부족! 묘지를 셔플하여 덱을 다시 생성합니다.");
			DeckManager.Instance.RefillDeckFromGraveyard();

			// 🔥 다시 덱에서 남은 카드 뽑기
			int drawNow = Mathf.Min(drawAmount, DeckManager.Instance.deck.Count);
			drawnCards.AddRange(DeckManager.Instance.DrawCards(drawNow));
		}

		// 🔥 실제로 뽑은 카드가 있으면 핸드에 추가
		if (drawnCards.Count > 0)
		{
			playerHand.DrawCards(drawnCards);
			StartCoroutine(PlayCards(playerHand, EndPlayerTurn));
		}
		else
		{
			Debug.LogWarning("⚠️ 뽑을 카드가 없어서 턴이 진행되지 않습니다.");
			EndPlayerTurn();
		}
	}



    public void EndPlayerTurn()
    {
        Debug.Log("⏩ 플레이어 턴 종료");
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        Debug.Log("💀 적 턴 시작!");
        UpdateTurnUI(); // 🔹 UI 업데이트 추가
        new WaitForSeconds(1f);
        StartPlayerTurn();//테스트용으로 바로 플레이어 턴 시작하게 만듬
        //StartCoroutine(PlayCards(enemyHand, EndEnemyTurn));
    }

    private void EndEnemyTurn()
    {
        Debug.Log("⏩ 적 턴 종료");
        StartPlayerTurn();
    }

	private IEnumerator PlayCards(HandManager hand, System.Action onTurnEnd)
	{
		while (hand.HasCards())
		{
			GameObject cardObject = hand.GetFirstCardObject(); // 🔥 UI 오브젝트 가져오기
			CardData card = hand.UseNextCard(cardObject); // 🔥 카드 사용 시 UI 오브젝트 전달

			if (card != null)
			{
				Debug.Log($"🃏 {card.cardName} 사용됨!");
				yield return new WaitForSeconds(0.5f);
			}
		}
		onTurnEnd.Invoke();
	}

    public void DealDamageToEnemy(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log("⚔️ 적에게 " + damage + " 데미지!");
        }
    }

    public void DealDamageToPlayer(int damage)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("⚔️ 플레이어가 " + damage + " 데미지를 받음!");
        }
    }

}
