using UnityEngine;
using UnityEngine.InputSystem; // Requerido para detectar el índice del jugador

public class Cat_Lives : MonoBehaviour
{
    [Header("Life Settings")]
    [SerializeField] private int maxLives = 7;
    private int currentLives;

    [Header("Respawn Points")]
    // Aquí arrastras los mismos spawnpoints que usaste en el RoundSystem
    [SerializeField] private Transform[] spawnPoints;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private Gatocidad gatocidad;
    private Cat_Movement catMovement;

    void Start()
    {
        currentLives = maxLives;

        // Cacheamos los componentes del gato
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        gatocidad = GetComponent<Gatocidad>();
        catMovement = GetComponent<Cat_Movement>();
    }

    /// <summary>
    /// Función pública para restar una vida al gato y reaparecerlo.
    /// </summary>
    public void LoseLive()
    {
        if (currentLives <= 0) return; // Ya está fuera de juego

        currentLives--;
        Debug.Log($"¡{gameObject.name} perdió una vida! Vidas restantes: {currentLives}");

        gameObject.GetComponent<Gatocidad>().LoseGatocidad();

        if (currentLives > 0)
        {
            Respawn();
        }
        else
        {
            OnDeath();
        }
    }

    private void Respawn()
    {
        // Buscamos el director de la partida en la escena
        RoundSystem roundSystem = Object.FindAnyObjectByType<RoundSystem>();

        if (roundSystem != null)
        {
            Debug.Log($"Delegando el respawn de {gameObject.name} al RoundSystem.");

            // Le pedimos al RoundSystem que nos reposicione usando su lista centralizada
            roundSystem.PositionSinglePlayer(gameObject);
        }
        else
        {
            Debug.LogError($"¡Error! {gameObject.name} no encontró el RoundSystem para poder reaparecer.");
        }
    }

    private void OnDeath()
    {
        Debug.Log($"¡{gameObject.name} se ha quedado sin vidas! ELIMINADO.");

        // Buscamos el sistema de rondas que está activo en la escena
        RoundSystem roundSystem = Object.FindAnyObjectByType<RoundSystem>();

        if (roundSystem != null)
        {
            // Le avisamos al sistema que nos saque de la lista de sobrevivientes
            roundSystem.RemovePlayer(gameObject);
        }
        else
        {
            Debug.LogWarning("No se encontró el RoundSystem en la escena para eliminar al jugador.");
        }

        // Desactivamos el objeto por completo para que desaparezca de la partida
        gameObject.SetActive(false);
    }

    // Función pública por si necesitas consultar cuántas vidas le quedan desde la UI
    public int GetCurrentLives()
    {
        return currentLives;
    }
}