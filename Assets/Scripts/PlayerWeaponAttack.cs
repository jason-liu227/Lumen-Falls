
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

    private static readonly int AttackState = Animator.StringToHash("Base Layer.Attack");

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

    // Called by the Attack clip when the blade reaches full extension.
    public void BeginStabDamage()
    {
        if (isActiveAndEnabled && isAttacking)
            swordDamage.StartDamage();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        playerAnimator.SetTrigger("Attack");

        // Wait for the Animator to evaluate the trigger.
        yield return null;

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == AttackState)
        {
            swordRenderer.enabled = true;
            do
            {
                yield return null;
            }
            while (playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == AttackState);
        }

        ResetWeapon();
        isAttacking = false;
    }
}
