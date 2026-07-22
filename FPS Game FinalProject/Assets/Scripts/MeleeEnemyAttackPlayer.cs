using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyAttackPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("References")]
    [SerializeField] private Animator animator;

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
                "Enemy ??? Player???? Player Tag?"
            );
        }

        agent.stoppingDistance = attackRange * 0.8f;
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

        // Enemy ????? Player ??????????
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
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void StopAndAttack()
    {
        StopMoving();
        FacePlayer();

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime =
                Time.time + attackCooldown;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
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