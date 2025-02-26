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
		StartBattle();
	}

    // GameManager.cs의 WaitForHandToPlay 코루틴 수정
    private IEnumerator WaitForHandToPlay()
    {
        Debug.Log("플레이어 턴: 카드 사용 대기 중...");
        
        // 카드를 드로우하고 잠시 대기 (UI 업데이트 시간 확보)
        yield return new WaitForSeconds(1f);
        
        // 중요! 핸드의 모든 카드 플레이 호출 추가
        if (HandManager.Instance != null)
        {
            Debug.Log("핸드의 모든 카드 플레이 시작");
            HandManager.Instance.PlayAllCardsInHand();
            
            // 카드 애니메이션이 완료될 때까지 대기
            while (HandManager.Instance.IsAnimating())
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("HandManager 인스턴스를 찾을 수 없어! 카드를 플레이할 수 없어...");
            yield return new WaitForSeconds(1f); // 기본 대기
        }
        
        Debug.Log("플레이어의 턴 종료. 다음 턴으로 이동!");
        EndTurn();
    }

    // PlayerTurn 메서드 수정
    void PlayPlayerTurn()
    {
        if (HandManager.Instance != null)
        {
            // 카드 드로우
            Debug.Log("플레이어 턴: 카드 드로우 시작!");
            HandManager.Instance.DrawCards();
            
            // 카드 사용 대기 코루틴 시작
            StartCoroutine(WaitForHandToPlay());
        }
        else
        {
            Debug.LogError("HandManager 인스턴스를 찾을 수 없어! 카드를 드로우할 수 없어...");
            EndTurn(); // 바로 턴 종료
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

    // GameManager.cs의 EndTurn 메서드 수정
    void EndTurn()
    {
        // 덱과 묘지 상태를 로그로 확인
        if (CardManager.Instance != null)
        {
            Debug.Log($"턴 종료 시 덱 상태 - 덱 크기: {CardManager.Instance.GetDeckSize()}, 묘지 크기: {CardManager.Instance.GetGraveyardSize()}");
        }
        
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