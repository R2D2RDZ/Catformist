using System.Collections.Generic;
using UnityEngine;

public class Cat_Eat : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float eatRange = 1f;
    [SerializeField] private float eatRadius = 1.5f;

    private Cat_Inputs catInputs;
    private Gatocidad gatocidad;

    private float holdTime = 0f;
    private const float LongPressThreshold = 0.2f;

    private Food currentGrabbedFood;
    private Food targetFood;

    // Save eating progress per food object
    private Dictionary<Food, float> foodProgress = new Dictionary<Food, float>();

    private void Start()
    {
        catInputs = GetComponent<Cat_Inputs>();
        gatocidad = GetComponent<Gatocidad>();
    }

    private void Update()
    {
        if (catInputs == null) return;

        if (catInputs.WasEatPressedThisFrame())
        {
            holdTime = 0f;
            targetFood = GetClosestFood();
        }

        if (catInputs.IsEatPressed())
        {
            holdTime += Time.deltaTime;

            if (holdTime > LongPressThreshold)
            {
                // Long press - Try to eat
                if (targetFood != null && currentGrabbedFood == null)
                {
                    EatFood(targetFood);
                }
            }
        }

        if (catInputs.WasEatReleasedThisFrame())
        {
            if (holdTime <= LongPressThreshold)
            {
                // Short press - Try to grab or drop
                if (currentGrabbedFood != null)
                {
                    DropFood();
                }
                else if (targetFood != null)
                {
                    GrabFood(targetFood);
                }
            }

            // Reset
            targetFood = null;
            holdTime = 0f;
        }
    }

    private Food GetClosestFood()
    {
        Vector3 checkCenter = transform.position + transform.forward * eatRange;
        Collider[] hits = Physics.OverlapSphere(checkCenter, eatRadius);
        Food closest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Food"))
            {
                Food foodComponent = hit.GetComponent<Food>();
                if (foodComponent != null && foodComponent != currentGrabbedFood)
                {
                    float dist = Vector3.Distance(checkCenter, hit.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = foodComponent;
                    }
                }
            }
        }

        return closest;
    }

    private void GrabFood(Food food)
    {
        currentGrabbedFood = food;

        // Parent to grab point
        if (grabPoint != null)
        {
            currentGrabbedFood.transform.SetParent(grabPoint);
            currentGrabbedFood.transform.localPosition = Vector3.zero;
        }
        else
        {
            currentGrabbedFood.transform.SetParent(transform);
        }

        // Disable physics while holding
        Rigidbody rb = currentGrabbedFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void DropFood()
    {
        if (currentGrabbedFood == null) return;

        currentGrabbedFood.transform.SetParent(null);

        // Re-enable physics
        Rigidbody rb = currentGrabbedFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        currentGrabbedFood = null;
    }

    private void EatFood(Food food)
    {
        if (!foodProgress.ContainsKey(food))
        {
            foodProgress[food] = 0f;
        }

        foodProgress[food] += Time.deltaTime;

        if (foodProgress[food] >= food.timeToEat)
        {
            // Finish eating
            if (gatocidad != null)
            {
                gatocidad.IncreaseMaxGatocidad(food.maxGatocidadIncrease);
                gatocidad.RestoreGatocidad(food.gatocidadRestore);
            }

            // Clean up
            foodProgress.Remove(food);
            Destroy(food.gameObject);
            targetFood = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * eatRange, eatRadius);
    }
}
