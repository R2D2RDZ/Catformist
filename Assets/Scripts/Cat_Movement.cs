using UnityEngine;

public class Cat_Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed, rotationSpeed, frictionDivisor, jumpForce, gravityForce;
    [SerializeField] private float jumpVelocityToRotateX;
    [SerializeField] private float[] jumpRotationAngles;
<<<<<<< Updated upstream
=======
    [SerializeField] private float groundCheckRayDis;
    private bool isOnGround;

    [Header("Climbing")]
    [SerializeField] private float climbMoveSpeedVer;
    [SerializeField] private float climbMoveSpeedHor, climbRayDis, limitator, climbJumpVelHor;

    [Header("Gatocidad")]
    [SerializeField] private float gatocidadInfluence;
    [SerializeField] private float gatocidadJump;
    [SerializeField] private float gatocidadClimb;
    [SerializeField] private float gatocidadSlash;

    private bool isHittingWall, isClimbing, canCheckToClimb;

>>>>>>> Stashed changes


    [HideInInspector] public bool stateStunned;

    private Rigidbody rb;

    [HideInInspector] public Vector3 direction2D;
    private Vector3 setDirection3D;

    private Gatocidad gatocidad;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
<<<<<<< Updated upstream
=======
        gatocidad = GetComponent<Gatocidad>();
        canCheckToClimb = true;
>>>>>>> Stashed changes
    }

    void FixedUpdate()
    {
        Vector3 direction3D = new Vector3(direction2D.x, 0, direction2D.y);
        if (direction3D.magnitude >= 0.1f)
        {
            setDirection3D = direction3D;
        }

<<<<<<< Updated upstream
        rb.AddForce(direction3D * moveSpeed);
=======
        velocity = rb.linearVelocity;

        if (isClimbing == false)
        {
            Upt_Move();
            Upt_Rotate();
            Upt_Gravity();
        }
        else if(gatocidad.GetGatocidad() > gatocidadClimb)
            { Upt_ClimbMove(); }
        else {StopClimbing(); }
        Upt_GroundCheck();
        Upt_CheckIfCanClimb();
    }


    private void Upt_Move()
    {
        rb.AddForce(direction3D * moveSpeed * (gatocidad.GetGatocidad()/gatocidadInfluence));
>>>>>>> Stashed changes

        Vector3 velocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3((velocity.x / frictionDivisor), velocity.y, (velocity.z / frictionDivisor));


        Quaternion targetRotation = Quaternion.LookRotation(setDirection3D.normalized);


        float xAngle = 0f;

        if (velocity.y > jumpVelocityToRotateX)
        {
            xAngle = jumpRotationAngles[0];
        }
        else if (velocity.y < -jumpVelocityToRotateX)
        {
            xAngle = jumpRotationAngles[1];
        }

        Quaternion tiltRotation = Quaternion.Euler(xAngle, 0f, 0f);

        // Combina la rotación hacia la dirección con la inclinación en X
        targetRotation = targetRotation * tiltRotation;

        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);

        rb.AddForce(Vector3.down * gravityForce);
    }

<<<<<<< Updated upstream
=======
    private void Upt_GroundCheck()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);


        int wallLayer = LayerMask.NameToLayer("Ground");
        int wallMask = 1 << wallLayer;

        if (isClimbing == false)
        {
            isOnGround = Physics.Raycast(ray, out RaycastHit hit, groundCheckRayDis, wallMask);
        }
        else
        {
            Ray rayGroundedOnWall = new Ray(transform.position, -transform.up);

            bool isGroundedOnWall = Physics.Raycast(rayGroundedOnWall, out RaycastHit hit, groundCheckRayDis, wallMask);

            // No detecta más pared bajo sus pies
            if (isGroundedOnWall == false)
            { StopClimbing(); }



            // Si llega al suelo arrastrandose por la pared
            bool hitsBottom = Physics.Raycast(ray, out RaycastHit hitBottom, groundCheckRayDis, wallMask);
            if (hitsBottom)
            {
                StopClimbing();
            }

        }

    }

    private void Upt_ClimbMove()
    {
        Vector3 wallRight = transform.right;
        Vector3 wallUp = transform.forward;

        Vector3 climbUpVector = transform.forward * climbMoveSpeedVer;
        if (direction3D.magnitude < 0.1f || direction3D.z < 0)
        {
            gatocidad.UseGatocidad(gatocidadClimb);
            climbUpVector = -transform.forward * climbMoveSpeedVer;
        }
        rb.AddForce((climbUpVector) + (direction2D.x * transform.right * climbMoveSpeedHor));
        rb.AddForce(-rb.linearVelocity * limitator);
        rb.AddForce(-transform.up * gravityForce);

    }

    private void Upt_CheckIfCanClimb()
    {
        Ray ray = new Ray(transform.position, new Vector3(transform.forward.x, 0, transform.forward.z));


        int wallLayer = LayerMask.NameToLayer("Ground");
        int wallMask = 1 << wallLayer;

        isHittingWall = Physics.Raycast(ray, out RaycastHit hit, climbRayDis, wallMask);

        if (isHittingWall && !isOnGround && canCheckToClimb)
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
        if (isClimbing == false)
        {
            // Wall Check

            Gizmos.color = isHittingWall ? Color.green : Color.red;

            Gizmos.DrawRay(transform.position, new Vector3(transform.forward.x, 0, transform.forward.z) * climbRayDis);

            // Normal Ground Check

            Gizmos.color = isOnGround ? Color.green : Color.red;

            Gizmos.DrawRay(transform.position, -Vector3.up * groundCheckRayDis);
        }
        else
        {
            // Grounded on Wall Check

            Gizmos.color = isOnGround ? Color.green : Color.red;

            Gizmos.DrawRay(transform.position, -transform.up * groundCheckRayDis);
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        climbRotationLock = false;
    }

>>>>>>> Stashed changes


    public void Jump()
    {
<<<<<<< Updated upstream
        rb.AddForce(Vector3.up * jumpForce);
=======
        if (gatocidad.GetGatocidad() > gatocidadJump)
        {
            gatocidad.UseGatocidad(gatocidadJump);
            if (isOnGround && isClimbing == false)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
            }
            else if (isClimbing == true)
            {

                Vector3 transformUp = transform.up;

                StartCoroutine(StopCheckingIfCanClimb());
                StopClimbing();
                rb.linearVelocity = new Vector3(transformUp.x * climbJumpVelHor, jumpVelocity, transformUp.z * climbJumpVelHor);

            }
        }
>>>>>>> Stashed changes
    }



    public void Stun(float stunDuration)
    {

    }
}
