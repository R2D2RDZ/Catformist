using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static UI_Cats;

public class Cat_Animations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float groundMoveMult;
    [SerializeField] private float secondsSlashToNormal;

    private Rigidbody rb;
    private Cat_Movement catMovementScr;


    [Header("KnockOut")]
    [SerializeField] private CapsuleCollider mainCollider;
    [SerializeField] private Collider[] koColliders;
    [SerializeField] private Rigidbody[] koRbs;
    [SerializeField] private ConfigurableJoint[] looseJoints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        catMovementScr = GetComponent<Cat_Movement>();

        KnockOut(false);
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
            else
            {
                animator.SetInteger("State", 3);
            }
        }
    }

    public IEnumerator PlaySlashAnimation()
    {
        animator.SetBool("Slash", true);
        yield return new WaitForSeconds(secondsSlashToNormal);
        animator.SetBool("Slash", false);
    }





    // Ragdoll

    public void KnockOut(bool isStunned)
    {

        // Revivir
        if (isStunned == false)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            animator.enabled = true;

            foreach (Collider col in koColliders)
            {
                col.enabled = false;
            }
            mainCollider.enabled = true;

            foreach (Rigidbody indRB in koRbs)
            {
                indRB.isKinematic = true;
            }

            foreach (ConfigurableJoint cj in looseJoints)
            {
                cj.xMotion = ConfigurableJointMotion.Free;
                cj.yMotion = ConfigurableJointMotion.Free;
                cj.zMotion = ConfigurableJointMotion.Free;
            }

            StartCoroutine(BackToNormal());
        }
        // Noquear
        else
        {
            rb.constraints = RigidbodyConstraints.None;

            animator.enabled = false;

            mainCollider.enabled = false;
            foreach (Collider col in koColliders)
            {
                col.enabled = true;
            }

            foreach (Rigidbody indRB in koRbs)
            {
                indRB.isKinematic = false;
            }

            foreach (ConfigurableJoint cj in looseJoints)
            {
                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;
            }
        }
    }


    public IEnumerator BackToNormal()
    {
        animator.SetBool("BackToNormal", true);

        yield return new WaitForSeconds(0.25f);

        animator.SetBool("BackToNormal", false);

    }
}
