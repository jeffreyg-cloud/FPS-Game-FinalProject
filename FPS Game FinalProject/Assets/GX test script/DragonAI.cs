using UnityEngine;

public class DragonAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public DragonShoot dragonShoot;
    public Animator animator;

    [Header("Movement")]
    public float flySpeed = 8f;
    public float rotationSpeed = 5f;
    public float flyHeight = 6f;
    public float followDistance = 5f;

    private bool playerDetected = false;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (dragonShoot == null)
            dragonShoot = GetComponent<DragonShoot>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        animator.SetBool("IsFlying", false);
        animator.SetBool("IsAttacking", false);
    }

    private void Update()
    {
        if (!playerDetected)
            return;

        if (player == null)
            return;

        FacePlayer();
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = player.position;
        targetPosition.y += flyHeight;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            flySpeed * Time.deltaTime
        );

        animator.SetBool("IsFlying", true);
    }

    private void FacePlayer()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Dragon detected player!");

        playerDetected = true;

        animator.SetBool("IsAttacking", true);

        dragonShoot.StartBreathing();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player escaped.");

        playerDetected = false;

        animator.SetBool("IsFlying", false);
        animator.SetBool("IsAttacking", false);

        dragonShoot.StopBreathing();
    }

    private void OnDrawGizmosSelected()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();

        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireSphere(col.center, col.radius);
        }
    }
}