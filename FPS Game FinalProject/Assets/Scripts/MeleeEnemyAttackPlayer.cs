using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyAttackPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    // Attack animation starts first, then damage happens after this delay.
    [SerializeField] private float damageDelay = 0.4f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;

    [Range(0f, 1f)]
    [SerializeField] private float attackVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float deathVolume = 1f;

    [Header("Audio Distance Settings")]

    // 0 = 2D sound, 1 = fully 3D sound.
    // A lower value makes the sound clearer and less affected by distance.
    [Range(0f, 1f)]
    [SerializeField] private float spatialBlend = 0.4f;

    // The sound remains at full volume within this distance.
    [SerializeField] private float minSoundDistance = 8f;

    // Maximum distance from which the sound can still be heard.
    [SerializeField] private float maxSoundDistance = 45f;

    private Transform player;
    private NavMeshAgent agent;

    private float nextAttackTime;
    private bool hasDetectedPlayer;
    private bool isAttacking;
    private bool deathSoundPlayed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        ConfigureAudioSource();
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            // Try to find PlayerHealthUI from the Player or its children.
            if (playerHealthUI == null)
            {
                playerHealthUI =
                    playerObject.GetComponentInChildren<PlayerHealthUI>();
            }
        }
        else
        {
            Debug.LogError(
                "Melee Enemy cannot find Player. Check whether the Player object has the Player tag."
            );
        }

        // Fallback if PlayerHealthUI is located on the Canvas.
        if (playerHealthUI == null)
        {
            playerHealthUI =
                FindFirstObjectByType<PlayerHealthUI>();
        }

        agent.stoppingDistance = attackRange * 0.8f;
    }

    private void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            PlayDeathSound();
            return;
        }

        if (player == null ||
            !agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // Once the Player is detected, keep chasing them.
        if (!hasDetectedPlayer &&
            distanceToPlayer <= detectionRange)
        {
            hasDetectedPlayer = true;
        }

        if (!hasDetectedPlayer)
        {
            StopMoving();
            UpdateMovementAnimation();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            StopAndAttack();
        }
        else
        {
            FollowPlayer();
        }

        UpdateMovementAnimation();
    }

    private void FollowPlayer()
    {
        if (isAttacking)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void StopAndAttack()
    {
        StopMoving();
        FacePlayer();

        if (!isAttacking &&
            Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        nextAttackTime =
            Time.time + attackCooldown;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        PlayAttackSound();

        // Wait until the attack animation reaches the hit moment.
        yield return new WaitForSeconds(damageDelay);

        if (enemyHealth == null || !enemyHealth.IsDead)
        {
            TryDamagePlayer();
        }

        // Prevent following until the attack cooldown has mostly completed.
        float remainingCooldown =
            Mathf.Max(0f, attackCooldown - damageDelay);

        yield return new WaitForSeconds(remainingCooldown);

        isAttacking = false;
    }

    private void TryDamagePlayer()
    {
        if (player == null)
        {
            return;
        }

        if (playerHealthUI == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " cannot damage Player because PlayerHealthUI was not found."
            );

            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // Check again because the Player may have moved away during the animation.
        if (distanceToPlayer <= attackRange + 0.3f)
        {
            playerHealthUI.TakeDamage(attackDamage);

            Debug.Log(
                gameObject.name +
                " dealt " +
                attackDamage +
                " damage to Player."
            );
        }
    }

    private void ConfigureAudioSource()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;

        audioSource.spatialBlend = spatialBlend;
        audioSource.minDistance = minSoundDistance;
        audioSource.maxDistance = maxSoundDistance;

        audioSource.rolloffMode =
            AudioRolloffMode.Logarithmic;
    }

    private void PlayAttackSound()
    {
        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(
                attackClip,
                attackVolume
            );
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            PlayDeathSound();
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            PlayDeathSound();
        }
    }

    private void PlayDeathSound()
    {
        if (deathSoundPlayed)
        {
            return;
        }

        deathSoundPlayed = true;

        if (deathClip == null)
        {
            return;
        }

        GameObject temporaryAudioObject =
            new GameObject(gameObject.name + " Death Sound");

        temporaryAudioObject.transform.position =
            transform.position;

        AudioSource temporaryAudioSource =
            temporaryAudioObject.AddComponent<AudioSource>();

        temporaryAudioSource.clip = deathClip;
        temporaryAudioSource.volume = deathVolume;

        temporaryAudioSource.playOnAwake = false;
        temporaryAudioSource.loop = false;

        temporaryAudioSource.spatialBlend = spatialBlend;
        temporaryAudioSource.minDistance = minSoundDistance;
        temporaryAudioSource.maxDistance = maxSoundDistance;

        temporaryAudioSource.rolloffMode =
            AudioRolloffMode.Logarithmic;

        temporaryAudioSource.Play();

        Destroy(
            temporaryAudioObject,
            deathClip.length + 0.2f
        );
    }

    private void StopMoving()
    {
        if (!agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;

        if (agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    private void FacePlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime
        );
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(
            "Speed",
            agent.velocity.magnitude,
            0.1f,
            Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}