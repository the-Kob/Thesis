using System;
using System.Collections;
using Data_Storage;
using Data_Storage.Events;
using Menu;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject finalScoreBox;
    [SerializeField] private GameObject returnToMenuButton;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI disableScore;
    private bool _canGainScore;
    private float _scoreInitialFontSize;
    private const float ScoreGainFontSize = 86f;
    private float _scoreValue;
    [SerializeField] private GameObject lostScorePrefab;

    [SerializeField] private TextMeshProUGUI combo;
    private float _comboInitialFontSize;
    private const float ComboMultiplierFontSize = 24f;
    private Color _comboInitialColor;
    private int _comboMultiplier;
    [SerializeField] private float comboWindow;
    private float _activeComboCooldownTimer;
    private bool _isComboActive;
    
    [SerializeField] private RectTransform p1Universe;
    private bool _p1UniverseExists;
    [SerializeField] private RectTransform p2Universe;
    private bool _p2UniverseExists;
    [SerializeField] private GameObject p1StarsParent;
    private GameObject[] _p1Stars;
    [SerializeField] private GameObject p2StarsParent;
    private GameObject[] _p2Stars;
    
    [SerializeField] private Sprite effectIcon;
    [SerializeField] private Image p1EffectIcon;
    [SerializeField] private Image p2EffectIcon;
    
    [SerializeField] private RectTransform p1AoeCooldownMask;
    private float _p1AoeCooldown;
    public GameObject p1EffectsMenu;
    private GameObject[] _p1EffectButtons;
    private Image[] _p1EffectIcons;
    [SerializeField] private RectTransform p1ChooseEffectCooldownMask;
    private float _p1ChooseEffectCooldown;
    private bool _isP1EffectActive;
    
    [SerializeField] private RectTransform p2AoeCooldownMask;
    private float _p2AoeCooldown;
    public GameObject p2EffectsMenu;
    private GameObject[] _p2EffectButtons;
    private Image[] _p2EffectIcons;
    [SerializeField] private RectTransform p2ChooseEffectCooldownMask;
    private float _p2ChooseEffectCooldown;
    private bool _isP2EffectActive;
    
    [SerializeField] private TextMeshProUGUI timeText;
    
    [SerializeField] private RectTransform timeMask;
    private float _timeMaskMaxWidth;
    
    [SerializeField] private float timeRemaining = 180f;
    private int _elapsedTime;
    private int _lastWaveSpawnTime;
    private int _lastTimeScaleUpdate;
    
    [SerializeField] private Color initialTimerColor;
    [SerializeField] private Color endTimerColor;
    private Vector3 _colorDelta;
    [SerializeField] private GameObject timeMaskColor;
    
    private float _timeScale = 1f;

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

        if (combo != null)
        {
            _comboInitialFontSize = combo.fontSize;
            _comboInitialColor = combo.color;
        }
        
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

        if (p1StarsParent != null)
        {
            var numberOfChildren = p1StarsParent.transform.childCount;
            _p1Stars = new GameObject[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                _p1Stars[i] = p1StarsParent.transform.GetChild(i).gameObject;
            }
        }
        
        if (p2StarsParent != null)
        {
            var numberOfChildren = p2StarsParent.transform.childCount;
            _p2Stars = new GameObject[numberOfChildren];

            for (var i = 0; i < numberOfChildren; i++)
            {
                _p2Stars[i] = p2StarsParent.transform.GetChild(i).gameObject;
            }
        }
        
        if (timeMask != null)
        {
            _timeMaskMaxWidth = timeMask.sizeDelta.x;
        }

        if (timeMaskColor != null)
        {
            timeMaskColor.GetComponent<Image>().color = initialTimerColor;
        }
        
        _colorDelta = new Vector3(
            (initialTimerColor.r - endTimerColor.r)/timeRemaining,
            (initialTimerColor.g - endTimerColor.g)/timeRemaining,
            (initialTimerColor.b - endTimerColor.b)/timeRemaining);
    }

    private void Start()
    {
        _timeScale = 1f;
        Time.timeScale = _timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        
        if (GameManager.Instance.TutorialDone) WaveManager.Instance.StartNewWave();
        _lastWaveSpawnTime = 0;
    }
    
    private void FixedUpdate()
    {
        score.fontSize = _scoreInitialFontSize;
        
        if (_p1UniverseExists) p1Universe.localScale = Vector2.one;
        if (_p2UniverseExists) p2Universe.localScale = Vector2.one;

        if (GameManager.Instance.CurrentState != GameManager.GameState.Tutorial)
        {
            // Time management
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.fixedDeltaTime;
                _elapsedTime = (int)(180f - timeRemaining);

                var minutes = Mathf.FloorToInt(timeRemaining / 60f);
                var seconds = Mathf.FloorToInt(timeRemaining % 60);
                timeText.text = $"{minutes:D2}:{seconds:D2}";

                timeMask.sizeDelta = new Vector2(_timeMaskMaxWidth * (timeRemaining / 180f), timeMask.sizeDelta.y);

                var currentColor = timeMaskColor.GetComponent<Image>().color;
                currentColor.r = Mathf.Max(0f, currentColor.r - _colorDelta.x * Time.fixedDeltaTime);
                currentColor.g = Mathf.Max(0f, currentColor.g - _colorDelta.y * Time.fixedDeltaTime);
                currentColor.b = Mathf.Max(0f, currentColor.b - _colorDelta.z * Time.fixedDeltaTime);
                timeMaskColor.GetComponent<Image>().color = currentColor;

                // Spawn wave every 30 seconds only if this is a new interval
                if (_elapsedTime % 30 == 0 && _elapsedTime != _lastWaveSpawnTime)
                {
                    WaveManager.Instance.StartNewWave();
                    _lastWaveSpawnTime = _elapsedTime;
                }

                // Increase time scale every 60 seconds, only if this is a new interval
                if (_elapsedTime % 60 == 0 && _elapsedTime != _lastTimeScaleUpdate && _elapsedTime > 0)
                {
                    _timeScale += 0.1f;
                    Time.timeScale = _timeScale;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    _lastTimeScaleUpdate = _elapsedTime;
                }
            }
            else
            {
                EndTimer();
            }
        }
        else
        {
            timeText.gameObject.SetActive(false);
            timeMask.gameObject.SetActive(false);
        }
        
        // Combo Logic
        if (_activeComboCooldownTimer > 0f)
        {
            _activeComboCooldownTimer -= Time.fixedDeltaTime;
            combo.color -= new Color(0f, 0f, 0f, Time.fixedDeltaTime / comboWindow);
        }

        if (_activeComboCooldownTimer < 0f && _isComboActive)
        {
            _comboMultiplier = 0;
            combo.text = "";
            _isComboActive = false;
            
            for (var i = 2; i < _p1Stars.Length; i++)
            {
                _p1Stars[i]?.SetActive(false);
                _p2Stars[i]?.SetActive(false);
            }
        }
         
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

    public void TriggerAoE(bool isPlayer1, float cooldown, float distance)
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
        
        DataStorageManager.Instance.SavePlayerSecondaryAttackUsed(isPlayer1, _elapsedTime, _scoreValue, _comboMultiplier, distance);
    }

    public void TriggerChooseEffect(bool isPlayer1, float cooldown, int chosenEffect, float distance)
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
        
        DataStorageManager.Instance.SavePlayerEffectUsed(isPlayer1, _elapsedTime, chosenEffect, _scoreValue, _comboMultiplier, distance);
    }

    public void TriggerEnemyHit(bool isPlayer1, float distance)
    {
        DataStorageManager.Instance.SaveEnemyHit(isPlayer1, _elapsedTime, _scoreValue, _comboMultiplier, distance);
    }
    
    public void TriggerEnemyKilled(bool isPlayer1, float distance)
    {
        DataStorageManager.Instance.SaveEnemyKilled(isPlayer1, _elapsedTime, _scoreValue, _comboMultiplier, distance);
    }

    public void TriggerBulletMiss(bool isPlayer1, float distance)
    {
        DataStorageManager.Instance.SaveBulletMiss(isPlayer1, _elapsedTime, _scoreValue, _comboMultiplier, distance);
    }

    public void TriggerPlayerRangeChange(bool isPlayer1, float distance, DistanceTrend distanceTrend, MovementTrend movementTrend)
    {
        DataStorageManager.Instance.SavePlayerRangeChange(isPlayer1, _elapsedTime, _scoreValue, _comboMultiplier, distance, distanceTrend, movementTrend);
    }

    #region Choose Effect Logic

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

    #endregion

    #region Score and Combo Logic

    public void ChangeScore(float newScore, bool isPlayer1)
    {
        if (!_canGainScore) return;

        score.fontSize = ScoreGainFontSize;
        _scoreValue += (_comboMultiplier > 0) ? newScore * _comboMultiplier : newScore;
        score.text = Mathf.CeilToInt(_scoreValue).ToString();

        if (isPlayer1)
        {
            p1Universe.localScale = new Vector2(1.1f, 1.1f);
        }
        else
        {
            p2Universe.localScale = new Vector2(1.1f, 1.1f);
        }
    }
    
    public void SetScoreGainAvailability(bool canGainScore,  bool? isPlayer1 = null, float? distance = null)
    {
        _canGainScore = canGainScore;

        disableScore.text = _canGainScore ? "" : "X";

        if (isPlayer1 == null || distance == null) return;

        DataStorageManager.Instance.SavePlayerGettingHit(isPlayer1.Value, _elapsedTime, _scoreValue, _comboMultiplier, distance.Value);
    }


    public void ChangeCombo(bool positive)
    {
        if (positive)
        {
            if (_canGainScore)
            {
                _isComboActive = true;
                _activeComboCooldownTimer = comboWindow;
                _comboMultiplier += 1;
            }
        }
        else
        {
            _isComboActive = false;
            _scoreValue -= _comboMultiplier > 0 ? 10f * _comboMultiplier : 10f;
            StartCoroutine(LoseScore(_comboMultiplier > 1 ? 2 + 2 * _comboMultiplier : 2));
            _activeComboCooldownTimer = 0f;
            _comboMultiplier = 0;
            combo.color = new Color(0f, 0f, 0f, 0f);
            _scoreValue = Mathf.Max(0f, _scoreValue);
            score.text = Mathf.CeilToInt(_scoreValue).ToString();
            
            for (var i = 0; i < _p1Stars.Length; i++)
            {
                _p1Stars[i]?.SetActive(false);
                _p2Stars[i]?.SetActive(false);
            }
        }

        if (_comboMultiplier > 1f)
        {
            combo.text = _comboMultiplier + "x Combo";
            combo.fontSize = ComboMultiplierFontSize + _comboMultiplier / 2f;
            combo.color = _comboInitialColor;
            combo.rectTransform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-5f, 5f));
        }

        combo.fontSize = _comboInitialFontSize + _comboMultiplier * 0.75f;

        if (_comboMultiplier > 45 || _p1Stars.Length <= 0 || _p2Stars.Length <= 0) return;

        var index = _comboMultiplier / 5 * 2;
        
        _p1Stars[index]?.SetActive(true);
        _p1Stars[index + 1]?.SetActive(true);
        
        _p2Stars[index]?.SetActive(true);
        _p2Stars[index + 1]?.SetActive(true);
    }

    private IEnumerator LoseScore(int objects)
    {
        for (var i = 0; i < objects; i++)
        {
            var scorePosition = score.rectTransform.TransformPoint(score.rectTransform.pivot);
            var lostScore = Instantiate(lostScorePrefab, scorePosition, Quaternion.identity);
            lostScore.GetComponent<LoseScore>().scorePosition = scorePosition;
        }
        
        yield return null;
    }

    #endregion
    
    private void EndTimer()
    {
        finalScoreBox.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(returnToMenuButton);
        finalScore.text = "Score: " + score.text;
        _timeScale = 0f;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void ReturnToMenu()
    {
        DataStorageManager.Instance.SaveEntry(_scoreValue, _comboMultiplier);
        GameManager.Instance.LoadScene("Menu");
    }
}
