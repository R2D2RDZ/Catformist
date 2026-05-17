using UnityEngine;

public class Gatocidad : MonoBehaviour
{
    [SerializeField] public float gatocidad = 100f;
    [SerializeField] public float maxGatocidad = 100f;
    [SerializeField] float restoringSpeed = 1f;

    /// <summary>
    /// Se llama en cada frame de fÃ­sicas. Regenera pasivamente la gatocidad con el paso del tiempo.
    /// </summary>
    private void FixedUpdate()
    {
        gatocidad = Mathf.Min(gatocidad + (Time.fixedDeltaTime * restoringSpeed), maxGatocidad);
    }

    /// <summary>
    /// Devuelve el valor actual de la gatocidad.
    /// </summary>
    /// <returns>La cantidad de gatocidad actual.</returns>
    public float GetGatocidad()
    {
        return gatocidad;
    }

    /// <summary>
    /// Intenta consumir una cantidad de gatocidad.
    /// </summary>
    /// <param name="amount">La cantidad de gatocidad a consumir.</param>
    /// <returns>Devuelve true si habÃ­a suficiente gatocidad y se consumiÃ³, false en caso contrario.</returns>
    public bool UseGatocidad(float amount)
    {
        if (amount > gatocidad)
        {
            return false;
        }
        gatocidad -= amount;
        return true;
    }

    /// <summary>
    /// AÃ±ade una cantidad especÃ­fica a la gatocidad actual.
    /// </summary>
    /// <param name="amount">La cantidad a aÃ±adir.</param>
    public void RestoreGatocidad(float amount)
    {
        gatocidad = Mathf.Min(gatocidad + amount, maxGatocidad);
    }

    /// <summary>
    /// Restablece la gatocidad a su valor mÃ¡ximo.
    /// </summary>
    public void ResetGatocidad()
    {
        gatocidad = maxGatocidad;
    }

    public void IncreaseMaxGatocidad(float amount)
    {
        maxGatocidad += amount;
    }
}

