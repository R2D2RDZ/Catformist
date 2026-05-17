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
    private FollowerEntity entity;
    public Transform safeSpace;

    public bool ignoreCat=false;
    private GameObject grabbedFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        chase = parent.GetComponent<AIDestinationSetter>();
        patrol = parent.GetComponent<Patrol>();
        rbFollow = parent.GetComponent<RigidbodyFollower>();
        entity = parent.GetComponent<FollowerEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(chase.isActiveAndEnabled == true)
        {
            if (entity.reachedEndOfPath)
            {
                Destroy(grabbedFood);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food")&&(chase.isActiveAndEnabled == true))
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
        grabbedFood = food.gameObject;
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
