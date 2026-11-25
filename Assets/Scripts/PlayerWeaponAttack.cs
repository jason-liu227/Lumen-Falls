
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private WeaponDamage swordDamage;
    [SerializeField] private Transform swordPivot;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer swordRenderer;

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.4f;

    private bool isAttacking;

    private void OnDisable()
    {
        StopAllCoroutines();
        isAttacking = false;
        ResetWeapon();
    }

    private void Start()
    {
        ResetWeapon();
    }

    private void ResetWeapon()
    {
        if (swordRenderer != null)
            swordRenderer.enabled = false;

        if (swordDamage != null)
            swordDamage.StopDamage();
    }

    private void Update()
    {
        // Flip weapon with player
        if (playerSprite != null && swordPivot != null)
        {
            Vector3 scale = swordPivot.localScale;

            scale.x = playerSprite.flipX
                ? -Mathf.Abs(scale.x)
                : Mathf.Abs(scale.x);

            swordPivot.localScale = scale;
        }

        bool attackPressed = Mouse.current?.leftButton.wasPressedThisFrame == true ||
                             Keyboard.current?.jKey.wasPressedThisFrame == true;
        if (attackPressed && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        // Show sword
        if (swordRenderer != null)
            swordRenderer.enabled = true;

        // Trigger player attack animation
        playerAnimator.SetTrigger("Attack");

        // Wind-up
        yield return new WaitForSeconds(0.1f);

        // Enable damage during swing
        swordDamage.StartDamage();

        yield return new WaitForSeconds(0.15f);

        // Disable damage after swing
        swordDamage.StopDamage();

        // Recovery
        yield return new WaitForSeconds(
            Mathf.Max(0f, attackDuration - 0.25f)
        );

        // Hide sword after attack
        if (swordRenderer != null)
            swordRenderer.enabled = false;

        isAttacking = false;
    }
}
