using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI Reference")]
    [SerializeField] private EnemyHealthUI healthUI;

    [Header("Final Enemy Settings")]
    [SerializeField] private bool isFinalEnemy = false;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        // Don't take damage if enemy is already dead
        if (isDead || damage <= 0f)
        {
            return;
        }

        // Reduce health
        currentHealth -= damage;

        // Keep health between 0 and maxHealth
        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            gameObject.name
            + " took "
            + damage
            + " damage! Current HP: "
            + currentHealth
        );

        // Update health UI
        UpdateHealthUI();

        // Check if enemy died
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.SetHealth(
                currentHealth,
                maxHealth
            );
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log(
            gameObject.name
            + " died!"
        );

        // =========================================
        // ONLY FINAL ENEMY TRIGGERS SCENE CHANGE
        // =========================================

        if (isFinalEnemy)
        {
            Debug.Log(
                "FINAL ENEMY DEFEATED!"
            );

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.EnemyDefeated();
            }
            else
            {
                Debug.LogError(
                    "SceneTransitionManager Instance not found!"
                );
            }
        }

        // Destroy enemy immediately
        Destroy(gameObject);
    }
}