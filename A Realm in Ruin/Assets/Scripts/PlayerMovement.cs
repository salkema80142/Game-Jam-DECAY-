using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Controls")]
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;

    private float moveInput;
    private bool jumpPressed;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement input
        moveInput = 0f;

        if (Input.GetKey(moveLeft))
            moveInput = -1f;

        if (Input.GetKey(moveRight))
            moveInput = 1f;

        // Jump input
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            jumpPressed = true;
        }
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // Apply jumping
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
        isGrounded = false;
    }

    void CheckGround(Collision2D collision)
    {
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
