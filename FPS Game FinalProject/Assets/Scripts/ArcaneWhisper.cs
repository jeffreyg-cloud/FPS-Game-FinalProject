using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Attach alongside WandStats on the "Arcane Whisper" wand GameObject.
[RequireComponent(typeof(WandStats))]
public class ArcaneWhisper : MonoBehaviour
{
    public GameObject bulletPrefab;    // visual-only prefab, no script/collider needed
    public GameObject impactEffectPrefab;
    public float fireRate = 0.4f;
    public float range = 35f;
    public float projectileSpeed = 30f;
    public float bulletHitRadius = 0.15f; // sphere-cast thickness for hit detection

    [Header("Swing")]
    public Transform wandMesh;      // the visual mesh child, NOT firePoint
    public float swingAngle = 40f;
    public float swingDuration = 0.15f;
    public float returnDuration = 0.15f;

    private WandStats stats;
    private float nextFireTime;

    private Quaternion meshStartRotation;
    private Coroutine swingRoutine;

    void Start()
    {
        stats = GetComponent<WandStats>();

        if (wandMesh != null)
            meshStartRotation = wandMesh.localRotation;
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.isPressed) return; // hold to fire
        if (Time.time < nextFireTime) return;

        TryFire();
    }

    void TryFire()
    {
        if (stats.wandManager != null && !stats.wandManager.TrySpendMana(stats.manaCost)) return;
        if (bulletPrefab == null || stats.firePoint == null) return;

        nextFireTime = Time.time + fireRate;

        AimFirePointAtCrosshair();

        if (stats.wandAnimator != null) stats.wandAnimator.SetTrigger("Fire");

        if (wandMesh != null)
        {
            if (swingRoutine != null) StopCoroutine(swingRoutine);
            swingRoutine = StartCoroutine(SwingRoutine());
        }

        if (stats.effectPrefab != null)
            Instantiate(stats.effectPrefab, stats.firePoint.position, stats.firePoint.rotation);

        GameObject bulletGO = Instantiate(bulletPrefab, stats.firePoint.position, stats.firePoint.rotation);
        StartCoroutine(MoveBullet(bulletGO, stats.firePoint.forward));
    }

    void AimFirePointAtCrosshair()
    {
        if (stats.camTrans == null || stats.firePoint == null) return;

        RaycastHit hit;

        if (Physics.Raycast(stats.camTrans.position, stats.camTrans.forward, out hit, stats.maxAimDistance))
        {
            stats.firePoint.LookAt(hit.point);
        }
        else
        {
            stats.firePoint.LookAt(stats.camTrans.position + stats.camTrans.forward * stats.maxAimDistance);
        }
    }

    IEnumerator SwingRoutine()
    {
        Quaternion swingRotation = meshStartRotation * Quaternion.Euler(-swingAngle, 0f, 0f);

        float t = 0f;
        while (t < swingDuration)
        {
            t += Time.deltaTime;
            wandMesh.localRotation = Quaternion.Slerp(meshStartRotation, swingRotation, t / swingDuration);
            yield return null;
        }

        t = 0f;
        while (t < returnDuration)
        {
            t += Time.deltaTime;
            wandMesh.localRotation = Quaternion.Slerp(swingRotation, meshStartRotation, t / returnDuration);
            yield return null;
        }

        wandMesh.localRotation = meshStartRotation;
    }

    // Manually moves the bullet each frame and sphere-casts along the step
    // it just took, so no script/collider is needed on the bullet itself.
    IEnumerator MoveBullet(GameObject bulletGO, Vector3 direction)
    {
        float elapsed = 0f;
        float lifeTime = range / Mathf.Max(projectileSpeed, 0.01f);

        while (elapsed < lifeTime)
        {
            Vector3 prevPos = bulletGO.transform.position;
            float step = projectileSpeed * Time.deltaTime;

            if (Physics.SphereCast(
                prevPos,
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
                    EnemyHealth enemyHealth =
                        hit.collider.GetComponentInParent<EnemyHealth>();

                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(stats.damage);
                        Debug.Log(
                            "Wand hit enemy: "
                            + enemyHealth.gameObject.name
                        );
                    }

                    if (impactEffectPrefab != null)
                    {
                        Instantiate(
                            impactEffectPrefab,
                            hit.point,
                            Quaternion.LookRotation(hit.normal)
                        );
                    }

                    Destroy(bulletGO);
                    yield break;
                }
            }

            bulletGO.transform.position = prevPos + direction * step;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(bulletGO); // ran out of range without hitting anything
    }
}