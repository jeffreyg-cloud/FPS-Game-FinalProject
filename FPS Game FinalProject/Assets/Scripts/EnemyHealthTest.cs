using UnityEngine;

public class EnemyHealthTest : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemyHealth.TakeDamage(20);
        }
    }
}