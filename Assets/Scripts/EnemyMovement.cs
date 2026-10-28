using UnityEngine;

public class EnemyChase2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
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

            // Move towards player until close enough
            if (Mathf.Abs(horizontalDistance) > stoppingDistance)
            {
                moveDirection = Mathf.Sign(horizontalDistance);
            }

            // Flip sprite to face player
            if (spriteRenderer != null)
            {
                if (horizontalDistance > 0.01f)
                    spriteRenderer.flipX = true;
                else if (horizontalDistance < -0.01f)
                    spriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Yellow circle = detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Red circle = stopping distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}

