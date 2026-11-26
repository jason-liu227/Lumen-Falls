using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Hop")]
    [SerializeField] private RuntimeAnimatorController hopController;
    [SerializeField, Min(0.1f)] private float hopSpeed = 2.5f;
    [SerializeField, Min(0f)] private float squashDuration = 0.1f;
    [SerializeField, Min(0f)] private float stretchDuration = 0.08f;
    [SerializeField, Min(0f)] private float landingDuration = 0.12f;

    [Header("Pounce")]
    [SerializeField] private Collider2D hopFloor;
    [SerializeField, Min(0f)] private float pounceRange = 2.2f;
    [SerializeField, Min(0f)] private float pounceWindup = 0.25f;
    [SerializeField, Min(0.1f)] private float pounceSpeed = 3.5f;
    [SerializeField, Min(0.1f)] private float pounceLift = 3.2f;

    [Header("Knockback")]
    [SerializeField, Min(0f)] private float knockbackSpeed = 4f;
    [SerializeField, Min(0f)] private float knockbackLift = 2.5f;
    [SerializeField, Min(0.02f)] private float knockbackDuration = 0.3f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform player;

    [Header("Sprite Direction")]
    [SerializeField] private bool facesLeftByDefault = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;

    private enum HopPose { Idle, Squash, Stretch, Airborne, Land, AttackWindup, AttackPounce }

    private readonly ContactPoint2D[] groundContacts = new ContactPoint2D[8];
    private Collider2D bodyCollider;
    private static readonly int HopPoseParameter = Animator.StringToHash("HopPose");
    private Animator hopAnimator;
    private HopPose hopPose;
    private float poseTime;

    private float moveDirection;
    private bool movementStopped;
    private float knockbackTimeRemaining;

    public bool CanDealContactDamage => !movementStopped && knockbackTimeRemaining <= 0f &&
        (hopController == null || hopPose == HopPose.AttackPounce);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        bodyCollider = GetComponent<Collider2D>();

        if (hopController == null)
            return;

        if (!TryGetComponent(out hopAnimator))
            hopAnimator = gameObject.AddComponent<Animator>();

        if (hopAnimator.runtimeAnimatorController != hopController)
            hopAnimator.runtimeAnimatorController = hopController;

        hopAnimator.enabled = true;
        SetHopPose(HopPose.Idle);
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
            if (spriteRenderer != null && hopPose != HopPose.AttackPounce && knockbackTimeRemaining <= 0f)
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
        if (knockbackTimeRemaining > 0f)
        {
            knockbackTimeRemaining -= Time.fixedDeltaTime;
            return;
        }

        if (movementStopped || (enemyHealth != null && enemyHealth.IsDead))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (hopAnimator != null && bodyCollider != null)
        {
            UpdateHop();
            return;
        }

        rb.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void UpdateHop()
    {
        poseTime += Time.fixedDeltaTime;
        Vector2 velocity = rb.linearVelocity;
        bool grounded = velocity.y <= 0.01f && HasGroundContact();

        switch (hopPose)
        {
            case HopPose.Idle:
                if (grounded && moveDirection != 0f)
                    SetHopPose(CanStartPounce() ? HopPose.AttackWindup : HopPose.Squash);
                break;

            case HopPose.Squash:
                if (!grounded || moveDirection == 0f)
                    SetHopPose(HopPose.Idle);
                else if (poseTime >= squashDuration)
                {
                    rb.linearVelocity = new Vector2(moveDirection * moveSpeed, hopSpeed);
                    SetHopPose(HopPose.Stretch);
                }
                break;

            case HopPose.AttackWindup:
                if (!grounded || !CanStartPounce())
                    SetHopPose(HopPose.Idle);
                else if (poseTime >= pounceWindup)
                {
                    rb.linearVelocity = new Vector2(moveDirection * pounceSpeed, pounceLift);
                    SetHopPose(HopPose.AttackPounce);
                }
                break;

            case HopPose.AttackPounce:
            case HopPose.Stretch:
            case HopPose.Airborne:
                if (grounded)
                {
                    rb.linearVelocity = new Vector2(0f, velocity.y);
                    SetHopPose(HopPose.Land);
                }
                else if (hopPose == HopPose.Stretch && poseTime >= stretchDuration)
                    SetHopPose(HopPose.Airborne);
                break;

            case HopPose.Land:
                if (poseTime >= landingDuration)
                    SetHopPose(HopPose.Idle);
                break;
        }
    }

    private bool CanStartPounce()
    {
        return moveDirection != 0f && player != null &&
            Vector2.Distance(rb.position, player.position) <= pounceRange;
    }

    private bool HasGroundContact()
    {
        float colliderCenterY = bodyCollider.bounds.center.y;
        int count = bodyCollider.GetContacts(groundContacts);
        for (int i = 0; i < count; i++)
        {
            ContactPoint2D contact = groundContacts[i];
            // Check support below the slime regardless of collider order in the contact.
            if (contact.point.y < colliderCenterY && Mathf.Abs(contact.normal.y) > 0.5f &&
                (hopFloor == null || contact.collider == hopFloor || contact.otherCollider == hopFloor))
                return true;
        }

        return false;
    }

    private void SetHopPose(HopPose pose)
    {
        hopPose = pose;
        poseTime = 0f;
        if (hopAnimator != null)
            hopAnimator.SetInteger(HopPoseParameter, (int)pose);
    }

    public void ApplyKnockback(Vector2 hitOrigin)
    {
        float separation = rb.position.x - hitOrigin.x;
        float direction = Mathf.Abs(separation) > 0.01f
            ? Mathf.Sign(separation)
            : moveDirection != 0f ? -moveDirection : 1f;

        knockbackTimeRemaining = knockbackDuration;
        SetHopPose(HopPose.Airborne);
        rb.linearVelocity = new Vector2(direction * knockbackSpeed, knockbackLift);
    }

    public void StopMoving()
    {
        movementStopped = true;
        moveDirection = 0f;

        if (knockbackTimeRemaining > 0f)
            return;

        SetHopPose(HopPose.Idle);

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
