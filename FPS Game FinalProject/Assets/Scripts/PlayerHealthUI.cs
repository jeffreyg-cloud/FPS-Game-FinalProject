using TMPro;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Animation")]
    [SerializeField] private float changeSpeed = 500f;

    private float fullWidth;
    private float targetWidth;

    private void Start()
    {
        fullWidth = healthFill.sizeDelta.x;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateTargetWidth();
        UpdateHealthText();
    }

    private void Update()
    {
        float currentWidth = healthFill.sizeDelta.x;

        float newWidth = Mathf.MoveTowards(
            currentWidth,
            targetWidth,
            changeSpeed * Time.deltaTime
        );

        healthFill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            newWidth
        );

        // 下面两段只用于测试，合并的时候删除以下两段
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(30f);
        } 
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateTargetWidth();
        UpdateHealthText();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateTargetWidth();
        UpdateHealthText();
    }

    public void SetHealth(float current, float maximum)
    {
        maxHealth = Mathf.Max(1f, maximum);

        currentHealth = Mathf.Clamp(
            current,
            0f,
            maxHealth
        );

        UpdateTargetWidth();
        UpdateHealthText();
    }

    private void UpdateTargetWidth()
    {
        float healthPercent = currentHealth / maxHealth;
        targetWidth = fullWidth * healthPercent;
    }

    private void UpdateHealthText()
    {
        healthText.text =
            Mathf.CeilToInt(currentHealth)
            + " / "
            + Mathf.CeilToInt(maxHealth);
    }
}