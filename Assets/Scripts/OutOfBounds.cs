using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<Cat_Lives>().LoseLive();
            }
        }
    }
}
