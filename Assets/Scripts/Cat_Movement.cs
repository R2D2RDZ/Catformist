using System.Collections;
using UnityEngine;

public class Cat_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float frictionDivisor;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float gravityForce;

    [Header("Jump Rotation")]
    [SerializeField] private float jumpVelocityToRotateX;
    [SerializeField] private float[] jumpRotationAngles;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRayDis; // Kept for wall check
    [SerializeField] private Vector3 groundCheckBoxSize = new Vector3(0.5f, 0.5f);
    [SerializeField] private float groundCheckDistance = 0.5f;
    [HideInInspector] public bool isOnGround; // Se usa en Cat_Animations.cs

    [Header("Climbing")]
    [SerializeField] private float climbMoveSpeedVer;
    [SerializeField] private float climbMoveSpeedHor;
    [SerializeField] private float climbRayDis;
    [SerializeField] private float limitator;
    [SerializeField] private float climbJumpVelHor;
    [SerializeField] private Vector3 climbBottomBoxSize = new Vector3(0.5f, 0.5f);
    [SerializeField] private float climbBottomDistance = 0.5f;

    [Header("Gatocidad")]
    private Gatocidad gatocidad;
    [SerializeField] private float gatocidadClimb;
    [SerializeField] private float gatocidadJump;
    [SerializeField] private float gatocidadInfluence;

    private bool isHittingWall;
    private bool canCheckToClimb;
    private bool isGroundedOnWall;

    [HideInInspector] public bool isClimbing; // Se usa en Cat_Animations.cs
    [HideInInspector] public bool stateStunned;

    [HideInInspector] public Rigidbody rb;
    private Vector3 velocity;

    [HideInInspector] public Vector3 direction2D;

    private Vector3 setDirection3D;
    private Vector3 direction3D;

    private Vector3 wallNormal;
    private bool climbRotationLock;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gatocidad = GetComponent<Gatocidad>();
        canCheckToClimb = true;
    }

    private void FixedUpdate()
    {
        direction3D = new Vector3(direction2D.x, 0f, direction2D.y);

        if (direction3D.magnitude >= 0.1f)
        {
            setDirection3D = direction3D;
        }

        velocity = rb.linearVelocity;

        if (!isClimbing)
        {
            Upt_Move();
            Upt_Rotate();
            Upt_Gravity();
        }
        else if (gatocidad.GetGatocidad() >= gatocidadClimb)
        {
            Upt_ClimbMove();
        } 
        else { StopClimbing(); }

        Upt_GroundCheck();
        Upt_CheckIfCanClimb();
    }

    private void Upt_Move()
    {
        rb.AddForce(direction3D * moveSpeed * Mathf.Max(gatocidad.GetGatocidad() / gatocidadInfluence, 0.5f));

        rb.linearVelocity = new Vector3(
            velocity.x / frictionDivisor,
            velocity.y,
            velocity.z / frictionDivisor
        );
    }

    private void Upt_Rotate()
    {
        if (setDirection3D.sqrMagnitude < 0.01f)
            return;

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
        targetRotation *= tiltRotation;

        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);
    }

    private void Upt_Gravity()
    {
        rb.AddForce(Vector3.down * gravityForce);
    }

    private void Upt_GroundCheck()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);

        int groundLayer = LayerMask.NameToLayer("Ground");
        int groundMask = 1 << groundLayer;

        if (!isClimbing)
        {
            isOnGround = Physics.BoxCast(
                transform.position,
                climbBottomBoxSize,
                Vector3.down,
                transform.rotation,
                groundCheckDistance,
                groundMask
            );
        }
        else
        {
            Ray rayGroundedOnWall = new Ray(transform.position, -transform.up);

            isGroundedOnWall = Physics.Raycast(
                rayGroundedOnWall,
                out RaycastHit hit,
                groundCheckRayDis,
                groundMask
            );

            // No detecta más pared bajo sus pies
            if (!isGroundedOnWall)
            {
                StopClimbing();
            }

            // Si llega al suelo arrastrándose por la pared
            bool hitsBottom = Physics.BoxCast(
                transform.position,
                climbBottomBoxSize,
                Vector3.down,
                transform.rotation,
                climbBottomDistance,
                groundMask
            );

            if (hitsBottom)
            {
                StartCoroutine(StopCheckingIfCanClimb());
            }
        }
    }

    private void Upt_ClimbMove()
    {
        Vector3 climbUpVector = transform.forward * climbMoveSpeedVer;

        if (direction3D.magnitude < 0.1f || direction3D.z < 0f)
        {
            gatocidad.UseGatocidad(gatocidadClimb);
            climbUpVector = -transform.forward * climbMoveSpeedVer;
        }

        rb.AddForce(climbUpVector + direction2D.x * transform.right * climbMoveSpeedHor);
        rb.AddForce(-rb.linearVelocity * limitator);

        // Gravedad relativa a la pared
        rb.AddForce(-transform.up * gravityForce);
    }

    private void Upt_CheckIfCanClimb()
    {
        Vector3 rayDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

        if (rayDirection.sqrMagnitude < 0.01f)
            return;

        Ray ray = new Ray(transform.position, rayDirection);

        int groundLayer = LayerMask.NameToLayer("Ground");
        int groundMask = 1 << groundLayer;

        isHittingWall = Physics.Raycast(ray, out RaycastHit hit, climbRayDis, groundMask);

        if (isHittingWall && !isOnGround && canCheckToClimb)
        {
            wallNormal = hit.normal;

            if (!climbRotationLock)
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

    private void OnDrawGizmos()
    {
        int groundMask = LayerMask.GetMask("Ground");

        if (!isClimbing)
        {
            // 1. WALL CHECK (Detección de pared)
            Gizmos.color = isHittingWall ? Color.green : Color.red;
            Vector3 rayDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Gizmos.DrawRay(transform.position, rayDirection * climbRayDis);

            // 2. NORMAL GROUND CHECK (isOnGround)
            Gizmos.color = isOnGround ? Color.green : Color.red;

            // El tamaño total de la caja del Gizmo debe ser el doble de los halfExtents del BoxCast
            Vector3 boxSize = new Vector3(groundCheckBoxSize.x * 2f, 0.5f, groundCheckBoxSize.y * 2f);

            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Caja en la posición inicial del gato
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Caja en la posición final a la que llega el barrido
            Vector3 endPosition = transform.position + Vector3.down * groundCheckDistance;
            Gizmos.matrix = Matrix4x4.TRS(endPosition, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Línea central que conecta el inicio y el fin del barrido
            Gizmos.matrix = oldMatrix;
            Gizmos.DrawLine(transform.position, endPosition);
        }
        else
        {
            // 3. GROUNDED ON WALL CHECK (Permanencia en pared)
            Gizmos.color = isGroundedOnWall ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, -transform.up * groundCheckRayDis);

            // 4. BOTTOM GROUND CHECK (hitsBottom)
            Gizmos.color = Color.yellow;

            Vector3 climbBoxSize = new Vector3(climbBottomBoxSize.x * 2f, 0.5f, climbBottomBoxSize.y * 2f);

            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Caja inicial
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, climbBoxSize);

            // Caja final del barrido hacia abajo
            Vector3 endPositionClimb = transform.position + Vector3.down * climbBottomDistance;
            Gizmos.matrix = Matrix4x4.TRS(endPositionClimb, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, climbBoxSize);

            Gizmos.matrix = oldMatrix;
            Gizmos.DrawLine(transform.position, endPositionClimb);
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        climbRotationLock = false;
    }

    public void Jump()
    {
        if (gatocidad.GetGatocidad() >= gatocidadJump)
        {    
            
            if (isOnGround && !isClimbing)
            {
                gatocidad.UseGatocidad(gatocidadJump);
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    jumpVelocity,
                    rb.linearVelocity.z
                );
            }
            else if (isClimbing)
            {
                gatocidad.UseGatocidad(gatocidadJump);
                Vector3 transformUp = transform.up;

                StartCoroutine(StopCheckingIfCanClimb());

                rb.linearVelocity = new Vector3(
                    transformUp.x * climbJumpVelHor,
                    jumpVelocity,
                    transformUp.z * climbJumpVelHor
                );
            }
        }
    }

    private IEnumerator StopCheckingIfCanClimb()
    {
        canCheckToClimb = false;
        StopClimbing();

        yield return new WaitForSeconds(0.25f);

        canCheckToClimb = true;
    }

    public void Stun(float stunDuration)
    {
        if (!stateStunned)
        {
            StartCoroutine(StunRoutine(stunDuration));
        }
    }

    private IEnumerator StunRoutine(float duration)
    {
        stateStunned = true;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        direction2D = Vector2.zero;
        direction3D = Vector3.zero;
        
        yield return new WaitForSeconds(duration);
        
        stateStunned = false;
    }
}
