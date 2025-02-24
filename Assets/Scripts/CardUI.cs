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

    private static Queue<CardUI> cardQueue = new Queue<CardUI>(); // 사용될 카드 큐
    private static bool isProcessing = false; // 카드 사용 진행 여부
    private Card cardData;

    public void SetCardData(Card card)
    {
        cardData = card;
        
        if (cardNameText == null || cardDescriptionText == null || cardImage == null)
        {
            Debug.LogError("CardUI elements are not assigned properly in the prefab!");
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
            Debug.Log($"Card UI updated: {cardData.cardName}");
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
        Debug.Log($"Starting animation for {cardData.cardName}");
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;
        
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }

        Debug.Log($"Animation completed. Destroying {cardData.cardName}");
        Destroy(gameObject); // 애니메이션 후 삭제
    }
}
