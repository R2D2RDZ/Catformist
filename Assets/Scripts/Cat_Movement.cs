using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Cat_Movement : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed, frictionDivisor, jumpVelocity, gravityForce;
    [SerializeField] private float jumpVelocityToRotateX;
    [SerializeField] private float[] jumpRotationAngles;
    [SerializeField] private float groundCheckRayDis;
    private bool isOnGround;

    [Header("Climbing")]
    [SerializeField] private float climbMoveSpeedVer, climbMoveSpeedHor, climbRayDis, limitator;
    private bool isHittingWall, isClimbing;



    [HideInInspector] public bool stateStunned;

    private Rigidbody rb;
    private Vector3 velocity;

    [HideInInspector] public Vector3 direction2D;
    private Vector3 setDirection3D, direction3D;

    private Vector3 wallNormal;
    private bool climbRotationLock;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        direction3D = new Vector3(direction2D.x, 0, direction2D.y);
        if (direction3D.magnitude >= 0.1f)
        {
            setDirection3D = direction3D;
        }

        velocity = rb.linearVelocity;

        if (isClimbing == false)
        {
            BH_Move();
            BH_Rotate();
            BH_Gravity();
        }
        else
        { BH_ClimbMove(); }
        BH_GroundCheck();
        BH_CheckIfCanClimb();
    }


    private void BH_Move()
    {
        rb.AddForce(direction3D * moveSpeed);

        
        rb.linearVelocity = new Vector3((velocity.x / frictionDivisor), velocity.y, (velocity.z / frictionDivisor));
    }

    private void BH_Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(setDirection3D.normalized);


        float xAngle = 0f;

        if (rb.linearVelocity.y > jumpVelocityToRotateX)
        {
            xAngle = jumpRotationAngles[0];
        }
        else if (rb.linearVelocity.y < -jumpVelocityToRotateX)
        {
            xAngle = jumpRotationAngles[1];
        }

        Quaternion tiltRotation = Quaternion.Euler(xAngle, 0f, 0f);

        // Combina la rotación hacia la dirección con la inclinación en X
        targetRotation = targetRotation * tiltRotation;

        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);
    }

    private void BH_Gravity()
    {
        rb.AddForce(Vector3.down * gravityForce);
    }

    private void BH_GroundCheck()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);


        int wallLayer = LayerMask.NameToLayer("Ground");
        int wallMask = 1 << wallLayer;

        if (isClimbing == false)
        {
            

            isOnGround = Physics.Raycast(ray, out RaycastHit hit, groundCheckRayDis, wallMask);

            if (isOnGround)
            {
                // puede escalar si está en el cielo
            }
        }
        
    }

    private void BH_ClimbMove()
    {
        Vector3 wallRight = transform.right;
        Vector3 wallUp = transform.forward;

        rb.AddForce((direction2D.y * transform.forward * climbMoveSpeedVer) + (direction2D.x * transform.right * climbMoveSpeedHor));
        rb.AddForce(-rb.linearVelocity * limitator);
        rb.AddForce(-transform.up * gravityForce);

    }

    private void BH_CheckIfCanClimb()
    {
        Ray ray = new Ray(transform.position, new Vector3(transform.forward.x, 0 , transform.forward.z));


        int wallLayer = LayerMask.NameToLayer("Ground");
        int wallMask = 1 << wallLayer;

        isHittingWall = Physics.Raycast(ray, out RaycastHit hit, climbRayDis, wallMask);

        if(isHittingWall && !isOnGround)
        {
            wallNormal = hit.normal;

            if (climbRotationLock == false)
            {
                Vector3 forwardOnWall = Vector3.ProjectOnPlane(Vector3.up, wallNormal).normalized;

                // Rotación final:
                // forward = hacia arriba en la pared
                // up = normal de la pared
                Quaternion targetRotation = Quaternion.LookRotation(forwardOnWall, wallNormal);

                transform.rotation = targetRotation;
                climbRotationLock = true;
            }

            isClimbing = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isHittingWall ? Color.green : Color.red;

        Gizmos.DrawRay(transform.position, new Vector3(transform.forward.x, 0, transform.forward.z) * climbRayDis);

        Gizmos.color = isOnGround ? Color.green : Color.red;

        Gizmos.DrawRay(transform.position, -Vector3.up * groundCheckRayDis);
    }



    public void Jump()
    {
        if(isOnGround)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
    }



    public void Stun(float stunDuration)
    {

    }
}
