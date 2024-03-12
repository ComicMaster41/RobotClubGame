using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform cam, player;
    CharacterController characterController;

    [Header("Grappling")]
    public float maxGrapplineDistance;
    public float trajectoryHeight = 5f; // Maximum height the arc will reach
    float minMovementThreshold = 0.1f; // Threshold to determine if the player is moving
    public LayerMask whatIsGrappeable;
    public Transform gunTip;
    public LineRenderer lr;
    bool grappling;
    public Vector3 grapplePoint;
    Vector3 currentVelocity;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey;

    public Transform cameraTransform; // Assign your main camera or a transform that points in the direction you want to shoot the grappling hook
    public LayerMask whatIsGrappleable; // Configure this to only include layers you can grapple to

    public float grappleSpeed = 10f; // Speed at which the player moves towards the grapple point


    private void Start()
    {
        player = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Check key and if player is moving to grapple
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grappling) GrappleMovement();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        //if (grapplingCdTimer > 0) return;

        grappling = true;
        
        RaycastHit hit;
        // If the grapple hit a valid point
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplineDistance, whatIsGrappeable))
        {
            grapplePoint = hit.point;
            grappling = true;
            currentVelocity = CalculateJumpVelocity(transform.position, grapplePoint, trajectoryHeight);
        }

        // Raycast line renderer
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    void GrappleMovement()
    {
        if (!grappling) return;

        float deltaTime = Time.deltaTime;
        characterController.Move(currentVelocity * deltaTime);
        // Apply gravity (increase vertical velocity)
        currentVelocity += Physics.gravity * deltaTime;
        // Stop grappling if close enough to the target or if moving downwards after reaching the peak
        if (Vector3.Distance(transform.position, grapplePoint) < 1f || currentVelocity.y < 0 && transform.position.y > grapplePoint.y)
        {
            grappling = false;
        }

        Invoke(nameof(StopGrapple), 1f);
    }

    // Regular calculation
    //public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    //{
    //    float gravity = Physics.gravity.y;
    //    float displacementY = endPoint.y - startPoint.y;
    //    Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

    //    Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
    //    Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
    //        + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

    //    return velocityXZ + velocityY;
    //}

    // Chat GPT caluclation
    Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float speed)
    {
        Vector3 toTarget = endPoint - startPoint;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float distanceXZ = toTargetXZ.magnitude;
        float time = distanceXZ / speed;

        // Simplify by using a direct approach for the vertical component
        float velocityY = (1 / time) * (toTarget.y + (0.5f * Mathf.Abs(Physics.gravity.y) * time * time));

        // Use the horizontal distance and time to calculate horizontal speed
        Vector3 velocityXZ = toTargetXZ.normalized * speed;

        return new Vector3(velocityXZ.x, velocityY, velocityXZ.z);
    }





    public void StopGrapple()
    {
        grappling = false;

        lr.enabled = false;

        //grapplingCdTimer = grapplingCd;
    }
}
