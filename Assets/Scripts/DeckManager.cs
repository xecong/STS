using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public List<Card> deck = new List<Card>();
    public List<Card> graveyard = new List<Card>();

    public TMP_Text deckCountText;
    public TMP_Text graveyardCountText;
    private GameObject deckUI; // ✅ 덱 이미지 자동 탐색
    private GameObject graveyardUI; // ✅ 묘지 이미지 자동 탐색

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AssignUIObjects();
        UpdateDeckUI();
        UpdateGraveyardUI();
        AddEventTriggers();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIObjects();
        UpdateDeckUI();
        UpdateGraveyardUI();
        AddEventTriggers();
    }

    private void AssignUIObjects()
    {
        if (deckCountText == null)
            deckCountText = GameObject.Find("DeckCountText")?.GetComponent<TMP_Text>();

        if (graveyardCountText == null)
            graveyardCountText = GameObject.Find("GraveyardCountText")?.GetComponent<TMP_Text>();

        if (deckUI == null)
            deckUI = GameObject.Find("DeckIMG"); // ✅ 덱 UI 자동 찾기

        if (graveyardUI == null)
            graveyardUI = GameObject.Find("GraveIMG"); // ✅ 묘지 UI 자동 찾기
    }

    private void AddEventTriggers()
    {
        if (deckUI != null)
        {
            AddEventTrigger(deckUI, EventTriggerType.PointerEnter, ShowDeckCount);
            AddEventTrigger(deckUI, EventTriggerType.PointerExit, HideDeckCount);
        }

        if (graveyardUI != null)
        {
            AddEventTrigger(graveyardUI, EventTriggerType.PointerEnter, ShowGraveyardCount);
            AddEventTrigger(graveyardUI, EventTriggerType.PointerExit, HideGraveyardCount);
        }
    }

    public void SetDeck(List<Card> newDeck)
    {
        deck = new List<Card>(newDeck);
        ShuffleDeck();
        UpdateDeckUI();
        Debug.Log("Deck saved and shuffled! Cards: " + string.Join(", ", deck));
    }

    public List<Card> GetDeck()
    {
        return new List<Card>(deck);
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        UpdateDeckUI();
        Debug.Log("Deck shuffled!");
    }

    public void DrawCardFromDeck()
    {
        if (deck.Count > 0)
        {
            Card drawnCard = deck[0];
            deck.RemoveAt(0); // ✅ 덱에서 카드 제거
            UpdateDeckUI(); // ✅ UI 업데이트
            HandManager.Instance.AddCardToHand(drawnCard);
            Debug.Log($"Drew card: {drawnCard.cardName}. Remaining deck size: {deck.Count}");
        }
        else
        {
            Debug.Log("Deck is empty, cannot draw more cards.");
        }
    }

    public void MoveGraveyardToDeck()
    {
        if (graveyard.Count > 0)
        {
            deck.AddRange(graveyard);
            graveyard.Clear();
            ShuffleDeck();
            UpdateDeckUI();
            UpdateGraveyardUI();
            Debug.Log("Graveyard shuffled back into deck!");
        }
    }

    public void AddToGraveyard(Card card)
    {
        graveyard.Add(card);
        UpdateGraveyardUI();
        Debug.Log($"{card.cardName} moved to graveyard.");
    }

    public void UpdateDeckUI()
    {
        if (deckCountText != null)
        {
            deckCountText.text = $"Deck: {deck.Count}";
            deckCountText.gameObject.SetActive(false);
        }
    }

    public void UpdateGraveyardUI()
    {
        if (graveyardCountText != null)
        {
            graveyardCountText.text = $"Graveyard: {graveyard.Count}";
            graveyardCountText.gameObject.SetActive(false);
        }
    }

    public void ShowDeckCount()
    {
        if (!HandManager.Instance.IsAnimating()) // ✅ 애니메이션 중이 아니면 표시
        {
            if (deckCountText != null)
                deckCountText.gameObject.SetActive(true);
        }
    }



    public void HideDeckCount()
    {
        if (deckCountText != null)
            deckCountText.gameObject.SetActive(false);
    }

    public void ShowGraveyardCount()
    {
        if (!HandManager.Instance.IsAnimating()) // ✅ 애니메이션 중이 아니면 표시
        {
            if (graveyardCountText != null)
                graveyardCountText.gameObject.SetActive(true);
        }
    }
    
    public void HideGraveyardCount()
    {
        if (graveyardCountText != null)
            graveyardCountText.gameObject.SetActive(false);
    }

    // ✅ 특정 UI 오브젝트에 EventTrigger 자동 추가
    private void AddEventTrigger(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((eventData) => action());

        trigger.triggers.Add(entry);
    }
}
