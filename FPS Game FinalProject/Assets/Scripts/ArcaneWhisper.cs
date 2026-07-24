using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
// Attach alongside WandStats on the "Arcane Whisper" wand GameObject.
[RequireComponent(typeof(WandStats))]
public class ArcaneWhisperWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;    // visual-only prefab, no script/collider needed
    public GameObject impactEffectPrefab;
    public float fireRate = 0.4f;
    public float range = 35f;
    public float projectileSpeed = 30f;
    public float bulletHitRadius = 0.15f; // sphere-cast thickness for hit detection

    [Header("Fire Tilt")]
    public float tiltAngle = 45f;       // how far the wand tilts on fire
    public float tiltOutTime = 0.05f;   // time to reach full tilt
    public float tiltReturnTime = 0.15f; // time to return to normal

    private WandStats stats;
    private float nextFireTime;
    private Quaternion baseLocalRotation;
    private Coroutine tiltCoroutine;

    void Start()
    {
        stats = GetComponent<WandStats>();
        baseLocalRotation = transform.localRotation;
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

        // Aim firePoint to face exactly the same direction as the camera,
        // so the bullet travels perfectly parallel to the crosshair
        // regardless of where firePoint is physically positioned.
        if (stats.camTrans != null)
        {
            stats.firePoint.rotation = Quaternion.LookRotation(stats.camTrans.forward);
        }

        if (stats.wandAnimator != null) stats.wandAnimator.SetTrigger("Fire");
        if (stats.effectPrefab != null)
            Instantiate(stats.effectPrefab, stats.firePoint.position, stats.firePoint.rotation);

        GameObject bulletGO = Instantiate(bulletPrefab, stats.firePoint.position, stats.firePoint.rotation);
        // Runs on WandManager (which never gets disabled), so the bullet
        // keeps flying even if the player switches wands mid-flight.
        MonoBehaviour coroutineHost = stats.wandManager != null ? (MonoBehaviour)stats.wandManager : this;
        coroutineHost.StartCoroutine(MoveBullet(bulletGO, stats.firePoint.forward));

        // Play the tilt effect on this wand's own transform
        if (tiltCoroutine != null) StopCoroutine(tiltCoroutine);
        tiltCoroutine = StartCoroutine(TiltOnFire());
    }

    IEnumerator TiltOnFire()
    {
        Quaternion tiltedRotation = baseLocalRotation * Quaternion.Euler(-tiltAngle, 0f, 0f);

        // Tilt out
        float t = 0f;
        while (t < tiltOutTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(baseLocalRotation, tiltedRotation, t / tiltOutTime);
            yield return null;
        }
        transform.localRotation = tiltedRotation;

        // Return to normal
        t = 0f;
        while (t < tiltReturnTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(tiltedRotation, baseLocalRotation, t / tiltReturnTime);
            yield return null;
        }
        transform.localRotation = baseLocalRotation;
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
            if (Physics.SphereCast(prevPos, bulletHitRadius, direction, out RaycastHit hit, step, stats.hittableLayers))
            {
                bool isSelf = hit.collider.CompareTag("Player")
                    || hit.collider.CompareTag("Weapon")
                    || hit.collider.transform.root.CompareTag("Player");
                if (!isSelf)
                {
                    hit.collider.SendMessage("TakeDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
                    if (impactEffectPrefab != null)
                        Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
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