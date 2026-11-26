using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float attackCooldown = 1f;

    private float nextAttackTime;
    private EnemyMovement movement;
    private PlayerHealth pendingTarget;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        QueueHit(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        QueueHit(collision);
    }

    private void QueueHit(Collision2D collision)
    {
        if (!isActiveAndEnabled || (movement != null && !movement.CanDealContactDamage))
            return;

        PlayerHealth player =
            collision.gameObject.GetComponentInParent<PlayerHealth>();

        if (player == null || player.IsDead)
            return;

        if (Time.fixedTime >= nextAttackTime)
            pendingTarget = player;
    }

    private void FixedUpdate()
    {
        // Resolve the previous physics step after both collision and sword callbacks.
        PlayerHealth target = pendingTarget;
        pendingTarget = null;
        if (target == null || target.IsDead)
            return;

        nextAttackTime = Time.fixedTime + attackCooldown;
        target.TakeDamage(damage);
    }

    public void CancelPendingHit()
    {
        pendingTarget = null;
    }

    private void OnDisable()
    {
        CancelPendingHit();
    }
}
