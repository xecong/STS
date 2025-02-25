using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween 라이브러리 활용 (임포트 필요)

// 향상된 카드 UI 클래스
public class EnhancedCardUI : MonoBehaviour
{
    [Header("카드 정보")]
    public Image cardBackground;
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescriptionText;
    public Image cardFrame;
    
    [Header("애니메이션 설정")]
    public float quickPlayDuration = 0.5f;       // 빠른 카드 애니메이션 시간
    public float standardPlayDuration = 0.8f;    // 표준 카드 애니메이션 시간
    public float enhancedPlayDuration = 1.2f;    // 강화 카드 애니메이션 시간
    public float corePlayDuration = 2.0f;        // 핵심 카드 애니메이션 시간
    
    [Header("이펙트 프리팹")]
    public GameObject slashEffectPrefab;         // 칼날 효과
    public GameObject enhanceEffectPrefab;       // 강화 효과
    public GameObject explosionEffectPrefab;     // 폭발 효과
    public GameObject healEffectPrefab;          // 회복 효과
    
    // 카드 데이터 및 애니메이션 타입
    private Card cardData;
    private CardAnimationType animationType = CardAnimationType.Standard;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // CanvasGroup이 없으면 추가
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    // 카드 데이터 설정
    public void SetCardData(Card card)
    {
        cardData = card;
        UpdateCardVisuals();
        
        // 카드 타입에 따라 애니메이션 타입 결정
        DetermineAnimationType();
    }
    
    // 카드 시각 요소 업데이트
    private void UpdateCardVisuals()
    {
        if (cardData == null) return;
        
        // 카드 이름과 설명 업데이트
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.cardDescription;
        
        // 카드 이미지 업데이트
        if (cardData.cardImage != null)
        {
            cardImage.sprite = cardData.cardImage;
            cardImage.gameObject.SetActive(true);
        }
        else
        {
            cardImage.gameObject.SetActive(false);
        }
        
        // 카드 타입에 따른 배경 색상 변경
        switch (cardData.cardType)
        {
            case CardType.Attack:
                cardBackground.color = new Color(0.9f, 0.3f, 0.3f, 0.8f); // 빨강
                break;
            case CardType.Defense:
                cardBackground.color = new Color(0.3f, 0.7f, 0.9f, 0.8f); // 파랑
                break;
            case CardType.Buff:
                cardBackground.color = new Color(0.9f, 0.8f, 0.3f, 0.8f); // 노랑
                break;
            case CardType.Special:
                cardBackground.color = new Color(0.7f, 0.3f, 0.9f, 0.8f); // 보라
                break;
            default:
                cardBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // 회색
                break;
        }
    }
    
    // 카드 이름을 기반으로 애니메이션 타입 결정
    private void DetermineAnimationType()
    {
        if (cardData == null) return;
        
        // 카드 이름에 따라 애니메이션 타입 설정
        // 실제 게임에서는 카드 데이터에 애니메이션 타입을 직접 저장하는 것이 좋음
        string cardName = cardData.cardName.ToLower();
        
        if (cardName.Contains("면도날"))
        {
            animationType = CardAnimationType.Quick;
        }
        else if (cardName.Contains("숫돌"))
        {
            animationType = CardAnimationType.Enhanced;
        }
        else if (cardName.Contains("피폭발"))
        {
            animationType = CardAnimationType.Core;
        }
        else if (cardName.Contains("깨물기"))
        {
            animationType = CardAnimationType.Standard;
        }
        else
        {
            // 기본값은 표준 애니메이션
            animationType = CardAnimationType.Standard;
        }
        
        Debug.Log($"카드 '{cardData.cardName}'의 애니메이션 타입: {animationType}");
    }
    
    // 카드 사용 애니메이션 재생
    public void PlayUseAnimation()
    {
        // 애니메이션 타입에 따라 다른 애니메이션 실행
        switch (animationType)
        {
            case CardAnimationType.Quick:
                StartCoroutine(PlayQuickAnimation());
                break;
            case CardAnimationType.Standard:
                StartCoroutine(PlayStandardAnimation());
                break;
            case CardAnimationType.Enhanced:
                StartCoroutine(PlayEnhancedAnimation());
                break;
            case CardAnimationType.Core:
                StartCoroutine(PlayCoreAnimation());
                break;
        }
    }
    
    // 빠른 카드 애니메이션 (면도날 등 기본 카드)
    private IEnumerator PlayQuickAnimation()
    {
        Debug.Log($"🔪 빠른 애니메이션 시작: {cardData.cardName}");
        
        // 원래 위치와 회전 저장
        Vector3 originalPosition = rectTransform.position;
        Quaternion originalRotation = rectTransform.rotation;
        Vector3 originalScale = rectTransform.localScale;
        
        // 화면 중앙 위치 계산
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // 약간 위로 올라가는 효과
        rectTransform.DOMove(originalPosition + new Vector3(0, 20, 0), 0.2f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.2f);
        
        // 빠르게 중앙으로 날아가는 효과
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOMove(center, quickPlayDuration * 0.5f).SetEase(Ease.InQuad));
        sequence.Join(rectTransform.DOScale(1.2f, quickPlayDuration * 0.5f));
        sequence.Join(canvasGroup.DOFade(0.8f, quickPlayDuration * 0.5f));
        
        yield return sequence.WaitForCompletion();
        
        // 공격 효과 표시
        if (slashEffectPrefab != null && cardData.cardType == CardType.Attack)
        {
            GameObject slashEffect = Instantiate(slashEffectPrefab, canvas.transform);
            slashEffect.transform.position = center;
            Destroy(slashEffect, 0.5f);
        }
        
        // 페이드 아웃하며 사라짐
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        
        Destroy(gameObject);
    }
    
    // 표준 카드 애니메이션 (깨물기 등 일반 카드)
    private IEnumerator PlayStandardAnimation()
    {
        Debug.Log($"🎯 표준 애니메이션 시작: {cardData.cardName}");
        
        // 원래 위치와 회전 저장
        Vector3 originalPosition = rectTransform.position;
        Quaternion originalRotation = rectTransform.rotation;
        Vector3 originalScale = rectTransform.localScale;
        
        // 화면 중앙 위치 계산
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // 약간 위로 올라가는 효과
        rectTransform.DOMove(originalPosition + new Vector3(0, 30, 0), 0.3f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        
        // 중앙으로 이동하며 약간 회전
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOMove(center, standardPlayDuration * 0.6f).SetEase(Ease.InOutQuad));
        sequence.Join(rectTransform.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), standardPlayDuration * 0.6f));
        sequence.Join(rectTransform.DOScale(1.3f, standardPlayDuration * 0.6f));
        
        yield return sequence.WaitForCompletion();
        
        // 잠깐 대기
        yield return new WaitForSeconds(0.2f);
        
        // 카드 타입에 따른 효과 표시
        if (cardData.cardType == CardType.Special && healEffectPrefab != null)
        {
            GameObject healEffect = Instantiate(healEffectPrefab, canvas.transform);
            healEffect.transform.position = center;
            Destroy(healEffect, 0.7f);
        }
        
        // 페이드 아웃하며 사라짐
        canvasGroup.DOFade(0, 0.4f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.4f);
        
        Destroy(gameObject);
    }
    
    // 강화 카드 애니메이션 (숫돌 등 강화 카드)
    private IEnumerator PlayEnhancedAnimation()
    {
        Debug.Log($"✨ 강화 애니메이션 시작: {cardData.cardName}");
        
        // 원래 위치와 회전 저장
        Vector3 originalPosition = rectTransform.position;
        Vector3 originalScale = rectTransform.localScale;
        
        // 화면 중앙 위치 계산
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // 강조 효과 (약간 커지기)
        rectTransform.DOScale(originalScale * 1.2f, 0.4f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.4f);
        
        // 천천히 중앙으로 이동
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOMove(center, enhancedPlayDuration * 0.4f).SetEase(Ease.InOutCubic));
        sequence.Join(rectTransform.DOScale(1.4f, enhancedPlayDuration * 0.4f));
        
        yield return sequence.WaitForCompletion();
        
        // 중앙에서 빛나는 효과
        if (enhanceEffectPrefab != null)
        {
            GameObject enhanceEffect = Instantiate(enhanceEffectPrefab, canvas.transform);
            enhanceEffect.transform.position = center;
            
            // 강화 효과를 보여주기 위한 대기
            yield return new WaitForSeconds(0.8f);
            
            Destroy(enhanceEffect, 0.5f);
        }
        else
        {
            // 효과 프리팹이 없을 경우에도 대기
            yield return new WaitForSeconds(0.8f);
        }
        
        // 패널 숨기기 (천천히 사라짐)
        canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.5f);
        
        Destroy(gameObject);
    }
    
    // 핵심 카드 애니메이션 (피폭발 등 핵심 카드)
    private IEnumerator PlayCoreAnimation()
    {
        Debug.Log($"💥 핵심 애니메이션 시작: {cardData.cardName}");
        
        // 원래 위치와 회전 저장
        Vector3 originalPosition = rectTransform.position;
        Vector3 originalScale = rectTransform.localScale;
        
        // 화면 중앙 위치 계산
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // 첫 번째 단계: 카드 강조 효과
        Sequence emphasisSequence = DOTween.Sequence();
        emphasisSequence.Append(rectTransform.DOScale(originalScale * 1.3f, 0.5f).SetEase(Ease.OutBack));
        emphasisSequence.Join(rectTransform.DORotate(new Vector3(0, 0, -5f), 0.5f).SetEase(Ease.OutQuad));
        emphasisSequence.Join(cardBackground.DOColor(new Color(1f, 0.3f, 0.3f, 0.9f), 0.5f));
        
        yield return emphasisSequence.WaitForCompletion();
        
        // 약간의 대기 시간
        yield return new WaitForSeconds(0.5f);
        
        // 두 번째 단계: 중앙으로 천천히 이동
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(rectTransform.DOMove(center, 1.0f).SetEase(Ease.InOutCubic));
        moveSequence.Join(rectTransform.DOScale(1.6f, 1.0f));
        moveSequence.Join(rectTransform.DORotate(Vector3.zero, 1.0f));
        
        yield return moveSequence.WaitForCompletion();
        
        // 세 번째 단계: 화려한 이펙트와 함께 핵심 효과 발동
        if (explosionEffectPrefab != null)
        {
            // 잠시 정지 효과 (서스펜스)
            yield return new WaitForSeconds(0.3f);
            
            // 폭발 효과 생성
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, canvas.transform);
            explosionEffect.transform.position = center;
            
            // 화면 전체에 플래시 효과를 위한 패널
            GameObject flashPanel = new GameObject("FlashPanel");
            flashPanel.transform.SetParent(canvas.transform, false);
            RectTransform flashRect = flashPanel.AddComponent<RectTransform>();
            flashRect.anchorMin = Vector2.zero;
            flashRect.anchorMax = Vector2.one;
            flashRect.sizeDelta = Vector2.zero;
            
            Image flashImage = flashPanel.AddComponent<Image>();
            flashImage.color = new Color(1f, 0.3f, 0.3f, 0f);
            
            // 플래시 효과
            flashImage.DOFade(0.7f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() => {
                flashImage.DOFade(0f, 0.5f);
            });
            
            // 카드 축소 및 회전
            rectTransform.DOScale(0.8f, 0.3f);
            rectTransform.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), 0.3f);
            
            // 효과 지속 시간
            yield return new WaitForSeconds(1.2f);
            
            Destroy(explosionEffect, 0.5f);
            Destroy(flashPanel, 0.5f);
        }
        else
        {
            // 효과 프리팹이 없을 경우 대기
            yield return new WaitForSeconds(1.5f);
        }
        
        // 마지막 단계: 카드 사라짐
        canvasGroup.DOFade(0, 0.7f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.7f);
        
        Destroy(gameObject);
    }
}
