using UnityEngine;
using System.Collections;

public class DragonShoot : MonoBehaviour
{
    public GameObject firePrefab;
    public Transform firePoint;

    public float fireInterval = 2f;

    private bool canShoot = false;

    private Coroutine fireRoutine;

    public void StartBreathing()
    {
        if (fireRoutine == null)
        {
            canShoot = true;
            fireRoutine = StartCoroutine(BreathFire());
        }
    }

    public void StopBreathing()
    {
        canShoot = false;

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    IEnumerator BreathFire()
    {
        while (canShoot)
        {
            Instantiate(firePrefab,
                        firePoint.position,
                        firePoint.rotation);

            yield return new WaitForSeconds(fireInterval);
        }
    }
}