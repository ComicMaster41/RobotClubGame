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

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    // Grab a reference to a rigidbody on the player
    public Rigidbody rb;

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
        // Enable kinematic on rigidbody
        rb.isKinematic = true;

        if (grapplingCdTimer > 0) return;

        grappling = true;
        
        pm.freeze = true;

        // Raycast creation
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrapplineDistance, whatIsGrappeable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrapplineDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // Raycast line renderer
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

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

        // Disable kinematic on rigidbody
        rb.isKinematic = false;
    }
}
