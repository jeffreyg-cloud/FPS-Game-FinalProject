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

    // Attack ???????????? Bullet?
    // ?????????????
    [SerializeField] private float fireDelay = 0.3f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // ? Bullet ?? Player ???????????
    [SerializeField] private float targetHeightOffset = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private Transform player;
    private NavMeshAgent agent;

    private bool hasDetectedPlayer;
    private bool isAttacking;
    private float nextAttackTime;

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
                "Ranged Enemy ??? Player???? Player Tag?"
            );
        }

        // Enemy ????????????
        agent.stoppingDistance = attackRange * 0.9f;
    }

    private void Update()
    {
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

        // ???????????Enemy ????? Player?
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
        // Attack Coroutine ???????????
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

        // ??????????????
        yield return new WaitForSeconds(fireDelay);

        // ??????? Player ???????
        if (player != null)
        {
            FireProjectile();
        }

        // ???????????
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
                "Projectile Prefab ?????"
            );
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError(
                "FirePoint ?????"
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
                "Enemy Bullet Prefab ??? EnemyProjectile ???"
            );
        }
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