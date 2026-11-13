using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime = 0f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerHealth player =
            collision.gameObject.GetComponentInParent<PlayerHealth>();

        if (player == null || player.IsDead)
            return;

        // Damage player only after cooldown
        if (Time.time >= nextAttackTime)
        {
            player.TakeDamage(damage);

            nextAttackTime = Time.time + attackCooldown;
        }
    }
}
