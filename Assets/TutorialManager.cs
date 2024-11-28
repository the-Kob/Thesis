using System.Collections;
using Enemy;
using Player;
using TMPro;
using UnityEngine;

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

    [HideInInspector] public bool p1UsedBuff;
    [HideInInspector] public bool p1UsedDebuff;
    [HideInInspector] public bool p2UsedBuff;
    [HideInInspector] public bool p2UsedDebuff;
    
    [HideInInspector] public int currentTutorialStep = -1;
    private int _lastTutorialStep;
    private bool _enemiesHaveBeenSpawned;
    private int _nEnemies;
    
    private readonly string[] _tutorialStepSentences = {
        "Use left joystick to move",
        "Use right joystick to aim",
        "Press R1 or R2 to shoot",
        "Kill these enemies",
        "Use L1 to debuff your enemies",
        "Press Square to use your secondary attack and kill these enemies",
        "Use L1 to buff blue enemies and kill them",
        "You can't kill them, right?",
        "Just as we expected...",
        "Ok, here. Read these instructions."
    };
    
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
    
    private void Start()
    {
        Random.InitState(42);

        DisableArrows();
        
        StartCoroutine(NextTutorialPhase());
    }
    
    private void Update()
    {
        if (message.text == "" && currentTutorialStep < _tutorialStepSentences.Length)
        {
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
                p1.canFire = false;
                p1Arrows[2].SetActive(true);
                
                p2.canFire = false;
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

        /* Handle the last phase where the tutorial ends
        if (currentTutorialStep > 6)
        {
            continueText.SetActive(true);
        }
        */
        
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
        if(effect == 3 && currentTutorialStep == 4){
            if (isPlayer1)
            {
                p1UsedDebuff = true;
            }
            else
            {
                p2UsedDebuff = true;
            }
        }
        
        if(effect == 0 && currentTutorialStep == 6){
            if (isPlayer1)
            {
                p1UsedBuff = true;
            }
            else
            {
                p2UsedBuff = true;
            }
        }
    }
}
