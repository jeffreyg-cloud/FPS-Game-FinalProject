using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Attach alongside WandStats on the "Starfall Scepter" wand GameObject.
[RequireComponent(typeof(WandStats))]
public class StarfallScepterWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;    // visual-only prefab, no script/collider needed
    public GameObject explosionEffectPrefab;
    public float cooldown = 1.5f;
    public float range = 25f;
    public float projectileSpeed = 15f;
    public float bulletHitRadius = 0.2f;
    public float explosionRadius = 4f;
    public float knockbackForce = 5f;

    private WandStats stats;
    private float nextCastTime;

    void Start()
    {
        stats = GetComponent<WandStats>();
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return; // single click
        if (Time.time < nextCastTime) return;

        TryCast();
    }

    void TryCast()
    {
        if (stats.wandManager != null && !stats.wandManager.TrySpendMana(stats.manaCost)) return;
        if (bulletPrefab == null || stats.firePoint == null) return;

        nextCastTime = Time.time + cooldown;

        if (stats.wandAnimator != null) stats.wandAnimator.SetTrigger("Cast");
        if (stats.effectPrefab != null)
            Instantiate(stats.effectPrefab, stats.firePoint.position, stats.firePoint.rotation);

        GameObject bulletGO = Instantiate(bulletPrefab, stats.firePoint.position, stats.firePoint.rotation);
        StartCoroutine(MoveBullet(bulletGO, stats.firePoint.forward));
    }

    // Moves the orb each frame and sphere-casts along the step it just took.
    // On hit (or at max range), it explodes instead of dealing single-target damage.
    IEnumerator MoveBullet(GameObject bulletGO, Vector3 direction)
    {
        float elapsed = 0f;
        float lifeTime = range / Mathf.Max(projectileSpeed, 0.01f);

        while (elapsed < lifeTime)
        {
            Vector3 prevPos = bulletGO.transform.position;
            float step = projectileSpeed * Time.deltaTime;

            if (Physics.SphereCast(prevPos, bulletHitRadius, direction, out RaycastHit hit, step, stats.hittableLayers))
            {
                bool isSelf = hit.collider.CompareTag("Player")
                    || hit.collider.CompareTag("Weapon")
                    || hit.collider.transform.root.CompareTag("Player");

                if (!isSelf)
                {
                    Destroy(bulletGO);
                    Explode(hit.point);
                    yield break;
                }
            }

            bulletGO.transform.position = prevPos + direction * step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reached max range without hitting anything -> explode at final position anyway
        Vector3 finalPos = bulletGO.transform.position;
        Destroy(bulletGO);
        Explode(finalPos);
    }

    void Explode(Vector3 center)
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, center, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(center, explosionRadius, stats.hittableLayers))
        {
            if (col.CompareTag("Player") || col.CompareTag("Weapon") || col.transform.root.CompareTag("Player"))
                continue;

            col.SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);

            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 dir = (col.transform.position - center).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}