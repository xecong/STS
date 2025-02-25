// GameManager.cs 파일 수정

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
        // ✅ SaveLoadManager에서 JSON 불러오기
        DeckData loadedData = SaveLoadManager.LoadDeck();
        if (loadedData != null)
        {
            CardManager.Instance.InitializeDeck(loadedData.cardNames);
        }

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
        // 먼저 카드를 드로우
        HandManager.Instance.DrawCards();
        
        // 핸드에 있는 카드를 자동으로 플레이하기 위해 HandManager에게 지시
        HandManager.Instance.PlayAllCardsInHand();
        
        // 카드 플레이가 끝날 때까지 기다리기
        StartCoroutine(WaitForHandToPlay());
    }

    private IEnumerator WaitForHandToPlay()
    {
        // HandManager의 애니메이션이 끝날 때까지 대기
        while (HandManager.Instance.IsAnimating())
        {
            yield return null;
        }
        
        // 모든 카드가 플레이되고 애니메이션이 끝났으면 턴 종료
        EndTurn();
    }

    void PlayEnemyTurn()
    {
        int enemyDamage = Random.Range(5, 15);
        playerHealth -= enemyDamage;
        Debug.Log($"🔥 Enemy attacks! Player takes {enemyDamage} damage. Remaining HP: {playerHealth}");

        EndTurn();
    }

    void EndTurn()
    {
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

        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn) turnNumber++;

        StartTurn();
    }
}