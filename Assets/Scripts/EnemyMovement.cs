using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform player;

    [Header("Sprite Direction")]
    [SerializeField] private bool facesLeftByDefault = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;

    private float moveDirection;
    private bool movementStopped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (movementStopped || (enemyHealth != null && enemyHealth.IsDead))
        {
            moveDirection = 0f;
            return;
        }

        moveDirection = 0f;

        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectionRadius)
        {
            float horizontalDistance =
                player.position.x - transform.position.x;

            // Chase player
            if (Mathf.Abs(horizontalDistance) > stoppingDistance)
            {
                moveDirection = Mathf.Sign(horizontalDistance);
            }

            // Face player
            if (spriteRenderer != null)
            {
                if (horizontalDistance > 0.01f)
                    spriteRenderer.flipX = facesLeftByDefault;
                else if (horizontalDistance < -0.01f)
                    spriteRenderer.flipX = !facesLeftByDefault;
            }
        }
    }

    private void FixedUpdate()
    {
        if (movementStopped || (enemyHealth != null && enemyHealth.IsDead))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void StopMoving()
    {
        movementStopped = true;
        moveDirection = 0f;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
