using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float horizontalInput;
    private int currentHealth;
    private bool isDead = false;

    [Header("Weapon")]
    [SerializeField] private SpriteRenderer swordRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;


        // Hide the sword when the game starts
        if (swordRenderer != null)
        {
            swordRenderer.enabled = false;
        }

    }

    private void Update()
    {
        if (isDead)
            return;

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

        // Attack animation
        if (keyboard.jKey.wasPressedThisFrame)
        {
            animator.SetTrigger("Attack");
        }

        // Test damage with H
        if (keyboard.hKey.wasPressedThisFrame)
        {
            TakeDamage(20);
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
            return;
        }

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    private void Die()
    {
        isDead = true;
        horizontalInput = 0f;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.SetBool("IsDead", true);
    }

    public void ShowSword()
    {
        if (swordRenderer != null)
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
