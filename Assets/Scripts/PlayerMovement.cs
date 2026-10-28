using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontalInput = 0f;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        // Move left
        if (keyboard.aKey.isPressed ||
            keyboard.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }

        // Move right
        if (keyboard.dKey.isPressed ||
            keyboard.rightArrowKey.isPressed)
        {
            horizontalInput = 1f;
        }

        // Flip sprite based on movement direction
        if (spriteRenderer != null)
        {
            if (horizontalInput > 0)
                spriteRenderer.flipX = false;
            else if (horizontalInput < 0)
                spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }
}

