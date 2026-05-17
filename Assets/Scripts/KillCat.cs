using UnityEngine;

public class KillCat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(":0");
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Cat_Lives>().LoseLive();
        }
    }
}
