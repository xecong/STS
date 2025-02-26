using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardUI : MonoBehaviour
{
    public Image cardImage;  // 카드 이미지
    public TMP_Text cardNameText;  // 카드 이름 (TMP 적용)
    public TMP_Text cardDescriptionText;  // 카드 설명 (TMP 적용)
    public float moveUpDistance = 50f; // 위로 이동할 거리
    public float moveSpeed = 5f; // 이동 속도
    private bool isPlaying = false;

    // 사용되지 않는 정적 변수 제거 (사용되지 않는 큐 제거)
    private Card cardData;

    public void SetCardData(Card card)
    {
        cardData = card;
        
        if (cardNameText == null || cardDescriptionText == null || cardImage == null)
        {
            Debug.LogError("CardUI 요소들이 프리팹에 제대로 할당되지 않았어요! 확인해주세요~ 😭");
            return;
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cardData != null)
        {
            cardNameText.text = cardData.cardName;
            cardDescriptionText.text = cardData.cardDescription;
            cardImage.sprite = cardData.cardImage;
            Debug.Log($"카드 UI 업데이트: {cardData.cardName}");
        }
    }

    public void PlayUseAnimation()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            StartCoroutine(MoveUpAndDestroy());
        }
    }

    private IEnumerator MoveUpAndDestroy()
    {
        Debug.Log($"{cardData.cardName} 카드 애니메이션 시작");
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;
        
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        Debug.Log($"애니메이션 완료. {cardData.cardName} 카드 UI 제거");
        Destroy(gameObject); // 애니메이션 후 삭제
    }

    void OnApplicationQuit()
    {
        // SaveLoadManager.SaveDeck 메서드가 여러 버전으로 오버로드되어 있는지 확인하고 적절한 것 사용
        if (CardManager.Instance != null)
        {
            SaveLoadManager.SaveDeck(CardManager.Instance.GetCurrentDeckNames());
            Debug.Log("게임 종료 시 덱 저장 완료");
        }
    }
}