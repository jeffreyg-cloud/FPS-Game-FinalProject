using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody rb;
    public GameObject impactEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
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
        //if (other.CompareTag("Enemy"))
       // {
        //    other.transform.parent.GetComponent<EnemyHealthController>().DamageEnemy();
       // }
        Destroy(gameObject);
        float offset = 0.7f;
        Vector3 newPosition = transform.position - transform.forward * offset;
        Instantiate(impactEffect, newPosition, transform.rotation);

        Destroy(impactEffect, 0.2f);
    }
}
