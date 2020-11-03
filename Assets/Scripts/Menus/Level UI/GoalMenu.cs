using UnityEngine;
using UnityEngine.UI;

public class GoalMenu : MonoBehaviour {
    public Image backer;
    public SuperTextMesh stageName;
    public GameObject scoreStuff;
    public SuperTextMesh goalText;
    public SuperTextMesh goalRequirement;
    public SuperTextMesh conditionText;
    public SuperTextMesh conditionLeftText;
    public SuperTextMesh timeHeader;
    public SuperTextMesh timeText;

    int seconds;
    int minutes;

    bool _gameOver = false;

    GameManager _gameManager;
    LevelManager _levelManager;
    BubbleManager _bubbleManager;
    float _conditionLeft;

    // Use this for initialization
    void Start () {
        _gameManager = GameManager.instance;
        _levelManager = FindObjectOfType<LevelManager>();
        _bubbleManager = FindObjectOfType<BubbleManager>();
        _conditionLeft = _gameManager.conditionLimit;
        conditionLeftText.text = _gameManager.conditionLimit.ToString();

        SetupMenuDisplay();

        // Find and set the stage name
        stageName.text = _gameManager.stageName;

        goalRequirement.text = _gameManager.goalCount.ToString();

        _gameManager.gameOverEvent.AddListener(GameOver);
	}

    void SetupMenuDisplay() {
        switch (_gameManager.gameMode) {
            case GAME_MODE.MP_VERSUS:
                gameObject.SetActive(false);
                break;
            case GAME_MODE.SP_CLEAR:
                // Display stage name
                stageName.gameObject.SetActive(true);
                // Display total time
                timeHeader.gameObject.SetActive(true);
                timeText.gameObject.SetActive(true);
                // No goal or condition text for this mode
                scoreStuff.SetActive(false);
                goalText.gameObject.SetActive(false);
                goalRequirement.gameObject.SetActive(false);
                conditionText.gameObject.SetActive(false);
                conditionLeftText.gameObject.SetActive(false);
                break;
            case GAME_MODE.SP_POINTS:
                // Hide stage name
                stageName.gameObject.SetActive(false);
                // Don't display total time
                timeHeader.gameObject.SetActive(false);
                timeText.gameObject.SetActive(false);
                // Display goal stuff
                scoreStuff.SetActive(true);
                goalText.gameObject.SetActive(true);
                goalRequirement.gameObject.SetActive(true);
                conditionText.gameObject.SetActive(true);
                conditionLeftText.gameObject.SetActive(true);

                break;
            case GAME_MODE.SP_MATCH:
                goalText.text = "Matches\n       Needed";
                break;
            case GAME_MODE.SURVIVAL:
                // Hide stage name
                stageName.gameObject.SetActive(false);
                // Display total time and score I guess?
                timeHeader.gameObject.SetActive(true);
                timeText.gameObject.SetActive(true);
                scoreStuff.SetActive(true);
                // No goal or condition text for this mode
                goalText.gameObject.SetActive(false);
                goalRequirement.gameObject.SetActive(false);
                conditionText.gameObject.SetActive(false);
                conditionLeftText.gameObject.SetActive(false);
                break;
        }

        // Load backer sprite
        Sprite[] backerSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Special-Stage-Boxes-GUI");
        backer.sprite = backerSprites[(int)_gameManager.selectedBoard+1];
    }

    // Update is called once per frame
    void Update () {
        if(_gameOver) {
            return;
        }

        // This is the TOTAL TIME so we need to add the timeOverflow
        seconds = Mathf.FloorToInt((_gameManager.timeOverflow + _levelManager.LevelTimer) % 60);
        minutes = Mathf.FloorToInt((_gameManager.timeOverflow + _levelManager.LevelTimer) / 60);
        timeText.text = string.Format("{0}:{1:00}", minutes, seconds);

        switch (_gameManager.gameMode) {
            case GAME_MODE.SP_CLEAR:
                break;
            case GAME_MODE.SURVIVAL:
                if (_gameManager.conditionLimit > 0) {
                    _conditionLeft -= Time.deltaTime;
                } else {
                    _conditionLeft += Time.deltaTime;
                }

                conditionLeftText.text = string.Format("{0}:{1:00}", (int)_conditionLeft / 60, (int)_conditionLeft % 60);
                break;
            case GAME_MODE.SP_POINTS:
                //_conditionLeft = _gameManager.conditionLimit - PlayerController.totalThrowCount;
                conditionLeftText.text = PlayerController.totalThrowCount.ToString() + " / " + _gameManager.conditionLimit.ToString();
                break;
            case GAME_MODE.SP_MATCH:
                goalRequirement.text = (_gameManager.goalCount - _bubbleManager.matchCount).ToString();
                break;
        }
    }

    void GameOver() {
        _gameOver = true;
    }
}
