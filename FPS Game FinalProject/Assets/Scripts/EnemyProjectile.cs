using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        // Bullet ?? Enemy ????? Enemy ????
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            PlayerHealthUI healthUI =
                FindFirstObjectByType<PlayerHealthUI>();

            if (healthUI != null)
            {
                healthUI.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning(
                    "?????? PlayerHealthUI?"
                );
            }

            Destroy(gameObject);
            return;
        }

        // ???????????
        hasHit = true;
        Destroy(gameObject);
    }
}