using UnityEngine;

public class BlueMovement : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Rigidbody playerRigidbody;
    public float movementMultiplier = 5f;
    public float maxVelocity = 10f;
    public float dashForce = 20f;
    public LayerMask groundLayer;

    private Vector3 leftHandLastPosition;
    private Vector3 rightHandLastPosition;

    private bool canDash = true;
    public float dashCooldown = 2f;

    void Start()
    {
        leftHandLastPosition = leftHand.position;
        rightHandLastPosition = rightHand.position;
    }

    void FixedUpdate()
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

        LimitVelocity();
    }

    void Update()
    {
        if (canDash && Input.GetKeyDown(KeyCode.Space)) // Press Space to Dash
        {
            Vector3 dashDirection = (leftHand.position - rightHand.position).normalized;
            PerformDash(dashDirection);
        }
    }

    private bool IsHandGrounded(Transform hand)
    {
        return Physics.CheckSphere(hand.position, 0.1f, groundLayer);
    }

    private void ApplyMovement(Vector3 handVelocity)
    {
        playerRigidbody.AddForce(-handVelocity * movementMultiplier, ForceMode.Impulse);
    }

    private void LimitVelocity()
    {
        if (playerRigidbody.velocity.magnitude > maxVelocity)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxVelocity;
        }
    }

    private void PerformDash(Vector3 direction)
    {
        playerRigidbody.AddForce(direction * dashForce, ForceMode.Impulse);
        canDash = false;
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftHand.position, 0.1f);
        Gizmos.DrawWireSphere(rightHand.position, 0.1f);
    }
}
