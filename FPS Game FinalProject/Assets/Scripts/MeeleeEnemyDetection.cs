using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // =========================
    // TARGET
    // =========================

    [Header("Target")]
    [SerializeField] private Transform player;


    // =========================
    // ENEMY SETTINGS
    // =========================

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackInterval = 1.2f;


    // =========================
    // ENEMY STATES
    // =========================

    private enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    private EnemyState currentState = EnemyState.Idle;


    // =========================
    // TIMER
    // =========================

    private float attackTimer;


    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        // If we don't have a player target
        if (player == null)
        {
            return;
        }


        // Calculate distance between enemy and player
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );


        // =========================
        // DETERMINE ENEMY STATE
        // =========================

        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Idle;
        }
        else if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Attacking;
        }


        // =========================
        // EXECUTE STATE
        // =========================

        switch (currentState)
        {
            case EnemyState.Idle:

                Idle();

                break;


            case EnemyState.Chasing:

                ChasePlayer();

                break;


            case EnemyState.Attacking:

                Attack();

                break;
        }
    }


    // =========================
    // IDLE
    // =========================

    private void Idle()
    {
        // Enemy does nothing
    }


    // =========================
    // CHASE PLAYER
    // =========================

    private void ChasePlayer()
    {
        Vector3 direction = (
            player.position - transform.position
        ).normalized;


        // Don't move vertically
        direction.y = 0f;


        // Face the player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }


        // Move toward player
        transform.position +=
            direction *
            movementSpeed *
            Time.deltaTime;
    }


    // =========================
    // ATTACK
    // =========================

    private void Attack()
    {
        // Stop moving

        // Face player
        Vector3 direction = (
            player.position - transform.position
        ).normalized;

        direction.y = 0f;


        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }


        // Attack cooldown
        attackTimer -= Time.deltaTime;


        if (attackTimer <= 0f)
        {
            Debug.Log("Enemy is attacking!");

            // Reset timer
            attackTimer = attackInterval;
        }
    }


    // =========================
    // GIZMOS
    // =========================

    private void OnDrawGizmosSelected()
    {
        // Detection Range
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );


        // Attack Range
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}