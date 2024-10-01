using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI score;
    private float _scoreInitialFontSize;
    [SerializeField] private RectTransform p1Universe;
    private bool _p1UniverseExists;
    [SerializeField] private RectTransform p2Universe;
    private bool _p2UniverseExists;
    
    [SerializeField] private Sprite effectIcon;
    [SerializeField] private Image p1EffectIcon;
    [SerializeField] private Image p2EffectIcon;
    
    [SerializeField] private RectTransform p1AoeCooldownMask;
    private float _p1AoeCooldown;
    [SerializeField] private GameObject p1EffectsMenu;
    private GameObject[] _p1Effects;
    [HideInInspector] public bool isP1MenuOpen;
    [SerializeField] private RectTransform p1ChooseEffectCooldownMask;
    private float _p1ChooseEffectCooldown;
    private bool _isP1EffectActive;
    
    [SerializeField] private RectTransform p2AoeCooldownMask;
    private float _p2AoeCooldown;
    [SerializeField] private GameObject p2EffectsMenu;
    private GameObject[] _p2Effects;
    [HideInInspector] public bool isP2MenuOpen;
    [SerializeField] private RectTransform p2ChooseEffectCooldownMask;
    private float _p2ChooseEffectCooldown;
    private bool _isP2EffectActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (score != null) _scoreInitialFontSize = score.fontSize;
        
        if (p1Universe != null) _p1UniverseExists = true;
        if (p2Universe != null) _p2UniverseExists = true;

        if (p1EffectsMenu != null)
        {
            var numberOfChildren = p1EffectsMenu.transform.childCount;
            _p1Effects = new GameObject[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                _p1Effects[i] = p1EffectsMenu.transform.GetChild(i).gameObject;
            }
        }
        
        if (p2EffectsMenu != null)
        {
            var numberOfChildren = p2EffectsMenu.transform.childCount;
            _p2Effects = new GameObject[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                _p2Effects[i] = p2EffectsMenu.transform.GetChild(i).gameObject;
            }
        }
    }

    private void FixedUpdate()
    {
        score.fontSize = _scoreInitialFontSize;
        
        if (_p1UniverseExists) p1Universe.localScale = Vector2.one;
        if (_p2UniverseExists) p2Universe.localScale = Vector2.one;
         
        // If the AoE is on cooldown, animate the cooldown mask to decrease until the AoE is available
        if (p1AoeCooldownMask.localScale.y > 0f) {
            p1AoeCooldownMask.localScale -= new Vector3(0f, Time.fixedDeltaTime/_p1AoeCooldown, 0f);
        }
        
        if (p2AoeCooldownMask.localScale.y > 0f) {
            p2AoeCooldownMask.localScale -= new Vector3(0f, Time.fixedDeltaTime/_p2AoeCooldown, 0f);
        }
        
        // If the Choose Effect is on cooldown, animate the cooldown mask to decrease until the Choose Effect is available
        if (p1ChooseEffectCooldownMask.localScale.y > 0f) {
            p1ChooseEffectCooldownMask.localScale -= new Vector3(0f, Time.fixedDeltaTime/_p1ChooseEffectCooldown, 0f);
        }
        else if (_isP1EffectActive)
        {
            _isP1EffectActive = false;  
            ColorUtility.TryParseHtmlString("#FB8F13", out var p1Color);
            p1EffectIcon.color = p1Color;
            p1EffectIcon.sprite = effectIcon;
        }
        
        if (p2ChooseEffectCooldownMask.localScale.y > 0f) {
            p2ChooseEffectCooldownMask.localScale -= new Vector3(0f, Time.fixedDeltaTime/_p2ChooseEffectCooldown, 0f);
        } 
        else if (_isP2EffectActive)
        {
            _isP2EffectActive = false;
            ColorUtility.TryParseHtmlString("#315D9A", out var p2Color);
            p2EffectIcon.color = p2Color;
            p2EffectIcon.sprite = effectIcon;

        }
    }

    public void TriggerAoE(bool isPlayer1, float cooldown)
    {
        if (isPlayer1)
        {
            _p1AoeCooldown = cooldown;
            p1AoeCooldownMask.localScale = Vector3.one;
        }
        else
        {
            _p2AoeCooldown = cooldown;
            p2AoeCooldownMask.localScale = Vector3.one;
        }
    }

    public void TriggerChooseEffect(bool isPlayer1, float cooldown)
    {
        if (isPlayer1)
        {
            _p1ChooseEffectCooldown = cooldown;
            p1ChooseEffectCooldownMask.localScale = Vector3.one;
            _isP1EffectActive = true;
        }
        else
        {
            _p2ChooseEffectCooldown = cooldown;
            p2ChooseEffectCooldownMask.localScale = Vector3.one;
            _isP2EffectActive = true;
        }
    }

    public void ToggleMenu(bool isPlayer1, bool toggle)
    {
        if (toggle)
        {
            AudioManager.Instance.DecreasePitch();
        }
        else
        {
            AudioManager.Instance.IncreasePitch();
        }
        
        if (isPlayer1)
        {
            p1EffectsMenu.SetActive(toggle);
            isP1MenuOpen = toggle;
        }
        else
        {
            p2EffectsMenu.SetActive(toggle);
            isP2MenuOpen = toggle;
        }
    }
}
