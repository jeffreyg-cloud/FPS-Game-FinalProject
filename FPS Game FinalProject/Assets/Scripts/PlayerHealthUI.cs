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

    [SerializeField] private HealthTutorial healthTutorial;

    [Header("Respawn")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerManaUI playerManaUI;

    private float fullWidth;
    private float targetWidth;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            maxHealth = Mathf.Max(1f, GameManager.Instance.maxHealth);
            currentHealth = Mathf.Clamp(GameManager.Instance.currentHealth, 0f, maxHealth);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        fullWidth = healthFill.sizeDelta.x;
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

        // Testing
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(10f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);

        UpdateTargetWidth();
        UpdateHealthText();
        SaveToGameManager();

        if (healthTutorial != null)
            healthTutorial.ShowHealthTutorial();

        // Player died
        if (currentHealth <= 0f)
        {
            Respawn();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

        UpdateTargetWidth();
        UpdateHealthText();
        SaveToGameManager();
    }

    public void SetHealth(float current, float maximum)
    {
        maxHealth = Mathf.Max(1f, maximum);
        currentHealth = Mathf.Clamp(current, 0f, maxHealth);

        UpdateTargetWidth();
        UpdateHealthText();
        SaveToGameManager();
    }

    private void Respawn()
    {
        // Restore Health
        currentHealth = maxHealth;

        UpdateTargetWidth();
        UpdateHealthText();
        SaveToGameManager();

        // Restore Mana
        if (playerManaUI != null)
        {
            playerManaUI.RestoreFullMana();
        }

        // Teleport to latest checkpoint
        if (playerTransform != null &&
            GameManager.Instance != null &&
            GameManager.Instance.hasCheckpoint)
        {
            CharacterController cc =
                playerTransform.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            playerTransform.position =
                GameManager.Instance.lastCheckpointPosition;

            playerTransform.rotation =
                GameManager.Instance.lastCheckpointRotation;

            if (cc != null)
                cc.enabled = true;
        }
    }

    private void SaveToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentHealth = currentHealth;
            GameManager.Instance.maxHealth = maxHealth;
        }
    }

    private void UpdateTargetWidth()
    {
        targetWidth = fullWidth * (currentHealth / maxHealth);
    }

    private void UpdateHealthText()
    {
        healthText.text =
            Mathf.CeilToInt(currentHealth) +
            " / " +
            Mathf.CeilToInt(maxHealth);
    }
}