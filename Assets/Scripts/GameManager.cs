using System;
using Data_Storage;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public enum GameState
    {
        Menu,
        Tutorial,
        Game
    }
    
    public GameState CurrentState { get; private set; }
    public InputDevice P1Device { get; private set; }
    public InputDevice P2Device { get; private set; }

    public bool TutorialDone { get; private set; }
    public bool BothPlayersHaveConnectedOnce { get; private set; }

    private bool _isFirstPlaythrough;
    
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
        
        ChangeState(GameState.Menu);
    }

    public void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            _isFirstPlaythrough = true;
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
    
    public void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        SceneManager.LoadScene(sceneName);
        
        if (sceneName == "Menu")
        {
            ChangeState(GameState.Menu);
            AudioManager.Instance.PlayMenuMusic();
        }
        else
        {
            if (sceneName == "Tutorial")
            {
                ChangeState(GameState.Tutorial);
            }
            else
            {
                ChangeState(GameState.Game);
                DataStorageManager.Instance.CreateNewEntry(_isFirstPlaythrough);
            }
            
            AudioManager.Instance.PlayGameMusic();
        }
    }
    
    public void SetTutorialDone(bool result)
    {
        TutorialDone = result;
    }
    
    public void SetBothPlayersConnected(bool result)
    {
        BothPlayersHaveConnectedOnce = result;
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        Instance._isFirstPlaythrough = false;
    }
    
    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }
}
