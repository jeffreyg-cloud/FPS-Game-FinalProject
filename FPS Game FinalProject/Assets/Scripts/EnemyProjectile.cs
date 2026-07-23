using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifeTime = 2f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(moveDirection);
        }
    }

    private void FixedUpdate()
    {
        if (hasHit)
        {
            return;
        }

        rb.linearVelocity =
            moveDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
        {
            return;
        }

        // ???? Bullet ? Enemy ??? Enemy?
        if (other.GetComponentInParent<RangedEnemyAttackPlayer>() != null)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            return;
        }

        hasHit = true;

        bool hitPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (hitPlayer)
        {
            DamagePlayer();
        }

        Explode();
    }

    private void DamagePlayer()
    {
        PlayerHealthUI healthUI =
            FindFirstObjectByType<PlayerHealthUI>();

        if (healthUI != null)
        {
            healthUI.TakeDamage(damage);

            Debug.Log(
                $"Enemy Bullet ? Player ?? {damage} ????"
            );
        }
        else
        {
            Debug.LogWarning(
                "?????? PlayerHealthUI?"
            );
        }
    }

    private void Explode()
    {
        rb.linearVelocity = Vector3.zero;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            // ???????? Stop Action = Destroy?
            // ?????????????
            Destroy(explosion, explosionLifeTime);
        }
        else
        {
            Debug.LogWarning(
                "Enemy Bullet ???? Explosion Prefab?"
            );
        }

        Destroy(gameObject);
    }
}