using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float detectDistance = 10f;
    public float attackDistance = 2f;
    public float moveSpeed = 3f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("GameController");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player with tag 'GameController' not found!");
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectDistance)
        {
            Vector3 target = player.position;
            target.y = transform.position.y;

            transform.LookAt(target);

            if (distance > attackDistance)
            {
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsAttacking", false);

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", false);
        }
    }
}

