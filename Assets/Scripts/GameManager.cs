using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject playerPrefab;
    private Transform player1SpawnPoint;
    private Transform player2SpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load a new scene and spawn players afterward
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  // Load the target scene
        SceneManager.sceneLoaded += OnSceneLoaded; // Register callback for when the scene is fully loaded
    }

    // Method gets called once the new scene has finished loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event to prevent multiple calls

        // Find the spawn points in the newly loaded scene
        player1SpawnPoint = GameObject.FindWithTag("Player1Spawn").transform;
        player2SpawnPoint = GameObject.FindWithTag("Player2Spawn").transform;

        // Spawn players after scene is loaded and spawn points are identified
        SpawnPlayers();
    }

    // Spawns the players in the new scene
    private void SpawnPlayers()
    {
        // Spawn player 1
        GameObject player1 = Instantiate(playerPrefab, player1SpawnPoint.position, Quaternion.identity);
        PlayerInput player1Input = player1.GetComponent<PlayerInput>();
        player1Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);

        // Spawn player 2
        GameObject player2 = Instantiate(playerPrefab, player2SpawnPoint.position, Quaternion.identity);
        PlayerInput player2Input = player2.GetComponent<PlayerInput>();
        player2Input.SwitchCurrentControlScheme("Gamepad", Gamepad.all[1]);
    }
}
