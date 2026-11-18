using UnityEngine;
using System.Collections.Generic;

public class PlayerMoving : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Controls")]
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.S;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Sliding Settings")]
    [Range(0.1f, 1f)]
    public float slideHeight = 0.5f;

    [Header("Ground Detection")]
    public float groundCheckDistance = 0.1f;

    private float moveInput;

    private bool jumpPressed;
    private bool isGrounded;
    private bool isSliding;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    // Keeps track of platforms the player is standing on.
    private HashSet<Collider2D> groundColliders =
        new HashSet<Collider2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null || playerCollider == null)
        {
            Debug.LogError(
                "Player needs a Rigidbody2D and BoxCollider2D!"
            );

            enabled = false;
            return;
        }

        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
    }

    void Update()
    {
        // MOVEMENT INPUT
        moveInput = 0f;

        if (Input.GetKey(moveLeft))
            moveInput = -1f;

        if (Input.GetKey(moveRight))
            moveInput = 1f;

        // FLIP PLAYER SPRITE
        if (spriteRenderer != null)
        {
            if (moveInput < 0)
                spriteRenderer.flipX = true;

            else if (moveInput > 0)
                spriteRenderer.flipX = false;
        }

        // JUMP INPUT
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            jumpPressed = true;
        }

        // SLIDE INPUT
        isSliding = isGrounded &&
            (Input.GetKey(slideKey) ||
             Input.GetKey(KeyCode.DownArrow));

        // UPDATE SLIDING COLLIDER
        UpdateSlideCollider();

        // UPDATE ANIMATIONS
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // HORIZONTAL MOVEMENT
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // JUMP
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
            groundColliders.Clear();
        }

        jumpPressed = false;
    }

    void UpdateSlideCollider()
    {
        if (isSliding)
        {
            float newHeight =
                originalColliderSize.y * slideHeight;

            playerCollider.size = new Vector2(
                originalColliderSize.x,
                newHeight
            );

            // Keep the player's feet in the same position.
            playerCollider.offset = new Vector2(
                originalColliderOffset.x,
                originalColliderOffset.y -
                (originalColliderSize.y - newHeight) / 2f
            );
        }
        else
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null)
            return;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsSliding", isSliding);
        animator.SetBool("IsMoving", moveInput != 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        groundColliders.Remove(collision.collider);

        isGrounded = groundColliders.Count > 0;
    }

    void CheckGround(Collision2D collision)
    {
        bool touchingGround = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                touchingGround = true;
                break;
            }
        }

        if (touchingGround)
            groundColliders.Add(collision.collider);
        else
            groundColliders.Remove(collision.collider);

        isGrounded = groundColliders.Count > 0;
    }

    void OnDrawGizmosSelected()
    {
        if (playerCollider == null)
            return;

        Gizmos.color = Color.green;

        Bounds bounds = playerCollider.bounds;

        Vector3 checkPosition = new Vector3(
            bounds.center.x,
            bounds.min.y - groundCheckDistance,
            bounds.center.z
        );

        Gizmos.DrawWireSphere(checkPosition, 0.05f);
    }
}
