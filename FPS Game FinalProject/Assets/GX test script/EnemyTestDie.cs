using UnityEngine;
using System.Collections;

public class EnemyTestDie : MonoBehaviour
{
    private Animator animator;

    public float destroyDelay = 2f; // Adjust to match the Lose animation length

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Press L to play Lose animation
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger("Lose");
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}