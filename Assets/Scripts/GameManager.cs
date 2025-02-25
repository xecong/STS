using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int playerHealth = 100;
    public int enemyHealth = 100;
    private int turnNumber = 1;
    private bool isPlayerTurn = true;

    void Start()
    {
        // ✅ DeckManager에서 JSON 불러오기
        DeckData loadedData = SaveLoadManager.LoadDeck();
        if (loadedData != null)
        {
            DeckManager.Instance.SetDeckByNames(loadedData.cardNames);
        }

        // ✅ CardManager에 덱 초기화 요청
        CardManager.Instance.InitializeDeck(DeckManager.Instance.GetDeck());

        // ✅ 전투 시작
        StartBattle();
    }

    void StartBattle()
    {
        Debug.Log("⚔ Battle Started!");
        StartTurn();
    }

    void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log($"🃏 Turn {turnNumber}: Player's Turn");
            PlayPlayerTurn();
        }
        else
        {
            Debug.Log($"👹 Turn {turnNumber}: Enemy's Turn");
            PlayEnemyTurn();
        }
    }

    void PlayPlayerTurn()
    {
        HandManager.Instance.DrawCards();
        StartCoroutine(WaitForHandToPlay()); // ✅ 코루틴 실행 시 StartCoroutine() 사용!
    }

    private IEnumerator WaitForHandToPlay()
    {
        // ✅ 모든 카드가 사용될 때까지 대기
        while (HandManager.Instance.IsAnimating())
        {
            yield return null;
        }

        // ✅ 플레이어 턴 종료
        EndTurn();
    }

    void PlayEnemyTurn()
    {
        // ✅ 간단한 적 행동 (더 발전 가능)
        int enemyDamage = Random.Range(5, 15);
        playerHealth -= enemyDamage;
        Debug.Log($"🔥 Enemy attacks! Player takes {enemyDamage} damage. Remaining HP: {playerHealth}");

        // ✅ 적 턴 종료 후 플레이어 턴으로 전환
        EndTurn();
    }

    void EndTurn()
    {
        // ✅ 승리/패배 체크
        if (playerHealth <= 0)
        {
            Debug.Log("💀 Game Over! Player is defeated.");
            return;
        }
        if (enemyHealth <= 0)
        {
            Debug.Log("🏆 Victory! Enemy is defeated.");
            return;
        }

        // ✅ 턴 전환
        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn) turnNumber++; // 적 턴이 끝나면 턴 번호 증가

        StartTurn();
    }
}
