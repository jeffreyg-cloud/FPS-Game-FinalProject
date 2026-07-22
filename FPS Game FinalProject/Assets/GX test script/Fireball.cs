using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 12f;
    public float destroyTime = 2f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
    }
}

