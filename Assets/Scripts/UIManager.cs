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
    private GameObject[] _p1EffectButtons;
    private Image[] _p1EffectIcons;
    [SerializeField] private RectTransform p1ChooseEffectCooldownMask;
    private float _p1ChooseEffectCooldown;
    private bool _isP1EffectActive;
    
    [SerializeField] private RectTransform p2AoeCooldownMask;
    private float _p2AoeCooldown;
    [SerializeField] private GameObject p2EffectsMenu;
    private GameObject[] _p2EffectButtons;
    private Image[] _p2EffectIcons;
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
            _p1EffectButtons = new GameObject[numberOfChildren];
            _p1EffectIcons = new Image[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                var effectButton = p1EffectsMenu.transform.GetChild(i).gameObject;
                var effectButtonIcon = effectButton.transform.Find("Icon");

                if (effectButton != null)
                {
                    _p1EffectButtons[i] = effectButton;
                }
                
                if (effectButtonIcon != null)
                {
                    _p1EffectIcons[i] = effectButtonIcon.GetComponent<Image>();
                }
            }
        }
        
        if (p2EffectsMenu != null)
        {
            var numberOfChildren = p2EffectsMenu.transform.childCount;
            _p2EffectButtons = new GameObject[numberOfChildren];
            _p2EffectIcons = new Image[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                var effectButton = p2EffectsMenu.transform.GetChild(i).gameObject;
                var effectButtonIcon = effectButton.transform.Find("Icon");

                if (effectButton != null)
                {
                    _p2EffectButtons[i] = effectButton;
                }
                
                if (effectButtonIcon != null)
                {
                    _p2EffectIcons[i] = effectButtonIcon.GetComponent<Image>();
                }
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

    public void OpenEffectMenu(bool isPlayer1)
    {
        if (isPlayer1)
        {
            if(p1EffectsMenu.activeSelf) return;
            
            p1EffectsMenu.SetActive(true);
        }
        else
        {
            if(p2EffectsMenu.activeSelf) return;
            
            p2EffectsMenu.SetActive(true);
        }
    }

    public void CloseEffectMenu(bool isPlayer1, int chosenEffect)
    {
        if (isPlayer1)
        {
            if (chosenEffect != -1)
            {
                p1EffectIcon.sprite = _p1EffectIcons[chosenEffect].sprite;
                p1EffectIcon.color = Color.white;
            }
            
            ClearEffectChoice(true);
            p1EffectsMenu.SetActive(false);
        }
        else
        {
            if (chosenEffect != -1)
            {
                p2EffectIcon.sprite = _p2EffectIcons[chosenEffect].sprite;
                p2EffectIcon.color = Color.white;
            }

            ClearEffectChoice(false);
            p2EffectsMenu.SetActive(false);
        }
    }

    public void ClearEffectChoice(bool isPlayer1)
    {
        if (isPlayer1)
        {
            foreach (var effect in _p1EffectButtons)
            {
                effect.GetComponent<Image>().color = new Color (0f,0f,0f,1f);
            }
        }
        else
        {
            foreach (var effect in _p2EffectButtons)
            {
                effect.GetComponent<Image>().color = new Color (0f,0f,0f,1f);
            }
        }
    }

    public void ChooseEffect(bool isPlayer1, int chosenEffect)
    {
        if (isPlayer1)
        {
            _p1EffectButtons[chosenEffect].GetComponent<Image>().color = new Color (0.1f,0.1f,0.1f,1f);
        }
        else
        {
            _p2EffectButtons[chosenEffect].GetComponent<Image>().color = new Color (0.1f,0.1f,0.1f,1f);
        }
    }

    public void ScaleEffectButtons(bool isPlayer1, int chosenEffect)
    {
        if (isPlayer1)
        {
            for (var i = 0; i < _p1EffectButtons.Length; i++)
            {
                if (_p1EffectButtons[i].GetComponent<RectTransform>().localScale.x > 1f && chosenEffect != i)
                {
                    _p1EffectButtons[i].GetComponent<RectTransform>().localScale -= new Vector3(0.01f, 0.01f, 0f);
                }
            }
            
            if (chosenEffect == -1) return;
            
            if (_p1EffectButtons[chosenEffect].GetComponent<RectTransform>().localScale.x < 1.1f)
            {
                _p1EffectButtons[chosenEffect].GetComponent<RectTransform>().localScale += new Vector3(0.01f, 0.01f, 0f);
            }
        }
        else
        {
            for (var i = 0; i < _p2EffectButtons.Length; i++)
            {
                if (_p2EffectButtons[i].GetComponent<RectTransform>().localScale.x > 1f && chosenEffect != i)
                {
                    _p2EffectButtons[i].GetComponent<RectTransform>().localScale -= new Vector3(0.01f, 0.01f, 0f);
                }
            }
            
            if (chosenEffect == -1) return;
            
            if (_p2EffectButtons[chosenEffect].GetComponent<RectTransform>().localScale.x < 1.1f)
            {
                _p2EffectButtons[chosenEffect].GetComponent<RectTransform>().localScale += new Vector3(0.01f, 0.01f, 0f);
            }
        }
    }
}
