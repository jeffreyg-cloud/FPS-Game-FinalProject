using TMPro;
using UnityEngine;

public class PlayerManaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform manaFill;
    [SerializeField] private TMP_Text manaText;

    [Header("Mana Settings")]
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float currentMana = 100f;

    [Header("Mana Regeneration")]
    [SerializeField] private float manaRegenPerSecond = 2f;

    [Header("UI Animation")]
    [SerializeField] private float changeSpeed = 500f;

    [SerializeField] private ManaTutorial manaTutorial;


    private float fullWidth;
    private float targetWidth;

    public float CurrentMana => currentMana;
    public float MaxMana => maxMana;
    public bool IsFull => currentMana >= maxMana;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            maxMana = Mathf.Max(1f, GameManager.Instance.maxMana);
            currentMana = Mathf.Clamp(GameManager.Instance.currentMana, 0f, maxMana);
        }
        else
        {
            maxMana = Mathf.Max(1f, maxMana);
            currentMana = Mathf.Clamp(currentMana, 0f, maxMana);
        }

        if (manaFill != null)
            fullWidth = manaFill.sizeDelta.x;
        else
            Debug.LogWarning("PlayerManaUI: Mana Fill has not been assigned.");

        UpdateManaUI(true);
    }

    private void Update()
    {
        RegenerateMana();
        AnimateManaBar();
    }

    private void RegenerateMana()
    {
        if (currentMana >= maxMana || manaRegenPerSecond <= 0f)
            return;

        currentMana = Mathf.Clamp(
            currentMana + manaRegenPerSecond * Time.deltaTime,
            0f,
            maxMana
        );

        SaveToGameManager();
        UpdateManaUI(false);
    }

    private void AnimateManaBar()
    {
        if (manaFill == null)
            return;

        float newWidth = Mathf.MoveTowards(
            manaFill.sizeDelta.x,
            targetWidth,
            changeSpeed * Time.deltaTime
        );

        manaFill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            newWidth
        );
    }

    public bool UseMana(float amount)
    {
        if (amount <= 0f)
            return true;

        if (currentMana < amount)
            return false;

        currentMana = Mathf.Clamp(currentMana - amount, 0f, maxMana);

        SaveToGameManager();
        UpdateManaUI(false);

        // Show mana tutorial
        if (manaTutorial != null)
            manaTutorial.ShowManaTutorial();

        return true;
    }

    public void RecoverMana(float amount)
    {
        if (amount <= 0f)
            return;

        currentMana = Mathf.Clamp(currentMana + amount, 0f, maxMana);

        SaveToGameManager();
        UpdateManaUI(false);
    }

    // Called when the player respawns
    public void RestoreFullMana()
    {
        currentMana = maxMana;
        SaveToGameManager();
        UpdateManaUI(true);
    }

    public void SetMana(float current, float maximum)
    {
        maxMana = Mathf.Max(1f, maximum);
        currentMana = Mathf.Clamp(current, 0f, maxMana);

        SaveToGameManager();
        UpdateManaUI(true);
    }

    private void SaveToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentMana = currentMana;
            GameManager.Instance.maxMana = maxMana;
        }
    }

    private void UpdateManaUI(bool updateImmediately)
    {
        targetWidth = fullWidth * Mathf.Clamp01(currentMana / maxMana);

        if (updateImmediately && manaFill != null)
        {
            manaFill.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                targetWidth
            );
        }

        UpdateManaText();
    }

    private void UpdateManaText()
    {
        if (manaText == null)
            return;

        manaText.text =
            Mathf.CeilToInt(currentMana) +
            " / " +
            Mathf.CeilToInt(maxMana);
    }
}