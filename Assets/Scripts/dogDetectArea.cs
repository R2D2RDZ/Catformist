using UnityEngine;

public class dogDetectArea : MonoBehaviour
{
    public float speedThreshold;
    public int damping = 2;
    public float turnSpeed;

    private Transform parentTransform;
    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentTransform = transform.parent;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.GetComponent<Cat_Movement>().rb.linearVelocity.magnitude);
            if (other.gameObject.GetComponent<Cat_Movement>().rb.linearVelocity.magnitude > speedThreshold)
            {
                playerTransform = other.transform;
                Vector3 directionToPlayer = playerTransform.position - parentTransform.position;
                directionToPlayer.y = 0;

                if (directionToPlayer != Vector3.zero)
                {
                    // Calculate the target rotation
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

                    // Smoothly rotate the parent towards the player
                    parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}
