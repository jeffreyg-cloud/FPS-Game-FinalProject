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

            Debug.Log("playerHealth = " + playerHealth);

            if (playerHealth == null)
            {
                Debug.LogError("playerHealth is NULL");
            }
            else
            {
                Debug.Log("About to call TakeDamage");
                playerHealth.TakeDamage(damage);
                Debug.Log("TakeDamage finished");
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
    }
}