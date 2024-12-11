using UnityEngine;

public class BlueMovement : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Rigidbody playerRigidbody;
    public float movementMultiplier = 5f;
    public float maxVelocity = 10f;
    public float dashForce = 20f;
    public float dashCooldown = 2f;
    public float jumpForce = 10f;
    public float climbMultiplier = 1.5f;
    public float wallRunForce = 15f;
    public float wallRunDuration = 2f;
    public float slideForce = 25f;
    public float slideCooldown = 3f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Vector3 leftHandLastPosition;
    private Vector3 rightHandLastPosition;
    private bool canDash = true;
    private bool isClimbing = false;
    private bool isWallRunning = false;
    private bool canSlide = true;

    private void Start()
    {
        leftHandLastPosition = leftHand.position;
        rightHandLastPosition = rightHand.position;
    }

    private void FixedUpdate()
    {
        HandleHandMovement();
        LimitVelocity();
    }

    private void Update()
    {
        HandleDashInput();
        HandleJumpInput();
        HandleWallRunInput();
        HandleSlideInput();
    }

    private void HandleHandMovement()
    {
        Vector3 leftHandVelocity = (leftHand.position - leftHandLastPosition) / Time.fixedDeltaTime;
        Vector3 rightHandVelocity = (rightHand.position - rightHandLastPosition) / Time.fixedDeltaTime;

        leftHandLastPosition = leftHand.position;
        rightHandLastPosition = rightHand.position;

        if (IsHandGrounded(leftHand))
        {
            ApplyMovement(leftHandVelocity);
        }
        if (IsHandGrounded(rightHand))
        {
            ApplyMovement(rightHandVelocity);
        }

        if (IsHandGrounded(leftHand) && IsHandGrounded(rightHand))
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    private void HandleDashInput()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            PerformDash();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.J) && IsGrounded())
        {
            PerformJump();
        }
    }

    private void HandleWallRunInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && IsNearWall())
        {
            StartWallRun();
        }
    }

    private void HandleSlideInput()
    {
        if (canSlide && Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded())
        {
            PerformSlide();
        }
    }

    private bool IsHandGrounded(Transform hand)
    {
        return Physics.CheckSphere(hand.position, 0.1f, groundLayer);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.5f, groundLayer);
    }

    private bool IsNearWall()
    {
        return Physics.CheckSphere(transform.position, 0.5f, wallLayer);
    }

    private void ApplyMovement(Vector3 handVelocity)
    {
        float multiplier = isClimbing ? climbMultiplier : 1f;
        playerRigidbody.AddForce(-handVelocity * movementMultiplier * multiplier, ForceMode.Impulse);
    }

    private void PerformDash()
    {
        Vector3 dashDirection = CalculateDashDirection();
        playerRigidbody.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        canDash = false;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void PerformJump()
    {
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void StartWallRun()
    {
        if (!isWallRunning)
        {
            isWallRunning = true;
            playerRigidbody.useGravity = false;
            playerRigidbody.AddForce(Vector3.up * wallRunForce, ForceMode.Impulse);
            Invoke(nameof(StopWallRun), wallRunDuration);
        }
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        playerRigidbody.useGravity = true;
    }

    private void PerformSlide()
    {
        Vector3 slideDirection = playerRigidbody.velocity.normalized;
        playerRigidbody.AddForce(slideDirection * slideForce, ForceMode.Impulse);
        canSlide = false;
        Invoke(nameof(ResetSlide), slideCooldown);
    }

    private void ResetSlide()
    {
        canSlide = true;
    }

    private Vector3 CalculateDashDirection()
    {
        Vector3 handDifference = leftHand.position - rightHand.position;
        return handDifference.normalized;
    }

    private void LimitVelocity()
    {
        if (playerRigidbody.velocity.magnitude > maxVelocity)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
        }
    }

    private void ResetDash()
    {
        canDash = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftHand.position, 0.1f);
        Gizmos.DrawWireSphere(rightHand.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
