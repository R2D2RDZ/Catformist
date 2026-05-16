using UnityEngine;

public class Cat_Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed, rotationSpeed, frictionDivisor, jumpForce, gravityForce;
    [SerializeField] private float jumpVelocityToRotateX;
    [SerializeField] private float[] jumpRotationAngles;


    [HideInInspector] public bool stateStunned;

    private Rigidbody rb;

    [HideInInspector] public Vector3 direction2D;
    private Vector3 setDirection3D;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction3D = new Vector3(direction2D.x, 0, direction2D.y);
        if (direction3D.magnitude >= 0.1f)
        {
            setDirection3D = direction3D;
        }

        rb.AddForce(direction3D * moveSpeed);

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



    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
    }



    public void Stun(float stunDuration)
    {

    }
}
