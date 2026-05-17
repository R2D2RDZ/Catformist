using UnityEngine;

public class Cat_Attack : MonoBehaviour
{
    [Header("Slash Settings")]
    [SerializeField] private float slashRange = 1f;
    [SerializeField] private float slashRadius = 1f;
    [SerializeField] private float stunDuration = 3f;
    [SerializeField] private float slashCost = 20f;

    private Gatocidad gatocidad;

    [SerializeField] private Cat_Animations catAnimationsScr;

    private void Start()
    {
        gatocidad = GetComponent<Gatocidad>();
    }

    public void PerformSlash()
    {
        // Try to use Gatocidad before slashing
        if (gatocidad != null && !gatocidad.UseGatocidad(slashCost))
        {
            // Not enough Gatocidad to slash
            return;
        }

        Vector3 slashCenter = transform.position + transform.forward * slashRange;
        Collider[] hitColliders = Physics.OverlapSphere(slashCenter, slashRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Make sure we only hit other players
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject)
            {
                Cat_Movement targetMovement = hitCollider.GetComponent<Cat_Movement>();
                if (targetMovement != null)
                {
                    targetMovement.Stun(stunDuration);
                    targetMovement.catAnimationsScr.PushedWithSlash(transform.forward);
                }
                hitCollider.GetComponent<Cat_Eat>().DropFood();
            }
        }

        catAnimationsScr.StartCoroutine(catAnimationsScr.PlaySlashAnimation());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * slashRange, slashRadius);
    }
}
