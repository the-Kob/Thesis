using System.Collections;
using Enemy;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private PlayerController p1;
    [SerializeField] private PlayerController p2;

    [SerializeField] private GameObject[] p1Arrows;
    [SerializeField] private GameObject[] p2Arrows;

    [SerializeField] private GameObject p1EnemyPrefab;
    [SerializeField] private GameObject p2EnemyPrefab;

    [SerializeField] private TextMeshProUGUI message;

    [SerializeField] private GameObject continueText;

    [SerializeField] private GameObject book;
    [SerializeField] private Image bookImage;
    [SerializeField] private TextMeshProUGUI bookText;
    [SerializeField] private Sprite[] bookImages;
    [SerializeField] private Image[] bookSteps;
    [SerializeField] private Sprite progressBarFullSprite;
    private int _bookStep;
    [SerializeField] private TextMeshProUGUI pageCounter;
    
    [HideInInspector] public int currentTutorialStep = 0;
    private int _lastTutorialStep;
    
    private bool _enemiesHaveBeenSpawned;
    private int _nEnemies;
    
    [HideInInspector] public bool p1UsedBuff;
    [HideInInspector] public bool p1UsedDebuff;
    [HideInInspector] public bool p2UsedBuff;
    [HideInInspector] public bool p2UsedDebuff;
    
    private readonly string[] _tutorialStepSentences = {
        "Use left joystick to move",
        "Use right joystick to aim",
        "Press R1 or R2 to shoot",
        "Kill these enemies",
        "Use L1 to debuff YOUR enemies",
        "Press Square to use your secondary attack and kill these enemies",
        "Use L1 to buff EACH-OTHERS enemies and kill them",
        "You can't kill them, right?",
        "Just as we expected...",
        "Ok, here. Read these instructions."
    };
    
    private readonly string[] _bookStepSentences = {
        "This book was made on the go, we do not have much time. Hopefully this explanation is enough. Handle with care. -Rufus",
        "Our team of researchers were doing significant progress on understanding parallel universes. Doing so would bring a big new boom to science as a whole.",
        "But, like everything in the world, people moved by greed (the rebels) were trying to get their hands onto our research. They tried so much that eventually...",
        "They succeeded.",
        "And like the smart people they are (were), decided to activate the prototype. Our team was the only shielded from such disaster. And, somehow, you.",
        "And just like that, what was once one universe...",
        "Became two.",
        "We are here.",
        "This is creating what we call the 'Splitters', holographic creatures who are visible in both universes",
        "But we can only destroy the Splitters of our universe, which will not be enough",
        "Luckily we found we can have influence on the other universe in the form of pulse forces, making those Splitters easier or harder to kill.",
        "We were able to establish a connection with the other universe's researchers, who claim one of these 4 people might be your other universe self.",
        "We only got you. Your job is to first understand which companion from the other universe is, indeed, yourself, and slay some Splitters together.",
        "From what we observe, making splitters harder to kill will make them tougher, get less impact from shots, and slightly increase their movement speed.",
        "Why would we make them harder to kill? To Sync, of course.",
        "The more score you and your companion do, the bigger the odds of syncing both universes.",
        "You, or your companion, can also make them easier to kill, and try to sync by combo-ing, as we like to call it",
        "Killing multiple Splitters in a small period of time will create a combo, meaning each consequent kill will give more sync energy.",
        "But be careful, getting hit while the combo is high is really damaging to the connection between both universes and you wont be able to gather energy for a while",
        "This is just a hunch, but we do think your connection with your companion can be even more important than the sync energy we gather. Don't forget he is in another whole universe, light-years from us. If you can sync between yourselves, the universes might be able to sync as well",
        "Wish you the best of lucks. Choose wisely."
    };
    
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
    }
    
    private void Start()
    {
        Random.InitState(42);

        DisableArrows();
        book.SetActive(false);
        
        StartCoroutine(NextTutorialPhase());
    }
    
    private void Update()
    {
        if (message.text == "" && currentTutorialStep < _tutorialStepSentences.Length && currentTutorialStep >= 0)
        {
            Debug.Log("Current Tutorial Step: " + currentTutorialStep);
            message.text = _tutorialStepSentences[currentTutorialStep];
        }
        
        switch (currentTutorialStep)
        {
            case -1:
                break;
            case 0:
                if (p1.HasMoved && p2.HasMoved)
                {
                    StartCoroutine(NextTutorialPhase());
                }
                break;
            case 1:
                if (p1.HasAimed && p2.HasAimed)
                {
                    StartCoroutine(NextTutorialPhase());
                }
                break;
            case 2:
                if (p1.HasFired && p2.HasFired)
                {
                    StartCoroutine(NextTutorialPhase());
                }
                break;
            case 3:
                if (!_enemiesHaveBeenSpawned)
                {
                    SpawnTutorialEnemies(true);
                }
                else
                {
                    if (_nEnemies == 0)
                    {
                        StartCoroutine(NextTutorialPhase());
                    }
                }
                break;
            case 4:
                if (!_enemiesHaveBeenSpawned)
                {
                    p1Arrows[0].SetActive(true);
                    p2Arrows[0].SetActive(true);
                    SpawnTutorialEnemies();
                }
                else
                {
                    if (p1UsedDebuff)
                    {
                        DisableArrows(true);
                    }
                    
                    if (p2UsedDebuff)
                    {
                        DisableArrows(false);
                    }
                    
                    if (p1UsedDebuff && p2UsedDebuff)
                    {
                        message.color = new Color(0f, 204f / 255f, 0f, 1f);
                        
                        if(_nEnemies == 0) StartCoroutine(NextTutorialPhase());
                    }
                    else
                    {
                        if (_nEnemies == 0) _enemiesHaveBeenSpawned = false;
                        
                        StartCoroutine(ChangeSecondArrowVisibility(p1.isInEffectMenu, true));
                        StartCoroutine(ChangeSecondArrowVisibility(p2.isInEffectMenu, false));
                    }
                }
                break;
            case 5:
                p1Arrows[2].SetActive(true);
                p2Arrows[2].SetActive(true);

                if (!_enemiesHaveBeenSpawned)
                {
                    SpawnTutorialEnemies();
                }
                else
                {
                    if (_nEnemies == 0)
                    {
                        p1.canFire = true;
                        p1Arrows[2].SetActive(false);
                
                        p2.canFire = true;
                        p2Arrows[2].SetActive(false);
                        
                        StartCoroutine(NextTutorialPhase());
                    }
                }
                
                break;
            case 6:
                if (!_enemiesHaveBeenSpawned)
                {
                    p1Arrows[0].SetActive(true);
                    p2Arrows[0].SetActive(true);
                    
                    SpawnTutorialEnemies(numberOfEnemiesToSpawn: 6);
                }
                else
                {
                    if (p1UsedBuff)
                    {
                        DisableArrows(true);
                    }

                    if (p2UsedBuff)
                    {
                        DisableArrows(false);
                    }
                    
                    if (p1UsedBuff && p2UsedBuff && _nEnemies == 0)
                    {
                        StartCoroutine(NextTutorialPhase());
                    }
                    else
                    {
                        if (_nEnemies == 0) _enemiesHaveBeenSpawned = false;
                        
                        StartCoroutine(ChangeFourthArrowVisibility(p1.isInEffectMenu, true));
                        StartCoroutine(ChangeFourthArrowVisibility(p2.isInEffectMenu, false));
                    }
                }
                
                break;
        }
    }

    private void SpawnTutorialEnemies(bool changeEnemies = false, int numberOfEnemiesToSpawn = 10)
    {
        // The number of enemies to spawn should be even so the number of enemies is equal for both players
        _nEnemies = numberOfEnemiesToSpawn;
        
        for(var i = 0; i < _nEnemies; i++)
        {
            var firstHalf = i < _nEnemies / 2;
            var player = firstHalf ? p1 : p2;
            var playerDirection = new Vector3(player.MoveInput.x, player.MoveInput.y, 0).normalized * 10;
            var enemyPrefab = firstHalf ? p1EnemyPrefab : p2EnemyPrefab;

            var enemyOffset = playerDirection.magnitude > 1f
                ? Quaternion.Euler(0, 0, Random.Range(-45, 45)) * playerDirection
                : Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(10f, 15f), Random.Range(10f, 15f), 0);
            
            var enemy = Instantiate(
                enemyPrefab,
                player.transform.position + enemyOffset,
                Quaternion.identity);

            var enemyBehavior = enemy.GetComponent<EnemyBehaviour>();
            enemyBehavior.isTutorial = true;
            
            if (changeEnemies) ChangeEnemyOnSpawn(enemyBehavior, i);
        }
        _enemiesHaveBeenSpawned = true;
    }

    private static void ChangeEnemyOnSpawn(EnemyBehaviour enemy, int index)
    {
        switch (index)
        {
            case 1:
            case 6:
                enemy.Buff();
                break;
            case 2:
            case 7:
                enemy.GetHit(Vector3.zero, 0f, 1f);
                break;
            case 3:
            case 8:
                enemy.GetHit(Vector3.zero, 0f, 1f);
                enemy.GetHit(Vector3.zero, 0f, 1f);
                break;
            case 4:
            case 9:
                enemy.Nerf();
                break;
        }
    }

    public void DecreaseEnemyCount()
    {
        _nEnemies -= 1;
    }
    
    private IEnumerator NextTutorialPhase()
    {
        _lastTutorialStep = currentTutorialStep;
        
        currentTutorialStep = -1;
        
        message.color = new Color(0f, 204f / 255f, 0f, 1f);
        var temporaryColor = message.color;
        
        while (temporaryColor.a > 0f)
        {
            temporaryColor.a -= 0.01f;
            message.color = temporaryColor;
            yield return new WaitForFixedUpdate();
        }
        
        message.text = "";
        message.color = new Color(1f, 1f, 1f, 1f);
        
        p1.ResetFlags();
        p2.ResetFlags();
        p1UsedBuff = false;
        p1UsedDebuff = false;
        p2UsedBuff = false;
        p2UsedDebuff = false;
        
        yield return new WaitForSeconds(1f);
        
        currentTutorialStep = _lastTutorialStep + 1;
        
        if (currentTutorialStep > 6)
        {
            continueText.SetActive(true);

            var p1Input = p1.gameObject.GetComponent<PlayerInput>();
            p1Input.SwitchCurrentActionMap("UI");
            
            var p2Input = p2.gameObject.GetComponent<PlayerInput>();
            p2Input.SwitchCurrentActionMap("UI");
        }
        
        _enemiesHaveBeenSpawned = false;
        
        yield return null;
    }

    private void DisableArrows(bool? player1 = null)
    {
        switch (player1)
        {
            case null:
                foreach (var arrow in p1Arrows)
                {
                    arrow.SetActive(false);
                }

                foreach (var arrow in p2Arrows)
                {
                    arrow.SetActive(false);
                }
                break;

            case true:
                foreach (var arrow in p1Arrows)
                {
                    arrow.SetActive(false);
                }
                break;

            case false:
                foreach (var arrow in p2Arrows)
                {
                    arrow.SetActive(false);
                }
                break;
        }
    }
    
    private IEnumerator ChangeSecondArrowVisibility(bool showSecond, bool isPlayer1)
    {
        yield return new WaitForFixedUpdate();

        switch (isPlayer1)
        {
            case true:
                if (UIManager.Instance.p1EffectsMenu.activeSelf)
                {
                    p1Arrows[0].SetActive(!showSecond);
                    p1Arrows[1].SetActive(showSecond);
                }
                break;

            case false:
                if (UIManager.Instance.p2EffectsMenu.activeSelf)
                {
                    p2Arrows[0].SetActive(!showSecond);
                    p2Arrows[1].SetActive(showSecond);
                }
                break;
        }
        
        yield return null;
    }
    
    private IEnumerator ChangeFourthArrowVisibility(bool showFourth, bool isPlayer1)
    {
        yield return new WaitForFixedUpdate();

        switch (isPlayer1)
        {
            case true:
                if (UIManager.Instance.p1EffectsMenu.activeSelf)
                {
                    p1Arrows[0].SetActive(!showFourth);
                    p1Arrows[3].SetActive(showFourth);
                }
                break;

            case false:
                if (UIManager.Instance.p2EffectsMenu.activeSelf)
                {
                    p2Arrows[0].SetActive(!showFourth);
                    p2Arrows[3].SetActive(showFourth);
                }
                break;
        }
        
        yield return null;
    }

    internal void GetEffectFromPlayer(bool isPlayer1, int effect)
    {
        if (currentTutorialStep == 4) 
        {
            if (effect == 3 && isPlayer1)
            {
                p1UsedDebuff = true;
            }
            else if(effect == 1 && !isPlayer1)
            {
                p2UsedDebuff = true;
            }
        }
        
        if (currentTutorialStep == 6)
        {
            if (effect == 0 && isPlayer1)
            {
                p1UsedBuff = true;
            }
            else if(effect == 2 && !isPlayer1)
            {
                p2UsedBuff = true;
            }
        }
    }
    
    private void HandleSubmit()
    {
        if (currentTutorialStep + _bookStep == _tutorialStepSentences.Length + _bookStepSentences.Length)
        {
            EndTutorial();
        }
        else if (_bookStep > 0)
        {
            ShowNextBookPage();
        }
        
        if (currentTutorialStep > 6 && _bookStep < 1)
        {
            currentTutorialStep++;
            message.text = "";
        }

        if (currentTutorialStep == _tutorialStepSentences.Length && _bookStep < 1)
        {
            ActivateBook();
        }
    }
    
    private void EndTutorial()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        GameManager.Instance.MarkTutorialDone();
        GameManager.Instance.LoadScene("Menu");
    }

    private void ActivateBook()
    {
        book.SetActive(true);
        bookSteps[_bookStep].sprite = progressBarFullSprite;
        _bookStep = 1;
        pageCounter.text = $"Page {_bookStep} of {_bookStepSentences.Length}";
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;

    }
    
    private void ShowNextBookPage()
    {
        bookText.text = _bookStepSentences[_bookStep];
        bookImage.sprite = bookImages[_bookStep];
        bookSteps[_bookStep].sprite = progressBarFullSprite;
        _bookStep++;
        pageCounter.text = $"Page {_bookStep} of {_bookStepSentences.Length}";
    }
    
    private void OnEnable()
    {
        PlayerController.OnSubmitPressed += HandleSubmit;
    }

    private void OnDisable()
    {
        PlayerController.OnSubmitPressed -= HandleSubmit;
    }

}
