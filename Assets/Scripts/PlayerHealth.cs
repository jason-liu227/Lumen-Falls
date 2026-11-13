using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDead = false;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerWeaponAttack playerAttack;
    private Rigidbody2D rb;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerWeaponAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("Player Health: " + currentHealth);

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

        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Attack");
        animator.SetBool("IsDead", true);

        // Stop player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop player attacks
        if (playerAttack != null)
            playerAttack.enabled = false;

        // Stop horizontal movement
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        Debug.Log("Player Died");
    }
}

