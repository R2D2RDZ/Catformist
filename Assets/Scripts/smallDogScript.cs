using UnityEngine;
using Pathfinding;

public class smallDogScript : MonoBehaviour
{
    [SerializeField] private Transform grabPoint;

    private Food currentGrabbedFood;
    private Food targetFood;
    private GameObject parent;
    private AIDestinationSetter chase;
    private Patrol patrol;
    private RigidbodyFollower rbFollow;
    public Transform safeSpace;

    public bool ignoreCat=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        chase = parent.GetComponent<AIDestinationSetter>();
        patrol = parent.GetComponent<Patrol>();
        rbFollow = parent.GetComponent<RigidbodyFollower>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food")&&(chase == true))
        {
            GrabFood(other.gameObject.GetComponent<Food>());
            chase.target = safeSpace;
            ignoreCat = true;
            Debug.Log("Agarrada la comida");
        }
    }

    private void GrabFood(Food food)
    {
        currentGrabbedFood = food;
        Debug.Log($"Agarraste: {food.gameObject.name}");

        if (grabPoint != null)
        {
            currentGrabbedFood.transform.SetParent(grabPoint);
            currentGrabbedFood.transform.localPosition = Vector3.zero;
            currentGrabbedFood.transform.localRotation = Quaternion.identity;
        }
        else
        {
            currentGrabbedFood.transform.SetParent(transform);
        }

        Rigidbody rb = currentGrabbedFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Collider collider = currentGrabbedFood.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}
