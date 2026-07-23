using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fire Settings")]
    public float speed = 15f;
    public float lifeTime = 5f;

    private void Start()
    {
        // Destroy automatically after a few seconds
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit Player
        if (other.CompareTag("GameController"))
        {
            Debug.Log("Player Hit!");

            // TODO: Damage player here

            Destroy(gameObject);
        }

        // Hit Ground
        if (other.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
    }
}