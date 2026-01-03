using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float airControlMultiplier = 0.7f;

    public float jumpForce = 14f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private float prevVerticalInput;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool jumpPressed = verticalInput > 0 && prevVerticalInput <= 0;
        bool jumpHeld = verticalInput > 0;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );


        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        { 
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }


        if (!jumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; 
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float control = isGrounded ? 1f : airControlMultiplier;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed * control, rb.linearVelocity.y);

        if ((isFacingRight && moveInput < 0) || (!isFacingRight && moveInput > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 currentScale = transform.localScale;
            currentScale.x *= -1;
            transform.localScale = currentScale;
        }
    }
}
