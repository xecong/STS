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
            playerHand = FindObjectOfType<HandManager>();
            if (playerHand == null)
            {
                Debug.LogError("❌ playerHand를 찾을 수 없습니다! HandManager가 씬에 있는지 확인하세요.");
            }
        }

        if (enemyHand == null)
        {
            enemyHand = FindObjectOfType<HandManager>();
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
        UpdateTurnUI(); // 🔹 UI 업데이트 추가


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

        if (DeckManager.Instance.deck.Count == 0)
        {
            Debug.Log("🔄 덱이 비어 있습니다. 묘지에서 덱을 다시 채웁니다.");
            DeckManager.Instance.RefillDeckFromGraveyard();
        }

        int drawAmount = Mathf.Min(5, DeckManager.Instance.deck.Count);
        if (drawAmount == 0)
        {
            Debug.LogError("❌ 덱이 비어 있어서 카드를 뽑을 수 없습니다!");
            return;
        }

        playerHand.DrawCards(DeckManager.Instance.DrawCards(drawAmount));
        StartCoroutine(PlayCards(playerHand, EndPlayerTurn));
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
            CardData card = hand.UseNextCard();
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
