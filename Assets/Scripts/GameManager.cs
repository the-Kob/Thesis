using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public InputDevice P1Device { get; private set; }
    public InputDevice P2Device { get; private set; }
    
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
    
    public void SetPlayerDevice(int playerIndex, InputDevice device)
    {
        if (playerIndex == 0)
        {
            P1Device = device;
        }
        else if (playerIndex == 1)
        {
            P2Device = device;
        }
    }
    
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        
        if (sceneName != "Menu")
        {
            AudioManager.Instance.PlayGameMusic();
        }
        else
        {
            AudioManager.Instance.PlayMenuMusic();
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded; // Register callback for when the scene is fully loaded
    }

    // Method gets called once the new scene has finished loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event to prevent multiple calls
    }
}
