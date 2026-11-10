
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D playerCollider;

    public float jumpForce = 12f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool isSliding;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();

        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
    }

    void Update()
    {
        // Check if the player is touching the ground
        isGrounded = playerCollider.IsTouchingLayers(groundLayer);

        if (Keyboard.current == null) return;

        // Jump with Space
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // Slide with Down Arrow or S
        isSliding = isGrounded &&
            (Keyboard.current.downArrowKey.isPressed ||
             Keyboard.current.sKey.isPressed);

        // Change collider size while sliding
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

        // Update animations
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsSliding", isSliding);
    }
}
