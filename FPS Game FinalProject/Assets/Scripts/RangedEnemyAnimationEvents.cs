using UnityEngine;

public class RangedEnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private RangedEnemyAttackPlayer rangedEnemyAttack;

    private void Awake()
    {
        if (rangedEnemyAttack == null)
        {
            rangedEnemyAttack = GetComponentInParent<RangedEnemyAttackPlayer>();
        }
    }

    public void AnimationFireProjectile()
    {
        if (rangedEnemyAttack != null)
        {
            rangedEnemyAttack.AnimationFireProjectile();
        }
    }
}