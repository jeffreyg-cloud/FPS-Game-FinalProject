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

    private float fullWidth;
    private float targetWidth;

    public float CurrentMana => currentMana;
    public float MaxMana => maxMana;
    public bool IsFull => currentMana >= maxMana;

    private void Start()
    {
        maxMana = Mathf.Max(1f, maxMana);

        currentMana = Mathf.Clamp(
            currentMana,
            0f,
            maxMana
        );

        if (manaFill != null)
        {
            fullWidth = manaFill.sizeDelta.x;
        }
        else
        {
            Debug.LogWarning(
                "PlayerManaUI: Mana Fill has not been assigned."
            );
        }

        UpdateManaUI(true);
    }

    private void Update()
    {
        RegenerateMana();
        AnimateManaBar();
    }

    private void RegenerateMana()
    {
        if (currentMana >= maxMana)
        {
            return;
        }

        if (manaRegenPerSecond <= 0f)
        {
            return;
        }

        currentMana +=
            manaRegenPerSecond * Time.deltaTime;

        currentMana = Mathf.Clamp(
            currentMana,
            0f,
            maxMana
        );

        UpdateManaUI(false);
    }

    private void AnimateManaBar()
    {
        if (manaFill == null)
        {
            return;
        }

        float currentWidth =
            manaFill.sizeDelta.x;

        float newWidth =
            Mathf.MoveTowards(
                currentWidth,
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
        {
            return true;
        }

        if (currentMana < amount)
        {
            return false;
        }

        currentMana -= amount;

        currentMana = Mathf.Clamp(
            currentMana,
            0f,
            maxMana
        );

        UpdateManaUI(false);

        return true;
    }

    public void RecoverMana(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentMana += amount;

        currentMana = Mathf.Clamp(
            currentMana,
            0f,
            maxMana
        );

        UpdateManaUI(false);
    }

    public void SetMana(
        float current,
        float maximum
    )
    {
        maxMana = Mathf.Max(
            1f,
            maximum
        );

        currentMana = Mathf.Clamp(
            current,
            0f,
            maxMana
        );

        UpdateManaUI(true);
    }

    private void UpdateManaUI(bool updateImmediately)
    {
        float manaPercent =
            Mathf.Clamp01(
                currentMana / maxMana
            );

        targetWidth =
            fullWidth * manaPercent;

        if (updateImmediately &&
            manaFill != null)
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
        {
            return;
        }

        manaText.text =
            Mathf.CeilToInt(currentMana)
            + " / "
            + Mathf.CeilToInt(maxMana);
    }
}