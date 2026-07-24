using System.Collections;
using TMPro;
using UnityEngine;
public class CollectibleItem : MonoBehaviour
{
    public enum ItemType { Mushroom, Gem }

    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;

    [Header("References")]
    [SerializeField] private ItemCounterUI itemCounterUI;

    [Header("Message UI (shared across all pickups)")]
    [SerializeField] private CanvasGroup messageCanvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Messages")]
    [SerializeField] private string nearbyMessage = "Press E to collect.";
    [SerializeField] private string fullMessage = "Mushroom pouch full!";
    [SerializeField] private float fullMessageDisplayTime = 2f;

    private bool playerNearby;
    private bool collected;
    private Collider itemCollider;
    private Coroutine messageRoutine;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (!playerNearby || collected) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCollect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerNearby = true;
        ShowMessage(nearbyMessage, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerNearby = false;
        if (!collected)
        {
            HideMessage();
        }
    }

    private void TryCollect()
    {
        if (itemCounterUI == null) return;

        bool isFull = itemType == ItemType.Mushroom
            ? itemCounterUI.IsMushroomFull()
            : itemCounterUI.IsGemFull();

        if (isFull)
        {
            ShowMessage(fullMessage, fullMessageDisplayTime);
            return;
        }

        bool added = itemType == ItemType.Mushroom
            ? itemCounterUI.AddMushroom()
            : itemCounterUI.AddGem();

        if (!added) return;

        collected = true;
        playerNearby = false;
        HideMessage();

        if (itemCollider != null) itemCollider.enabled = false;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        Destroy(gameObject, fadeDuration + 0.05f);
    }

    private void ShowMessage(string message, float displayTime)
    {
        if (messageCanvasGroup == null || messageText == null) return;

        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(ShowMessageRoutine(message, displayTime));
    }

    private void HideMessage()
    {
        if (messageCanvasGroup == null) return;

        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator ShowMessageRoutine(string message, float displayTime)
    {
        messageText.text = message;
        yield return FadeTo(1f);
        if (displayTime > 0f)
        {
            yield return new WaitForSeconds(displayTime);
            yield return FadeTo(0f);
        }
        messageRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = messageCanvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        messageCanvasGroup.alpha = targetAlpha;
    }
}