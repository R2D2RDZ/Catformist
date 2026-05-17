using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Requerido para detectar el Enter de forma nativa

public class RoundSystem : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform[] playerSpawnPoints;
    // Si dejas esta lista vacía en el inspector, el script buscará automáticamente a los gatos en la escena
    [SerializeField] private List<GameObject> players = new List<GameObject>();

    [Header("Food Spawning")]
    [SerializeField] private GameObject[] foodPrefabs;
    [SerializeField] private Transform[] foodSpawnPoints;
    [SerializeField] private int foodCountPerRound = 5;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private int enemyCountPerRound = 3;

    private int currentRound = 0;
    private bool gameStarted = false;

    // Listas internas para rastrear y limpiar los objetos clonados en cada ronda
    private List<GameObject> spawnedFood = new List<GameObject>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        // Al presionar Enter (Return) se inicia la partida si no ha empezado
        if (!gameStarted && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            StartGame();
        }

        // OPCIONAL: Puedes usar otra tecla (ej: Espacio o N) para avanzar de ronda manualmente si lo necesitas para pruebas
        // if (gameStarted && Keyboard.current.nKey.wasPressedThisFrame) { StartNextRound(); }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject newPlayer = playerInput.gameObject;

        if (!players.Contains(newPlayer))
        {
            players.Add(newPlayer);
            Debug.Log($"¡{newPlayer.name} (Jugador {players.Count}) se ha unido a la partida!");

            // Si un jugador se une "tarde" cuando la ronda ya empezó, 
            // lo teletransportamos de inmediato a su spawn para que no se quede flotando
            if (gameStarted)
            {
                PositionSinglePlayer(players.Count - 1);
            }
        }
    }

    // Opcional: Si un jugador se desconecta, lo removemos de la lista
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        if (players.Contains(playerInput.gameObject))
        {
            players.Remove(playerInput.gameObject);
            Debug.Log($"El jugador {playerInput.gameObject.name} ha abandonado la partida.");
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        currentRound = 0;
        Debug.Log("¡Partida Iniciada!");

        // Si no asignaste jugadores manualmente en el Inspector, los buscamos por su script de movimiento
        if (players.Count == 0)
        {
            Cat_Movement[] catMovements = Object.FindObjectsByType<Cat_Movement>();
            foreach (var cat in catMovements)
            {
                players.Add(cat.gameObject);
            }
        }

        StartNextRound();
    }

    public void StartNextRound()
    {
        currentRound++;
        Debug.Log($"=== Iniciando Ronda {currentRound} ===");

        // 1. Limpieza de seguridad (Destruir enemigos y comida de la ronda anterior)
        ClearPreviousRoundObjects();

        // 2. Resetear a todos los jugadores y moverlos a sus Spawnpoints
        ResetAndPositionPlayers();

        // 3. Aparecer la comida en puntos aleatorios
        SpawnFood();

        // 4. Aparecer los enemigos en puntos aleatorios
        SpawnEnemies();
    }

    private void ResetAndPositionPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;

            // Validamos que tengamos suficientes puntos de aparición para la cantidad de jugadores
            if (i < playerSpawnPoints.Length && playerSpawnPoints[i] != null)
            {
                Transform spawnPoint = playerSpawnPoints[i];

                // Importante: Al mover objetos con Rigidbody por script, debemos resetear 
                // sus fuerzas físicas instantáneamente para evitar teletransportaciones con bugs.
                Cat_Movement moveScript = players[i].GetComponent<Cat_Movement>();
                if (moveScript != null && moveScript.rb != null)
                {
                    moveScript.rb.linearVelocity = Vector3.zero;
                    moveScript.rb.angularVelocity = Vector3.zero;
                    moveScript.rb.position = spawnPoint.position;
                }

                // Mover la transformación visual y física
                players[i].transform.position = spawnPoint.position;
                players[i].transform.rotation = spawnPoint.rotation;
            }

            // Resetear la Gatocidad del jugador
            Gatocidad gatocidadScript = players[i].GetComponent<Gatocidad>();
            if (gatocidadScript != null)
            {
                gatocidadScript.ResetGatocidad();
                Debug.Log($"Gatocidad reseteada para {players[i].name}");
            }
        }
    }

    private void SpawnFood()
    {
        if (foodPrefabs.Length == 0 || foodSpawnPoints.Length == 0) return;
        Debug.Log("Apareciendo comida");
        // Creamos una copia de los spawnpoints para mezclarlos y evitar que aparezcan dos comidas en el mismo sitio
        List<Transform> availablePoints = new List<Transform>(foodSpawnPoints);

        for (int i = 0; i < foodCountPerRound; i++)
        {
            if (availablePoints.Count == 0) break; // Nos quedamos sin puntos disponibles

            int randomPointIndex = Random.Range(0, availablePoints.Count);
            int randomPrefabIndex = Random.Range(0, foodPrefabs.Length);

            Transform spawnPoint = availablePoints[randomPointIndex];
            GameObject prefabToSpawn = foodPrefabs[randomPrefabIndex];

            GameObject newFood = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            spawnedFood.Add(newFood);

            // Removemos este punto para que no se repita en esta ronda
            availablePoints.RemoveAt(randomPointIndex);
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || enemySpawnPoints.Length == 0) return;

        List<Transform> availablePoints = new List<Transform>(enemySpawnPoints);

        for (int i = 0; i < enemyCountPerRound; i++)
        {
            if (availablePoints.Count == 0) break;

            int randomPointIndex = Random.Range(0, availablePoints.Count);
            int randomPrefabIndex = Random.Range(0, enemyPrefabs.Length);

            Transform spawnPoint = availablePoints[randomPointIndex];
            GameObject prefabToSpawn = enemyPrefabs[randomPrefabIndex];

            GameObject newEnemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            spawnedEnemies.Add(newEnemy);

            availablePoints.RemoveAt(randomPointIndex);
        }
    }

    private void ClearPreviousRoundObjects()
    {
        // Destruir comida restante
        foreach (GameObject food in spawnedFood)
        {
            if (food != null) Destroy(food);
        }
        spawnedFood.Clear();

        // Destruir enemigos restantes
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        spawnedEnemies.Clear();
    }
    private void PositionSinglePlayer(int index)
    {
        if (index >= players.Count || players[index] == null) return;
        if (index >= playerSpawnPoints.Length || playerSpawnPoints[index] == null) return;

        GameObject player = players[index];
        Transform spawnPoint = playerSpawnPoints[index];

        // Reset físico para evitar tirones
        Cat_Movement moveScript = player.GetComponent<Cat_Movement>();
        if (moveScript != null && moveScript.rb != null)
        {
            moveScript.rb.linearVelocity = Vector3.zero;
            moveScript.rb.angularVelocity = Vector3.zero;
            moveScript.rb.position = spawnPoint.position;
        }

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        // Reset de Gatocidad
        Gatocidad gatocidadScript = player.GetComponent<Gatocidad>();
        if (gatocidadScript != null)
        {
            gatocidadScript.ResetGatocidad();
        }
    }
}