using UnityEngine;
using System.Collections;

public class QueenBoss : MonoBehaviour
{
    [Header("References")]
    public EnemyHealth enemyHealth;
    public Animator animator;

    private PlayerHealthUI playerHealthUI;
    private Transform playerTransform;

    [Header("Boss Activation Area")]
    public Collider bossTrigger;

    [Header("Projectile (Visual Effect Only)")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Summon")]
    public GameObject pawnPrefab;
    public GameObject knightPrefab;
    public Transform[] summonPoints;

    [Header("Summon Amount")]
    public int phase75PawnCount = 4;
    public int phase75KnightCount = 0;

    public int phase50PawnCount = 4;
    public int phase50KnightCount = 2;

    public int phase25PawnCount = 6;
    public int phase25KnightCount = 4;

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

        if (bossTrigger == null)
            bossTrigger = GetComponent<Collider>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;

            playerHealthUI = player.GetComponent<PlayerHealthUI>();

            if (playerHealthUI == null)
                playerHealthUI = player.GetComponentInChildren<PlayerHealthUI>();
        }

        if (playerHealthUI == null)
            playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();
    }

    void Update()
    {
        // Continuously check the player's real position.
        if (bossActivated && !IsPlayerInsideBossTrigger())
        {
            StopBossFight();
        }

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
            playerTransform = other.transform;

            playerHealthUI = other.GetComponent<PlayerHealthUI>();

            if (playerHealthUI == null)
                playerHealthUI = other.GetComponentInChildren<PlayerHealthUI>();

            if (playerHealthUI == null)
                playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();

            bossActivated = true;

            Debug.Log("Boss Fight Started!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            bossActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopBossFight();
        }
    }

    bool IsPlayerInsideBossTrigger()
    {
        if (bossTrigger == null || playerTransform == null)
            return false;

        Vector3 playerPosition = playerTransform.position;
        Vector3 closestPoint = bossTrigger.ClosestPoint(playerPosition);

        return Vector3.Distance(playerPosition, closestPoint) < 0.05f;
    }

    void StopBossFight()
    {
        bossActivated = false;

        StopAllCoroutines();

        attacking = false;
        summoning = false;

        Debug.Log("Player is outside the boss area. Boss stopped attacking!");
    }

    void CheckPhase()
    {
        float hp = enemyHealth.CurrentHealth / enemyHealth.MaxHealth;

        if (!phase75 && hp <= 0.75f)
        {
            phase75 = true;
            SummonForPhase(75);
        }

        if (!phase50 && hp <= 0.50f)
        {
            phase50 = true;
            SummonForPhase(50);
        }

        if (!phase25 && hp <= 0.25f)
        {
            phase25 = true;
            SummonForPhase(25);
        }
    }

    void SummonForPhase(int phase)
    {
        if (phase == 75)
        {
            StartCoroutine(
                SummonRoutine(phase75PawnCount, phase75KnightCount)
            );
        }
        else if (phase == 50)
        {
            StartCoroutine(
                SummonRoutine(phase50PawnCount, phase50KnightCount)
            );
        }
        else if (phase == 25)
        {
            StartCoroutine(
                SummonRoutine(phase25PawnCount, phase25KnightCount)
            );
        }
    }

    IEnumerator AttackRoutine()
    {
        attacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        if (!bossActivated || !IsPlayerInsideBossTrigger())
        {
            attacking = false;
            yield break;
        }

        DealDamageToPlayer();
        ShootProjectile();

        yield return new WaitForSeconds(attackCooldown);

        attacking = false;
    }

    void DealDamageToPlayer()
    {
        if (!bossActivated || !IsPlayerInsideBossTrigger())
            return;

        if (playerHealthUI == null)
            return;

        playerHealthUI.TakeDamage(attackDamage);
    }

    void ShootProjectile()
    {
        if (!bossActivated || !IsPlayerInsideBossTrigger())
            return;

        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        if (projectilePrefab == null || firePoint == null)
            return;

        Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
    }

    IEnumerator SummonRoutine(int pawnCount, int knightCount)
    {
        summoning = true;

        animator.SetTrigger("Summon");

        if (audioSource != null && summonSound != null)
            audioSource.PlayOneShot(summonSound);

        yield return new WaitForSeconds(1.5f);

        if (!bossActivated || !IsPlayerInsideBossTrigger())
        {
            summoning = false;
            yield break;
        }

        for (int i = 0; i < pawnCount; i++)
            SpawnEnemy(pawnPrefab);

        for (int i = 0; i < knightCount; i++)
            SpawnEnemy(knightPrefab);

        summoning = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (!bossActivated || !IsPlayerInsideBossTrigger())
            return;

        if (enemyPrefab == null)
            return;

        if (summonPoints == null || summonPoints.Length == 0)
            return;

        Transform point =
            summonPoints[Random.Range(0, summonPoints.Length)];

        Instantiate(
            enemyPrefab,
            point.position,
            point.rotation
        );
    }
}