using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControlMultiplier = 0.7f;
    [Space(5)]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [Space(5)]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 2;
    [SerializeField] private LayerMask interactionLayer;
    [Space(5)]
    [SerializeField] private float rotationDuration;

    [HideInInspector] public bool canPickUpBox;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool isFacingRight = true;

    GameObject boxToCarry;
    GameObject mirrorToRotate;

    Rigidbody2D rb;
    float moveInput;
    float prevVerticalInput;

    float coyoteTimeCounter;
    float jumpBufferCounter;
    bool isGrounded;

    bool isInteractingWithMirror;
    bool mirrorIsRotating;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        bool jumpPressed = verticalInput > 0 && prevVerticalInput <= 0 && canMove;
        bool jumpHeld = verticalInput > 0 && canMove;

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interaction();
        }

        if (isInteractingWithMirror && !mirrorIsRotating)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(RotateMirror(-90));
            } 
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(RotateMirror(90));
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove || isInteractingWithMirror) return;

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

    void Interaction()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, interactionRadius, Vector2.zero, 0, interactionLayer);

        if (!hit || !canMove) return;

        if (hit.collider.CompareTag("Lever"))
        {
            hit.collider.GetComponent<ActivationManager>().Interaction();
            hit.collider.GetComponent<LeverManager>().Interaction();
        } else if (hit.collider.CompareTag("Mirror"))
        {
            if (isInteractingWithMirror) mirrorToRotate = null;
            else mirrorToRotate = hit.collider.gameObject;

            isInteractingWithMirror = !isInteractingWithMirror;
            moveInput = 0;
            rb.linearVelocity = Vector2.zero;
        }
        else if (canPickUpBox && hit.collider.CompareTag("Box") && boxToCarry == null && hit.collider.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
        {
            boxToCarry = hit.collider.gameObject;
            boxToCarry.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            boxToCarry.GetComponent<Collider2D>().enabled = false;
            boxToCarry.transform.parent = transform;
            boxToCarry.transform.position = transform.position + new Vector3(1.25f * ((isFacingRight) ? 1 : -1), 0, 0);
        }
        else if (boxToCarry != null)
        {
            Vector3 checkPos = (transform.position + boxToCarry.transform.position) / 2f;
            float distanceBtwBoxAndPlayer = Vector3.Distance(transform.position, boxToCarry.transform.position);
            Vector3 checkSize = new Vector3(distanceBtwBoxAndPlayer, 1, 0);
            bool canPlaceBox = Physics2D.OverlapBoxAll(checkPos, checkSize, 0, groundLayer).Length <= 0;

            if (!canPlaceBox) return;

            boxToCarry.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            boxToCarry.GetComponent<Collider2D>().enabled = true;
            boxToCarry.transform.parent = null;
            boxToCarry = null;
        }
    }

    IEnumerator RotateMirror(float angleToRotate)
    {
        mirrorIsRotating = true;

        Transform mirrorTransform = mirrorToRotate.transform;
        float startAngle = mirrorTransform.eulerAngles.z;
        float endAngle = mirrorTransform.eulerAngles.z + angleToRotate;
        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / rotationDuration;

            float currentAngle = Mathf.LerpAngle(startAngle, endAngle, t);
            mirrorTransform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        mirrorTransform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        mirrorIsRotating = false;
    }
}
