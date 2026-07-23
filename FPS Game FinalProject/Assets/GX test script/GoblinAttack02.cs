using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class GoblinAttack02 : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 15f;
    public float attackRange = 5f;

    [Header("Attack")]
    public float attackCooldown = 3f;
    public float fireDelay = 1f;

    [Header("References")]
    public Animator animator;
    public GameObject firePrefab;
    public Transform firePoint;

    private Transform player;
    private NavMeshAgent agent;

    private PlayerHealthUI playerHealthUI;

    [Header("Damage")]
    public float attackDamage = 10f;

    private bool hasDetectedPlayer = false;
    private bool isAttacking = false;
    private bool hasShot = false;
    private float nextAttackTime = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found!");

        agent.stoppingDistance = attackRange;
        playerHealthUI = FindFirstObjectByType<PlayerHealthUI>();
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Detect player
        if (!hasDetectedPlayer && distance <= detectionRange)
        {
            hasDetectedPlayer = true;
        }

        // Idle
        if (!hasDetectedPlayer)
        {
            agent.isStopped = true;
            animator.SetBool("IsRunning", false);
            return;
        }

        // Chase
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("IsRunning", true);
        }
        // Attack
        else
        {
            agent.isStopped = true;

            if (agent.hasPath)
                agent.ResetPath();

            animator.SetBool("IsRunning", false);

            FacePlayer();

            if (!isAttacking && Time.time >= nextAttackTime)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hasShot = false;

        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(fireDelay);

        TryDamagePlayer();

        if (!hasShot)
        {
            ShootFire();
            hasShot = true;
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    void ShootFire()
    {
        if (firePrefab == null || firePoint == null)
            return;

        GameObject fire = Instantiate(
            firePrefab,
            firePoint.position,
            firePoint.rotation
        );

        // Destroy after 1 second
        Destroy(fire, 1f);
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

        // Only damage if the player is still inside attack range
        if (distanceToPlayer <= attackRange)
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

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            8f * Time.deltaTime
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}