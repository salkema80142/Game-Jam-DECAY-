
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool isSliding;
    private bool jumpPressed;

    private float moveInput;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // LEFT AND RIGHT MOVEMENT
        moveInput = 0f;

        if (Keyboard.current.aKey.isPressed)
            moveInput = -1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput = 1f;

        // FLIP PLAYER DIRECTION
        if (moveInput < 0)
            spriteRenderer.flipX = true;
        else if (moveInput > 0)
            spriteRenderer.flipX = false;

        // JUMP
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            jumpPressed = true;
        }

        // SLIDE
        isSliding = isGrounded &&
            (Keyboard.current.sKey.isPressed ||
             Keyboard.current.downArrowKey.isPressed);

        // SLIDING COLLIDER
        if (isSliding)
        {
            playerCollider.size = new Vector2(
                originalColliderSize.x,
                originalColliderSize.y * 0.5f
            );

            playerCollider.offset = new Vector2(
                originalColliderOffset.x,
                originalColliderOffset.y - originalColliderSize.y * 0.25f
            );
        }
        else
        {
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }

        // UPDATE ANIMATIONS
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsSliding", isSliding);
        animator.SetBool("IsMoving", moveInput != 0);
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
        }

        jumpPressed = false;
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
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0)
        {
            isGrounded = false;
        }
    }

    void CheckGround(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) == 0)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }
}
