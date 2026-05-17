using UnityEngine;
using Pathfinding;

public class visionSmallDog : MonoBehaviour
{
    private GameObject parent;
    private smallDogScript ignoreflag;
    private AIDestinationSetter chase;
    private Patrol patrol;
    private GameObject targetFood;
    private RigidbodyFollower rbFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        chase = parent.GetComponent<AIDestinationSetter>();
        patrol = parent.GetComponent<Patrol>();
        rbFollow = parent.GetComponent<RigidbodyFollower>();
        ignoreflag = transform.parent.Find("GrabFoodHB").GetComponent<smallDogScript>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ignoreflag.ignoreCat==false)
        {
            targetFood = FindClosestWithTagExcludingY("Food");
            patrol.enabled = false;
            chase.enabled = true;
            chase.target = targetFood.transform;
            rbFollow.moveSpeed = 10;
            Debug.Log("Persiguiendo " + targetFood);
        }
    }

    public GameObject FindClosestWithTagExcludingY(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float shortestDistanceSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject obj in objectsWithTag)
        {
            
            // Calculate the difference vector
            Vector3 diff = obj.transform.position - currentPos;

            // Exclude the vertical (Y) difference
            diff.y = 0;

            // Use sqrMagnitude for faster performance
            if (obj.GetComponent<Collider>().enabled == true)
            {
                float distanceSqr = diff.sqrMagnitude;
                if (distanceSqr < shortestDistanceSqr)
                {
                    shortestDistanceSqr = distanceSqr;
                    closest = obj;
                }
            }

            
        }
        return closest;
    }
}
