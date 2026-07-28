using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class DragonShoot : MonoBehaviour
{
    [Header("Fire Settings")]
    public GameObject firePrefab;
    public Transform firePoint;

    [Tooltip("Time between each fire breath")]
    public float fireInterval = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    private Coroutine fireRoutine;
    private bool isBreathing = false;

    public void StartBreathing()
    {
        // Already breathing
        if (isBreathing)
            return;

        isBreathing = true;

        fireRoutine = StartCoroutine(BreathFire());
    }

    public void StopBreathing()
    {
        isBreathing = false;

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    IEnumerator BreathFire()
    {
        while (isBreathing)
        {
            ShootFire();

            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void ShootFire()
    {
        if (audioSource != null && attackSound != null)
        {
            Debug.Log("Playing Dragon Sound");
            audioSource.PlayOneShot(attackSound);
        }
        if (firePrefab == null || firePoint == null)
            return;

        GameObject fire = Instantiate(
            firePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Fireball fireball = fire.GetComponent<Fireball>();

        if (fireball != null)
        {
            fireball.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        }
    }
}