using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;

    [Header("Damage")]
    public float damage = 15f;

    [Header("Lifetime")]
    public float lifeTime = 5f;

    private PlayerHealthUI playerHealth;
    private Vector3 direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        // Find the PlayerHealthUI on the Canvas
        playerHealth = FindFirstObjectByType<PlayerHealthUI>();
    }

    public void SetTarget(Transform player)
    {
        direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fireball hit: " + other.name + " Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected!");

            if (playerHealth != null)
            {
                Debug.Log("Calling TakeDamage()");
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogError("PlayerHealthUI is NULL!");
            }

            Destroy(gameObject);
        }
    

        // Hit Ground
        if (other.CompareTag("ground"))
        {
            Debug.Log("Hit Ground");
            Destroy(gameObject);
        }
    }
}