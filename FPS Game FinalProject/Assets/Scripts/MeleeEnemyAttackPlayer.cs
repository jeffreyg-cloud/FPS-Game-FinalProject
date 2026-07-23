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

    private Transform player;
    private NavMeshAgent agent;

    private float nextAttackTime;
    private bool hasDetectedPlayer;
    private bool isAttacking;

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

        // Wait until the attack animation reaches the hit moment.
        yield return new WaitForSeconds(damageDelay);

        TryDamagePlayer();

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