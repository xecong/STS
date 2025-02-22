using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private enum TurnState { PlayerTurn, EnemyTurn } // 플레이어 턴과 적 턴 구분
    private TurnState currentTurn; // 현재 턴 상태

    public HandManager playerHand; // 플레이어 핸드 (카드를 관리하는 클래스)
    public HandManager enemyHand; // NPC 핸드

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn(); // 게임 시작 시 플레이어 턴부터 시작
    }

	private void StartPlayerTurn()
	{
		currentTurn = TurnState.PlayerTurn;
		Debug.Log("▶ 플레이어 턴 시작!");

		// DeckManager.Instance로 직접 접근하도록 수정
		if (DeckManager.Instance.deck.Count == 0)
		{
			DeckManager.Instance.RefillDeckFromGraveyard();
		}

		int drawAmount = Mathf.Min(5, DeckManager.Instance.deck.Count);
		playerHand.DrawCards(DeckManager.Instance.DrawCards(drawAmount));

		StartCoroutine(PlayCards(playerHand, EndPlayerTurn));
	}

    public void EndPlayerTurn()
    {
        Debug.Log("⏩ 플레이어 턴 종료");
        StartPlayerTurn(); // 적 턴으로 전환 *(테스트로 다시 내턴실행되게해놓음)
    }

	private IEnumerator StartNextTurn() //테스트용
	{
		yield return new WaitForSeconds(1f); // 턴 전환 간격
		StartPlayerTurn(); // 다시 플레이어 턴 시작
	}
    private void StartEnemyTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        Debug.Log("💀 적 턴 시작!");
		Debug.Log($"💀 현재 턴: {currentTurn}");
        StartCoroutine(PlayCards(enemyHand, EndEnemyTurn)); // 적 카드 실행
    }

    private void EndEnemyTurn()
    {
        Debug.Log("⏩ 적 턴 종료");
        StartPlayerTurn(); // 다시 플레이어 턴으로 전환
    }

	private IEnumerator PlayCards(HandManager hand, System.Action onTurnEnd)
	{
		while (hand.HasCards())
		{
			CardData card = hand.UseNextCard(); // `CardData`로 변경
			if (card != null)
			{
				Debug.Log($"🃏 {card.cardName} 사용됨!"); // `PlayEffect()` 대신 로그 출력
				yield return new WaitForSeconds(0.5f);
			}
		}
		onTurnEnd.Invoke();
	}

}
