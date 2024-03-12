using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovementController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappeable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrapplineDistance;

    public float grappleDelayTime;
    public float overshootYAxis;

    public Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    // Grab a reference to a rigidbody on the player
    Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<PlayerMovementController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

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
        // Check if the grappling cooldown timer is still ticking
        if (grapplingCdTimer > 0) return;

        grappling = true;
        
        pm.freeze = true;

        RaycastHit hit;
        // If the grapple hit a valid point
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplineDistance, whatIsGrappeable))
        {
            // Set the grapple point to be that targeted area
            grapplePoint = hit.point;
            // Call the next grapple function
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            // Else, set grapple point to be the cam position + grapple distance
            grapplePoint = cam.position + cam.forward * maxGrapplineDistance;
            // Prevent grapple
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // Raycast line renderer
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        // Calculate the lowest point of grapple position
        Vector3 lowestPoint = new(transform.position.x, transform.position.y - 1f, transform.position.z);
        // Calculate highest position
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        // Calculate curve of positions
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        // Check if the highest point is negative, and set the curve to the highest point
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1.0f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
