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

    [HideInInspector] public int currentTutorialStep;
    private int _lastTutorialStep;
    
    private bool _enemiesHaveBeenSpawned;
    private int _nEnemies;
    
    [HideInInspector] public bool p1UsedBuff;
    [HideInInspector] public bool p1UsedDebuff;
    [HideInInspector] public bool p2UsedBuff;
    [HideInInspector] public bool p2UsedDebuff;
    
    private readonly string[] _tutorialStepSentences = {
        "Use the left joystick to move",
        "Use the right joystick to aim",
        "Press R1 or R2 to shoot",
        "Kill these enemies — pay attention to the colors",
        "Press L1 to debuff YOUR enemies",
        "Press Square to use your secondary attack and kill these enemies",
        "Use L1 to buff EACH OTHER’S enemies and then kill them",
        "You noticed how you can't kill the enemies of the other color, right?",
        "Just as we expected...",
        "Okay, here—read these instructions."
    };
    
    private readonly string[] _bookStepSentences = {
        "This book was made on the go—we don’t have much time. Hopefully, this explanation is enough. Handle with care. —Rufus",
        "Our team of researchers was making significant progress in understanding parallel universes. Doing so would bring a major breakthrough to science as a whole.",
        "But, like everything in the world, people driven by greed (the rebels) were trying to get their hands on our research. They tried so hard that eventually...",
        "They succeeded.",
        "And, being as clever as they were, they decided to activate the prototype. Our team was the only one shielded from the disaster. And, somehow, so were you.",
        "And just like that, what was once one universe...",
        "Became two.",
        "We are here.",
        "This split created what we call 'Splitters' — holographic creatures visible in both universes.",
        "But we can only destroy the Splitters from our own universe, which alone won't be enough.",
        "Luckily, we discovered we can influence the other universe using pulse forces, making those Splitters either easier or harder to kill.",
        "We managed to establish a connection with the other universe’s researchers and they sent your counterpart here. Hence you are two.",
        "We only have you guys. Your task is to slay some Splitters together.",
        "From what we’ve observed, making Splitters harder to kill makes them tougher, less affected by shots, and slightly faster.",
        "Why make them harder to kill? To sync, of course.",
        "The more points you and your other self score, the higher the odds of syncing both universes.",
        "You—or your other self—can also make them easier to kill, and attempt to sync by 'combo-ing', as we call it.",
        "Killing multiple Splitters in a short time creates a combo. Each consecutive kill generates more sync energy.",
        "But be careful—getting hit while the combo is high severely harms the connection between both universes. You won’t be able to gather energy for a while.",
        "Just a hunch—but we believe your connection with your other self might be even more important than the sync energy itself. Don’t forget, you are in separate universes, light-years away. If you two can sync, maybe the universes can too.",
        "Wishing you the best of luck."
    };


    private enum TutorialEnemySpawnMode
    {
        Both,
        OnlyP1,
        OnlyP2
    }
    
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
                    p1.canFire = false;
                    p2.canFire = false;
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

    private void SpawnTutorialEnemies(bool changeEnemies = false, int numberOfEnemiesToSpawn = 10, TutorialEnemySpawnMode spawnMode = TutorialEnemySpawnMode.Both)
	{
        if (spawnMode == TutorialEnemySpawnMode.Both)
        {
            // Ensure even number of enemies
            _nEnemies = numberOfEnemiesToSpawn % 2 == 0 ? numberOfEnemiesToSpawn : numberOfEnemiesToSpawn + 1;
        }
        else
        {
            _nEnemies = numberOfEnemiesToSpawn;
        }

        for (var i = 0; i < _nEnemies; i++)
        {
            PlayerController player = null;
            GameObject enemyPrefab = null;

            var spawnForP1 = spawnMode == TutorialEnemySpawnMode.OnlyP1 || (spawnMode == TutorialEnemySpawnMode.Both && i < _nEnemies / 2);
            var spawnForP2 = spawnMode == TutorialEnemySpawnMode.OnlyP2 || (spawnMode == TutorialEnemySpawnMode.Both && i >= _nEnemies / 2);

            if (spawnForP1)
            {
                player = p1;
                enemyPrefab = p1EnemyPrefab;
            }
            else if (spawnForP2)
            {
                player = p2;
                enemyPrefab = p2EnemyPrefab;
            }

            if (player == null || enemyPrefab == null) continue;

            var playerDirection = new Vector3(player.MoveInput.x, player.MoveInput.y, 0).normalized * 10;

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
