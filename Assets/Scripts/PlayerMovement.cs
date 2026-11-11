
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator bunnyAnimator;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private float moveInput;
    private bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        // Find Animator on BunnyVisual
        if (bunnyAnimator == null)
        {
            bunnyAnimator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        moveInput = 0f;

        // Keyboard movement
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                moveInput = -1f;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                moveInput = 1f;
            }

            // Keyboard jump
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }

        // Mobile joystick and jump
        if (Gamepad.current != null)
        {
            float joystickX =
                Gamepad.current.leftStick.ReadValue().x;

            if (Mathf.Abs(joystickX) > 0.1f)
            {
                moveInput = joystickX;
            }

            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }
    }

    void FixedUpdate()
    {
        // Horizontal movement
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // Check whether player touches ground
        bool isGrounded =
            playerCollider.IsTouchingLayers(groundLayer);

        // Jump only when grounded
        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            // Start jump animation immediately
            isGrounded = false;
        }

        // Update animation
        if (bunnyAnimator != null)
        {
            bunnyAnimator.SetBool("IsGrounded", isGrounded);
        }

        jumpRequested = false;
    }
}
