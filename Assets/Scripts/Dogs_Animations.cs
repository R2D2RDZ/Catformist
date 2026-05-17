using UnityEngine;

public class Dogs_Animations : MonoBehaviour
{
    [Range(1, 3)]
    [SerializeField] private int dogLevel; // Se crea en void Start, no manipular en Inspector
    [SerializeField] private float speedMultiplier;

    [SerializeField] private Animator dogAnimator;

    [Header("Just for Testing")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed, rotationSpeed, gravityForce;
    [SerializeField] private float frictionDivisor;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dogAnimator.SetInteger("DogLvl", dogLevel);
    }

    void FixedUpdate()
    {
        if(rb != null)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            dogAnimator.SetFloat("Speed", horizontalVelocity.magnitude * speedMultiplier);


            if(target != null)
            {
                // Seguir al target
                Vector3 dogTargetDirection = (target.position - transform.position);
                dogTargetDirection.y = 0;
                rb.AddForce(dogTargetDirection.normalized * moveSpeed);


                // Rotar hacia el target
                if (dogTargetDirection.magnitude >= 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dogTargetDirection.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
                }
            }


            // Gravedad porque se la quité al Rigidbody
            rb.AddForce(Vector3.down * gravityForce);

            // Frenar porque no debe tener fricción el collider
            Vector3 velocity = rb.linearVelocity;
            rb.linearVelocity = new Vector3((velocity.x / frictionDivisor), velocity.y, (velocity.z / frictionDivisor));


        }
    }
}
