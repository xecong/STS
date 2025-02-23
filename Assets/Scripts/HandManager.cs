using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandManager : MonoBehaviour
{
    public List<CardData> handCards = new List<CardData>(); // 현재 핸드
    public List<CardData> graveyard = new List<CardData>(); // 사용된 카드
    public GameObject cardPrefab;
    public Transform cardPanel;

    public bool HasCards()
    {
        return handCards.Count > 0;
    }

    public void DrawCards(List<CardData> newCards)
    {
        handCards.AddRange(newCards);
        UpdateHandUI(); // UI 갱신 추가
        Debug.Log($"🃏 {newCards.Count}장 핸드에 추가됨!");
    }

    public IEnumerator PlayHand()
    {
        while (handCards.Count > 0)
        {
            GameObject cardObject = GetFirstCardObject();
            CardData nextCard = UseNextCard(cardObject);

            if (nextCard != null)
            {
                Debug.Log($"✨ {nextCard.cardName} 사용됨!");
                yield return new WaitForSeconds(1f); // 카드 사용 간격
            }
            else
            {
                Debug.LogWarning("⚠️ 더 이상 사용할 카드가 없습니다.");
                break; // 카드가 없으면 루프 탈출
            }
        }
        Debug.Log("✅ 모든 카드 사용 완료!");
        GameManager.Instance.EndPlayerTurn(); // 턴 종료 확실히 실행
    }

    public CardData UseNextCard(GameObject cardObject)
    {
        if (handCards.Count == 0)
        {
            Debug.LogWarning("⚠️ 핸드에 카드가 없습니다!");
            return null;
        }

        CardData nextCard = handCards[0];
        handCards.RemoveAt(0);
        DeckManager.Instance.graveyard.Add(nextCard); // 묘지에 카드 추가

        if (cardObject != null)
        {
            Destroy(cardObject); // UI 카드 오브젝트 제거
        }

        return nextCard;
    }

    public GameObject GetFirstCardObject()
    {
        if (cardPanel.childCount > 0)
        {
            return cardPanel.GetChild(0).gameObject; // 첫 번째 카드 반환
        }
        return null;
    }

    private void UpdateHandUI()
    {
        foreach (Transform child in cardPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (CardData card in handCards)
        {
            GameObject newCard = Instantiate(cardPrefab, cardPanel);
            TMP_Text nameText = newCard.transform.Find("CardName").GetComponent<TMP_Text>();
            TMP_Text descText = newCard.transform.Find("CardDescription").GetComponent<TMP_Text>();
            Image cardImage = newCard.transform.Find("CardImage").GetComponent<Image>();

            if (nameText != null) nameText.text = card.cardName;
            if (descText != null) descText.text = card.CardDescription;
            if (cardImage != null) cardImage.sprite = card.cardImage;
        }
    }
}
