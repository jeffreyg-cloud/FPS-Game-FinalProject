using TMPro;
using UnityEngine;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Animation")]
    [SerializeField] private float changeSpeed = 5f;

    private float targetScale;

    private void Start()
    {
        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        targetScale = currentHealth / maxHealth;

        healthFill.localScale = new Vector3(
            targetScale,
            1f,
            1f
        );

        UpdateHealthText();
    }

    private void Update()
    {
        // Smoothly animate the health bar
        Vector3 currentScale = healthFill.localScale;

        float newScaleX = Mathf.MoveTowards(
            currentScale.x,
            targetScale,
            changeSpeed * Time.deltaTime
        );

        healthFill.localScale = new Vector3(
            newScaleX,
            currentScale.y,
            currentScale.z
        );

        // Temporary testing
        // Press K to deal 10 damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
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

        UpdateTargetScale();
        UpdateHealthText();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        UpdateTargetScale();
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

        UpdateTargetScale();
        UpdateHealthText();
    }

    private void UpdateTargetScale()
    {
        targetScale = currentHealth / maxHealth;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text =
                Mathf.CeilToInt(currentHealth)
                + " / "
                + Mathf.CeilToInt(maxHealth);
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        Destroy(gameObject);
    }
}