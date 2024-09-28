using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    
    private InputActionMap _uiActionMap;
    
    [SerializeField] public PistolRotation pistolP1;
    [SerializeField] public PistolRotation pistolP2;
    
    [SerializeField] private GameObject connectDevicesMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuFirstSelectedButton;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject playMenuFirstSelectedButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject settingsMenuFirstSelectedButton;
    
    private bool _bothPlayersConnected = false;

    private void Start()
    {
        _uiActionMap = inputActions.FindActionMap("UI");
        _uiActionMap.Enable();
    }
    
    private void Update()
    {
        if (!pistolP1.isPlayerConnected && Gamepad.all.Count > 0 && Gamepad.all[0].startButton.isPressed)
        {
            pistolP1.isPlayerConnected = true;
            
            Gamepad.all[0].SetMotorSpeeds(0.5f, 0.5f);
            StartCoroutine(StopGamepadVibration(0, 0.5f));
        }

        // Check for Player 2 connection (if a second controller is available)
        if (!pistolP2.isPlayerConnected && Gamepad.all.Count > 1 && Gamepad.all[1].startButton.isPressed)
        {
            pistolP2.isPlayerConnected = true;
            
            Gamepad.all[1].SetMotorSpeeds(0.5f, 0.5f);
            StartCoroutine(StopGamepadVibration(1, 0.5f));
        }

        if (pistolP1.isPlayerConnected && pistolP2.isPlayerConnected && !_bothPlayersConnected)
        {
            _bothPlayersConnected = true;
            
            OnBackButtonPressed();
            
            _uiActionMap.Enable(); // ensure the UI action map is active
        }
    }

    private static IEnumerator StopGamepadVibration(int playerIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        Gamepad.all[playerIndex].SetMotorSpeeds(0f, 0f);
    }

    public void OnPlayButtonPressed()
    {
        connectDevicesMenu.SetActive(false);
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        settingsMenu.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(playMenuFirstSelectedButton);
    }

    public void OnSettingsButtonPressed()
    {
        connectDevicesMenu.SetActive(false);
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(settingsMenuFirstSelectedButton);
    }
    
    public void OnTutorialButtonPressed()
    {
        GameManager.Instance.LoadGameScene("Tutorial");
    }
    
    public void OnGameButtonPressed()
    {
        GameManager.Instance.LoadGameScene("Game");
    }

    public void OnBackButtonPressed()
    {
        connectDevicesMenu.SetActive(false);
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(mainMenuFirstSelectedButton);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
}