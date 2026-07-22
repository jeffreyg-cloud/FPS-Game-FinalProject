using UnityEngine;
using System.Collections;

public class GoblinAttack : MonoBehaviour
{
    public Animator animator;
    public GameObject firePrefab;
    public Transform firePoint;

    public float fireDelay = 2f;
    public float fireDuration = 1f;

    private bool hasShot = false;

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // Only when actually inside the Attack animation
        if (state.IsName("Attack"))
        {
            if (!hasShot)
            {
                hasShot = true;
                StartCoroutine(ShootAfterDelay());
            }
        }
        else
        {
            hasShot = false;
        }
    }

    IEnumerator ShootAfterDelay()
    {
        yield return new WaitForSeconds(fireDelay);

        GameObject fire = Instantiate(
            firePrefab,
            firePoint.position + firePoint.forward * 0.3f,
            firePoint.rotation
        );

        ParticleSystem ps = fire.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            yield return new WaitForSeconds(fireDuration);
            ps.Stop();
            Destroy(fire, 2f);
        }
    }
}