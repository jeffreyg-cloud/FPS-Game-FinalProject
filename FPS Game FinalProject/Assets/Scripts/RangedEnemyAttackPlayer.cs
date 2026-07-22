using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemyAttackPlayer : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 8f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Projectile References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float targetHeightOffset = 1f;

    private Transform player;
    private NavMeshAgent agent;

    private float nextAttackTime;
    private bool hasDetectedPlayer;

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

        agent.stoppingDistance = attackRange * 0.9f;
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

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
            HandleAttack();
        }
        else
        {
            FollowPlayer();
        }

        UpdateMovementAnimation();
    }

    private void FollowPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void HandleAttack()
    {
        StopMoving();
        FacePlayer();

        if (Time.time < nextAttackTime)
        {
            return;
        }

        nextAttackTime =
            Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void StopMoving()
    {
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
            rotationSpeed * Time.deltaTime
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

    public void AnimationFireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Ranged Enemy ???? Projectile Prefab?");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Ranged Enemy ???? Fire Point?");
            return;
        }

        if (player == null)
        {
            return;
        }

        Vector3 targetPosition =
            player.position + Vector3.up * targetHeightOffset;

        Vector3 shootDirection =
            targetPosition - firePoint.position;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        EnemyProjectile projectile =
            projectileObject.GetComponent<EnemyProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(shootDirection);
        }
    }
}