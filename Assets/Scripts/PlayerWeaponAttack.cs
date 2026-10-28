using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponAttack : MonoBehaviour
{
    [SerializeField] private Animator swordAnimator;
    [SerializeField] private WeaponDamage swordDamage;
    [SerializeField] private Transform swordPivot;
    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField] private float attackDuration = 0.4f;

    private bool isAttacking;

    private void Update()
    {
        if (playerSprite != null && swordPivot != null)
        {
            Vector3 scale = swordPivot.localScale;
            scale.x = playerSprite.flipX
                ? -Mathf.Abs(scale.x)
                : Mathf.Abs(scale.x);

            swordPivot.localScale = scale;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        swordAnimator.SetTrigger("Attack");

        // Wind-up
        yield return new WaitForSeconds(0.1f);

        swordDamage.StartDamage();

        // Active swing
        yield return new WaitForSeconds(0.15f);

        swordDamage.StopDamage();

        // Recovery
        yield return new WaitForSeconds(
            Mathf.Max(0f, attackDuration - 0.25f)
        );

        isAttacking = false;
    }
}

