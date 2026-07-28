using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyAttackPlayer : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 8f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;

    // Attack animation starts first, then the projectile fires after this delay.
    [SerializeField] private float fireDelay = 0.3f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Makes the projectile aim slightly above the Player's feet.
    [SerializeField] private float targetHeightOffset = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("References")]
    [SerializeField] private Animator animator;
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

    private bool hasDetectedPlayer;
    private bool isAttacking;
    private float nextAttackTime;
    private bool deathSoundPlayed;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

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
        }
        else
        {
            Debug.LogError(
                "Ranged Enemy cannot find Player. Check the Player Tag."
            );
        }

        // Enemy stops slightly inside its attack range.
        agent.stoppingDistance = attackRange * 0.9f;
    }

    private void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            PlayDeathSound();
            return;
        }

        if (player == null)
        {
            return;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // Once detected, the Enemy continues following the Player.
        if (!hasDetectedPlayer &&
            distanceToPlayer <= detectionRange)
        {
            hasDetectedPlayer = true;
        }

        if (!hasDetectedPlayer)
        {
            StopMoving();
            UpdateAnimation();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            HandleAttack();
        }
        else
        {
            FollowPlayer();
        }

        UpdateAnimation();
    }

    private void FollowPlayer()
    {
        // Do not move while the attack Coroutine is running.
        if (isAttacking)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void HandleAttack()
    {
        StopMoving();
        FacePlayer();

        if (isAttacking)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        nextAttackTime =
            Time.time + attackCooldown;

        if (animator != null)
        {
            animator.ResetTrigger(AttackHash);
            animator.SetTrigger(AttackHash);
        }

        // Wait until the animation reaches the firing moment.
        yield return new WaitForSeconds(fireDelay);

        if (player != null &&
            (enemyHealth == null || !enemyHealth.IsDead))
        {
            FireProjectile();
        }

        // Wait for the remaining cooldown.
        float remainingTime =
            Mathf.Max(0f, attackCooldown - fireDelay);

        yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError(
                "Projectile Prefab is not assigned."
            );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogError(
                "FirePoint is not assigned."
            );

            return;
        }

        Vector3 targetPosition =
            player.position + Vector3.up * targetHeightOffset;

        Vector3 shootDirection =
            targetPosition - firePoint.position;

        if (shootDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection.normalized)
        );

        EnemyProjectile projectile =
            projectileObject.GetComponent<EnemyProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(shootDirection);
        }
        else
        {
            Debug.LogError(
                "Enemy Bullet Prefab does not contain EnemyProjectile."
            );
        }

        PlayAttackSound();
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

    private void FacePlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void StopMoving()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        agent.isStopped = true;

        if (agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        float movementSpeed = agent.velocity.magnitude;

        animator.SetFloat(
            SpeedHash,
            movementSpeed,
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