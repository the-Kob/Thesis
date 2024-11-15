using System.Collections;
using Player;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    
    [SerializeField] private PlayerController p1;
    [SerializeField] private PlayerController p2;
    
    [SerializeField] private TextMeshProUGUI message;
    
    [HideInInspector] public int currentTutorialStep = -1;
    private int _lastTutorialStep;
    
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

        StartCoroutine(NextTutorialPhase());
    }

    // Update is called once per frame
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

            default:
                break;
        }
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
        
        yield return new WaitForSeconds(1f);
        
        currentTutorialStep = _lastTutorialStep + 1;

        /* Handle the last phase where the tutorial ends
        if (currentTutorialStep > 6)
        {
            continueText.SetActive(true);
        }

        spawnedEnemies = false;
        */
        yield return null;
    }
}
