using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 12f;
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
        transform.forward = moveDirection;
    }

    private void FixedUpdate()
    {
        if (hasHit)
        {
            return;
        }

        rb.linearVelocity = moveDirection * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
        {
            return;
        }

        // ?????? Enemy?
        if (other.CompareTag("Enemy"))
        {
            return;
        }

        PlayerHealthUI playerHealth =
            other.GetComponentInParent<PlayerHealthUI>();

        if (playerHealth != null)
        {
            hasHit = true;
            playerHealth.TakeDamage(damage);

            Debug.Log($"?????? {damage} ???");

            Destroy(gameObject);
            return;
        }

        // ???????????
        hasHit = true;
        Destroy(gameObject);
    }
}