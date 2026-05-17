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
            // Calculamos la posición exacta del centro de la caja (igual que el Gizmo)
            Vector3 center = transform.position - Vector3.up * groundCheckDistance;

            // Convertimos el tamaño del inspector en la mitad (halfExtents) para la física
            Vector3 halfExtents = new Vector3(groundCheckBoxSize.x / 2f, 0.25f, groundCheckBoxSize.y / 2f);

            // CheckBox no sufre del bug de los bordes/esquinas al estar en contacto continuo
            isOnGround = Physics.CheckBox(center, halfExtents, transform.rotation, groundMask);
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

            // Si llega al suelo arrastrándose por la pared (Usando CheckBox)
            Vector3 climbCenter = transform.position - Vector3.up * climbBottomDistance;
            Vector3 climbHalfExtents = new Vector3(climbBottomBoxSize.x / 2f, 0.25f, climbBottomBoxSize.y / 2f);

            bool hitsBottom = Physics.CheckBox(climbCenter, climbHalfExtents, transform.rotation, groundMask);

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
        if (!isClimbing)
        {
            // Wall Check
            Gizmos.color = isHittingWall ? Color.green : Color.red;
            Vector3 rayDirection = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            Gizmos.DrawRay(transform.position, rayDirection * climbRayDis);

            // Normal Ground Check Box
            Gizmos.color = isOnGround ? Color.green : Color.red;
            Vector3 center = transform.position - Vector3.up * groundCheckDistance;
            Vector3 size = new Vector3(groundCheckBoxSize.x, 0.5f, groundCheckBoxSize.y);

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = oldMatrix;
        }
        else
        {
            // Grounded on Wall Check
            Gizmos.color = isGroundedOnWall ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, -transform.up * groundCheckRayDis);

            // Bottom Ground Check Box
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position - Vector3.up * climbBottomDistance;
            Vector3 size = new Vector3(climbBottomBoxSize.x, 0.5f, climbBottomBoxSize.y);

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = oldMatrix;
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
