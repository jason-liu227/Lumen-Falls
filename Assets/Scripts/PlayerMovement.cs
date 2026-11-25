using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float horizontalInput;
    private PlayerHealth health;

    [Header("Weapon")]
    [SerializeField] private SpriteRenderer swordRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        health = GetComponent<PlayerHealth>();

        // Hide the sword when the game starts
        if (swordRenderer != null)
        {
            swordRenderer.enabled = false;
        }

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

        // Idle and Walk animations
        animator.SetFloat(
            "Speed",
            Mathf.Abs(horizontalInput)
        );

        // Test damage with H
        if (keyboard.hKey.wasPressedThisFrame)
        {
            health?.TakeDamage(20);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void ShowSword()
    {
        if (isActiveAndEnabled && swordRenderer != null)
        {
            swordRenderer.enabled = true;
        }
    }

    public void HideSword()
    {
        if (swordRenderer != null)
        {
            swordRenderer.enabled = false;
        }
    }

}
