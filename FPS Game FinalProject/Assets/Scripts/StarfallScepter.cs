using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Attach alongside WandStats on the "Starfall Scepter" wand GameObject.
[RequireComponent(typeof(WandStats))]
public class StarfallScepterWeapon : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject bulletPrefab;
    public GameObject explosionEffectPrefab;

    [Header("Attack Settings")]
    public float cooldown = 1.5f;
    public float range = 25f;
    public float projectileSpeed = 15f;
    public float bulletHitRadius = 0.2f;

    [Header("Explosion")]
    public float explosionRadius = 4f;
    public float knockbackForce = 5f;

    private WandStats stats;
    private float nextCastTime;

    private void Start()
    {
        stats = GetComponent<WandStats>();
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (Time.time < nextCastTime)
        {
            return;
        }

        TryCast();
    }

    private void TryCast()
    {
        if (stats.wandManager != null &&
            !stats.wandManager.TrySpendMana(stats.manaCost))
        {
            return;
        }

        if (bulletPrefab == null || stats.firePoint == null)
        {
            return;
        }

        nextCastTime = Time.time + cooldown;

        if (stats.wandAnimator != null)
        {
            stats.wandAnimator.SetTrigger("Cast");
        }

        if (stats.effectPrefab != null)
        {
            Instantiate(
                stats.effectPrefab,
                stats.firePoint.position,
                stats.firePoint.rotation
            );
        }

        GameObject bulletGO = Instantiate(
            bulletPrefab,
            stats.firePoint.position,
            stats.firePoint.rotation
        );

        StartCoroutine(
            MoveBullet(
                bulletGO,
                stats.firePoint.forward
            )
        );
    }

    private IEnumerator MoveBullet(
        GameObject bulletGO,
        Vector3 direction
    )
    {
        float elapsed = 0f;

        float lifeTime =
            range / Mathf.Max(projectileSpeed, 0.01f);

        direction = direction.normalized;

        while (elapsed < lifeTime)
        {
            if (bulletGO == null)
            {
                yield break;
            }

            Vector3 previousPosition =
                bulletGO.transform.position;

            float step =
                projectileSpeed * Time.deltaTime;

            if (Physics.SphereCast(
                previousPosition,
                bulletHitRadius,
                direction,
                out RaycastHit hit,
                step,
                stats.hittableLayers
            ))
            {
                bool isSelf =
                    hit.collider.CompareTag("Player")
                    || hit.collider.CompareTag("Weapon")
                    || hit.collider.transform.root.CompareTag("Player");

                if (!isSelf)
                {
                    Vector3 explosionPosition = hit.point;

                    Destroy(bulletGO);
                    Explode(explosionPosition);

                    yield break;
                }
            }

            bulletGO.transform.position =
                previousPosition + direction * step;

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (bulletGO != null)
        {
            Vector3 finalPosition =
                bulletGO.transform.position;

            Destroy(bulletGO);
            Explode(finalPosition);
        }
    }

    private void Explode(Vector3 center)
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(
                explosionEffectPrefab,
                center,
                Quaternion.identity
            );
        }

        Collider[] colliders = Physics.OverlapSphere(
            center,
            explosionRadius,
            stats.hittableLayers
        );

        // ????? Enemy ????? Collider ???????
        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider col in colliders)
        {
            bool isPlayer =
                col.CompareTag("Player")
                || col.CompareTag("Weapon")
                || col.transform.root.CompareTag("Player");

            if (isPlayer)
            {
                continue;
            }

            EnemyHealth enemyHealth =
                col.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null &&
                damagedEnemies.Add(enemyHealth))
            {
                enemyHealth.TakeDamage(stats.damage);

                Debug.Log(
                    "Starfall explosion damaged "
                    + enemyHealth.gameObject.name
                    + " for "
                    + stats.damage
                );
            }

            Rigidbody targetRigidbody =
                col.attachedRigidbody;

            if (targetRigidbody != null)
            {
                Vector3 knockbackDirection =
                    col.transform.position - center;

                knockbackDirection.y = 0.3f;

                if (knockbackDirection.sqrMagnitude > 0.001f)
                {
                    knockbackDirection.Normalize();

                    targetRigidbody.AddForce(
                        knockbackDirection * knockbackForce,
                        ForceMode.Impulse
                    );
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null || stats.firePoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            stats.firePoint.position,
            explosionRadius
        );
    }
}