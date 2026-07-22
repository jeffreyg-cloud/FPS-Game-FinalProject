using UnityEngine;
using System.Collections;

public class DragonShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject firePrefab;
    public Transform firePoint;

    [Header("Fire Settings")]
    public float fireInterval = 0.15f;      // Change this in Inspector
    public float shootDownAngle = -45f;

    private bool canShoot = true;

    void Start()
    {
        StartCoroutine(BreathFire());
    }

    void Update()
    {
        // Press F to stop spawning new fire
        if (Input.GetKeyDown(KeyCode.F))
        {
            canShoot = false;
        }

        // Press G to resume spawning
        if (Input.GetKeyDown(KeyCode.G))
        {
            canShoot = true;
        }
    }

    IEnumerator BreathFire()
    {
        while (true)
        {
            if (canShoot)
            {
                Quaternion shootRotation =
                    firePoint.rotation * Quaternion.Euler(shootDownAngle, 0f, 0f);

                Instantiate(
                    firePrefab,
                    firePoint.position,
                    shootRotation
                );
            }

            yield return new WaitForSeconds(fireInterval);
        }
    }
}