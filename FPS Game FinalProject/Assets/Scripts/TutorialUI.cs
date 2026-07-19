using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Display Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float defaultDisplayTime = 3f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void Start()
    {
        // TODO：正式整合地图后删除，改为由 Tutorial Manager 调用
        ShowMessage("Welcome, young Mage.");
    }
    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDisplayTime);
    }

    public void ShowMessage(string message, float displayTime)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(
            ShowMessageRoutine(message, displayTime)
        );
    }

    public void HideMessage()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator ShowMessageRoutine(
        string message,
        float displayTime
    )
    {
        tutorialText.text = message;

        yield return FadeTo(1f);

        if (displayTime > 0f)
        {
            yield return new WaitForSeconds(displayTime);
            yield return FadeTo(0f);
        }

        currentRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private void Update()
    {
        // TODO：仅用于 UITestScene 测试，正式版本删除

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowMessage("Welcome, young Mage.");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShowMessage(
                "Move with WASD.\n" +
                "Look around with the mouse.",
                5f
            );
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            ShowMessage(
                "Hold Shift to sprint.\n" +
                "Press Space to jump.",
                5f
            );
        }
    }
}
