using UnityEngine;

public class visionScript : MonoBehaviour
{
    private GameObject parent;
    private perroScript perro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        perro = parent.GetComponent<perroScript>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            perro.patrol.enabled = false;
            perro.chase.enabled = true;
            perro.chase.target = other.transform;
            perro.rbFollow.moveSpeed = perro.chaseSpeed;
        }
    }
}
