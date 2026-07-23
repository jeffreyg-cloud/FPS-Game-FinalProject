using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class GoblinAttack02 : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float fireDelay = 1f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private NavMeshAgent agent;

    private bool hasDetectedPlayer = false;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found!");
        }
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Detect player
        if (!hasDetectedPlayer && distance <= detectionRange)
        {
            hasDetectedPlayer = true;
        }

        if (!hasDetectedPlayer)
        {
            StopMoving();
            UpdateMovementAnimation();
            return;
        }

        // Attack
        if (distance <= attackRange)
        {
            StopMoving();
            FacePlayer();

            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        // Chase
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        UpdateMovementAnimation();
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Wait until the goblin opens its mouth
        yield return new WaitForSeconds(fireDelay);

        ShootFire();

        yield return new WaitForSeconds(attackCooldown - fireDelay);

        isAttacking = false;
    }

    private void ShootFire()
    {
        if (firePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Fire Prefab or Fire Point is missing!");
            return;
        }

        Instantiate(
            firePrefab,
            firePoint.position,
            firePoint.rotation
        );
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
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime
        );
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}