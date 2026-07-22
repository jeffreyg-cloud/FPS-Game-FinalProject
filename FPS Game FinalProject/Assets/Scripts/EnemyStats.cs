using UnityEngine;



public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Combat")]
    public float damage = 10f;
    public float attackInterval = 1.2f;

    [Header("Movement")]
    public float movementSpeed = 3.5f;

    [Header("Detection")]
    public float detectionRange = 12f;

    [Header("Attack")]
    public float attackRange = 1.8f;
}

