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

    [HideInInspector] public float holdTime = 0f;
    private const float LongPressThreshold = 0.2f;

    [HideInInspector] public Food currentGrabbedFood;
    [HideInInspector] public Food targetFood;

    private Food foodWithOutline;

    // Guarda el progreso de masticado por cada objeto de comida
    [HideInInspector] public Dictionary<Food, float> foodProgress = new Dictionary<Food, float>();

    private void Start()
    {
        catInputs = GetComponent<Cat_Inputs>();
        gatocidad = GetComponent<Gatocidad>();
    }

    private void Update()
    {
        if (catInputs == null) return;

        Food closestFood = GetClosestFood();

        // Gestión de Outline Pasivo
        if (closestFood != foodWithOutline)
        {
            ToggleOutline(foodWithOutline, false); // Apaga el anterior
            ToggleOutline(closestFood, true);     // Enciende el nuevo
            foodWithOutline = closestFood;         // Actualiza la referencia
        }

        // 1. Frame Inicial: Detectamos qué comida tenemos enfrente
        if (catInputs.wasEatPressed)
        {
            holdTime = 0f;
            targetFood = GetClosestFood();
        }

        // 2. Mantener Presionado: Lógica de comer
        if (catInputs.isEatPressed)
        {
            holdTime += Time.deltaTime;

            if (holdTime > LongPressThreshold)
            {
                // REGLA: Solo se puede comer si está en el suelo (no la tenemos agarrada)
                Debug.Log("Agarrando");
                if (targetFood != null && currentGrabbedFood == null)
                {
                    // Validación de seguridad: Verificar si la comida sigue dentro del rango actual del gato
                    Vector3 checkCenter = transform.position + transform.forward * eatRange;
                    float distance = Vector3.Distance(checkCenter, targetFood.transform.position);

                    if (distance <= eatRadius)
                    {
                        EatFood(targetFood);
                    }
                    else
                    {
                        Debug.Log("Te alejaste demasiado de la comida, acción cancelada.");
                        targetFood = null;
                    }
                }
            }
        }

        // 3. Soltar el Botón: Lógica de Agarrar/Soltar
        if (catInputs.wasEatReleased)
        {
            if (holdTime <= LongPressThreshold)
            {
                // Click corto: Agarrar o Soltar
                if (currentGrabbedFood != null)
                {
                    Debug.Log("Soltando");
                    DropFood();
                }
                else if (targetFood != null)
                {
                    Debug.Log("Agarrando");
                    GrabFood(targetFood);
                }
            }
            else
            {
                // Si soltó un Long Press y no terminó de comer, mostramos el progreso guardado
                if (targetFood != null && foodProgress.ContainsKey(targetFood))
                {
                    Debug.Log($"Dejaste de comer. Progreso: {foodProgress[targetFood]:F1}s / {targetFood.timeToEat}s");
                }
            }

            // Resetear el objetivo al soltar el botón
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
                Debug.Log("Encontré comida");
                Food foodComponent = hit.GetComponent<Food>();
                // Ignoramos la comida que ya llevamos en la boca/garras
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

    public void DropFood()
    {
        if (currentGrabbedFood == null) return;
        Debug.Log($"Soltaste: {currentGrabbedFood.gameObject.name}");

        currentGrabbedFood.transform.SetParent(null);

        Rigidbody rb = currentGrabbedFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        Collider collider = currentGrabbedFood.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
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

        // Feedback en consola para saber que está funcionando
        Debug.Log($"Comiendo... {foodProgress[food]:F1}s / {food.timeToEat}s");

        if (foodProgress[food] >= food.timeToEat)
        {
            if (gatocidad != null)
            {
                gatocidad.IncreaseMaxGatocidad(food.maxGatocidadIncrease);
                gatocidad.RestoreGatocidad(food.gatocidadRestore);
                Debug.Log($"¡Ñam! Comida devorada. Max Gatocidad +{food.maxGatocidadIncrease}, Gatocidad restaurada +{food.gatocidadRestore}");
            }

            foodProgress.Remove(food);

            ToggleOutline(food, false);
            if (foodWithOutline == food) foodWithOutline = null;

            Destroy(food.gameObject);
            targetFood = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + transform.forward * eatRange, eatRadius);
    }
    private void ToggleOutline(Food food, bool state)
    {
        if (food == null) return;

        Outline outline = food.GetComponent<Outline>();
        if (outline == null)
        {
            outline = food.GetComponentInChildren<Outline>();
        }

        if (outline != null)
        {
            outline.enabled = state;
        }
    }
}