using UnityEngine;
using System.Collections;

public class QueenBoss : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemyHealth;
    public Animator animator;

    private PlayerHealthUI playerHealthUI;

    [Header("Projectile (Visual Effect Only)")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Summon")]
    public GameObject pawnPrefab;
    public GameObject knightPrefab;
    public Transform[] summonPoints;

    [Header("Attack")]
    public float attackCooldown = 3f;
    public float attackDamage = 20f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip summonSound;

    private bool attacking;
    private bool summoning;

    private bool bossActivated;

    private bool phase75;
    private bool phase50;
    private bool phase25;

    void Start()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();
    }

    void Update()
    {
        if (!bossActivated)
            return;

        if (enemyHealth == null || enemyHealth.IsDead)
            return;

        CheckPhase();

        if (!attacking && !summoning)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bossActivated = true;
            Debug.Log("Boss Fight Started!");
        }
    }

    void CheckPhase()
    {
        float hp = enemyHealth.CurrentHealth / enemyHealth.MaxHealth;

        if (!phase75 && hp <= 0.75f)
        {
            phase75 = true;
            StartCoroutine(SummonRoutine(4, 0));
        }

        if (!phase50 && hp <= 0.50f)
        {
            phase50 = true;
            StartCoroutine(SummonRoutine(4, 2));
        }

        if (!phase25 && hp <= 0.25f)
        {
            phase25 = true;
            StartCoroutine(SummonRoutine(6, 4));
        }
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        DealDamageToPlayer();

        ShootProjectile();

        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }

    void DealDamageToPlayer()
    {
        if (playerHealthUI == null)
            return;

        playerHealthUI.TakeDamage(attackDamage);
    }

    void ShootProjectile()
    {
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        if (projectilePrefab == null || firePoint == null)
            return;

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    IEnumerator SummonRoutine(int pawnCount, int knightCount)
    {
        summoning = true;

        animator.SetTrigger("Summon");

        if (audioSource != null && summonSound != null)
            audioSource.PlayOneShot(summonSound);

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < pawnCount; i++)
            SpawnEnemy(pawnPrefab);

        for (int i = 0; i < knightCount; i++)
            SpawnEnemy(knightPrefab);

        summoning = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
            return;

        if (summonPoints == null || summonPoints.Length == 0)
            return;

        Transform point = summonPoints[Random.Range(0, summonPoints.Length)];

        Instantiate(enemyPrefab, point.position, point.rotation);
    }
}