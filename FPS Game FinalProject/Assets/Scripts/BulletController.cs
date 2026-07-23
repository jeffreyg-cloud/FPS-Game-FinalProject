using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody rb;
    public GameObject impactEffect;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.linearVelocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the player/wand itself so it doesn't blow up on spawn
        if (other.CompareTag("Player") || other.CompareTag("Weapon"))
            return;

        float offset = 0.7f;
        Vector3 newPosition = transform.position - transform.forward * offset;
        Instantiate(impactEffect, newPosition, transform.rotation);

        Destroy(gameObject);
    }
}