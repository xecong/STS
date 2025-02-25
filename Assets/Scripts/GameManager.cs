using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Card> playerDeck; // 전투에서 사용할 플레이어 덱
    public int playerHealth = 100;
    public int enemyHealth = 100;
    private int turnNumber = 1;
    private bool isPlayerTurn = true;

	void Start()
	{
		DeckData loadedData = SaveLoadManager.LoadDeck();
		if (loadedData != null)
		{
			DeckManager.Instance.SetDeckByNames(loadedData.cardNames);
		}
		StartTurn();
	}



    void LoadPlayerDeck()
    {
        playerDeck = DeckManager.Instance.GetDeck(); // 저장된 덱 불러오기
        Debug.Log("Game started with deck: " + string.Join(", ", playerDeck));
    }

    void StartBattle()
    {
        Debug.Log("Battle Started!");
        StartTurn();
    }

    void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log($"Turn {turnNumber}: Player's Turn");
            PlayPlayerTurn();
        }
        else
        {
            Debug.Log($"Turn {turnNumber}: Enemy's Turn");
            PlayEnemyTurn();
        }
    }

    void PlayPlayerTurn()
    {
        // 플레이어가 카드 사용 (자동으로 진행됨)
        foreach (Card card in playerDeck)
        {
            card.Use();
        }

        // 플레이어 턴 종료 후 적 턴으로 전환
        EndTurn();
    }

    void PlayEnemyTurn()
    {
        // 간단한 적 행동 (더 발전 가능)
        int enemyDamage = Random.Range(5, 15);
        playerHealth -= enemyDamage;
        Debug.Log($"Enemy attacks! Player takes {enemyDamage} damage. Remaining HP: {playerHealth}");

        // 적 턴 종료 후 플레이어 턴으로 전환
        EndTurn();
    }

    void EndTurn()
    {
        // 승리/패배 체크
        if (playerHealth <= 0)
        {
            Debug.Log("Game Over! Player is defeated.");
            return;
        }
        if (enemyHealth <= 0)
        {
            Debug.Log("Victory! Enemy is defeated.");
            return;
        }

        // 턴 전환
        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn) turnNumber++; // 적 턴이 끝나면 턴 번호 증가

        StartTurn();
    }
}