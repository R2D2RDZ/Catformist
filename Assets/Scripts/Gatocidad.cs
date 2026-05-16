using UnityEngine;

public class Gatocidad : MonoBehaviour
{
    [SerializeField] float gatocidad = 100f;
    [SerializeField] float maxGatocidad = 100f;
    [SerializeField] float restoringSpeed = 1f;

    /// <summary>
    /// Se llama en cada frame de físicas. Regenera pasivamente la gatocidad con el paso del tiempo.
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
    /// <returns>Devuelve true si había suficiente gatocidad y se consumió, false en caso contrario.</returns>
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
    /// Añade una cantidad específica a la gatocidad actual.
    /// </summary>
    /// <param name="amount">La cantidad a añadir.</param>
    public void RestoreGatocidad(float amount)
    {
        gatocidad = Mathf.Min(gatocidad + amount, maxGatocidad);
    }

    /// <summary>
    /// Restablece la gatocidad a su valor máximo.
    /// </summary>
    public void ResetGatocidad()
    {
        gatocidad = maxGatocidad;
    }

}
