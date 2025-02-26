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
		// JSON에서 덱 불러오기
		DeckData loadedData = SaveLoadManager.LoadDeck();
		if (loadedData != null && loadedData.cardNames != null && loadedData.cardNames.Count > 0)
		{
			Debug.Log($"JSON에서 덱 로드 성공. 카드 수: {loadedData.cardNames.Count}");
			// 새로운 CardManager에 덱 초기화
			if (CardManager.Instance != null)
			{
				CardManager.Instance.InitializeDeck(loadedData.cardNames);
				Debug.Log("CardManager에 덱 초기화 완료");
			}
		}
		else
		{
			Debug.LogError("덱 데이터를 불러오지 못했습니다.");
		}

		// HandManager 인스턴스 확인
		if (HandManager.Instance == null)
		{
			Debug.LogError("HandManager 인스턴스를 찾을 수 없습니다!");
		}
		else
		{
			Debug.Log("HandManager 인스턴스 찾음");
		}

		// 게임 시작
		StartBattle();
	}

	// WaitForHandToPlay 코루틴이 HandManager.IsAnimating을 참조하는 경우 수정
	private IEnumerator WaitForHandToPlay()
	{
		if (HandManager.Instance != null)
		{
			while (HandManager.Instance.IsAnimating())
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(1f); // HandManager가 없는 경우 기본 대기 시간
		}
		
		EndTurn();
	}

// PlayerTurn 메서드에서 DrawCards 호출 부분 수정
void PlayPlayerTurn()
{
    if (HandManager.Instance != null)
    {
        HandManager.Instance.DrawCards();
        StartCoroutine(WaitForHandToPlay());
    }
    else
    {
        Debug.LogError("HandManager 인스턴스를 찾을 수 없어 카드를 드로우할 수 없습니다!");
        EndTurn(); // 그냥 턴 종료
    }
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