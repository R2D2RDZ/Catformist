using UnityEngine;

public class Cat_Animations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float groundMoveMult;

    private Rigidbody rb;
    private Cat_Movement catMovementScr;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        catMovementScr = GetComponent<Cat_Movement>();
    }

    void FixedUpdate()
    {
        if(rb != null)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if(catMovementScr.isClimbing == false)
            {
                animator.SetFloat("Speed", horizontalVelocity.magnitude * groundMoveMult);

                if (catMovementScr.isOnGround == false)
                {
                    animator.SetInteger("State", rb.linearVelocity.y > 0 ? 1 : 2);
                }
                else
                {
                    animator.SetInteger("State", 0);
                }

            }
        }
    }
}
