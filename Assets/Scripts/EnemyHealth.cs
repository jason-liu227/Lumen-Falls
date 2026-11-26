using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 30;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 2f;

    private int currentHealth;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(int damage, Vector2? hitOrigin = null)
    {
        if (IsDead || damage <= 0)
            return;

        if (TryGetComponent(out EnemyAttack attack))
            attack.CancelPendingHit();

        if (hitOrigin.HasValue && TryGetComponent(out EnemyMovement movement))
            movement.ApplyKnockback(hitOrigin.Value);

        currentHealth -= damage;

        Debug.Log(gameObject.name + " took " + damage +
                  " damage. HP remaining: " + Mathf.Max(currentHealth, 0));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        Debug.Log(gameObject.name + " has died!");

        // Stop enemy movement immediately
        EnemyMovement movement = GetComponent<EnemyMovement>();

        if (movement != null)
            movement.StopMoving();

        // Remove enemy after delay
        Destroy(gameObject, destroyDelay);
    }
}
