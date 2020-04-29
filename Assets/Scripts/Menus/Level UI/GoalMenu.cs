using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMenu : MonoBehaviour {
    public SuperTextMesh goalText;
    public SuperTextMesh goalRequirement;
    public SuperTextMesh conditionText;
    public SuperTextMesh conditionLeftText;

    public SuperTextMesh timeText;
    int seconds;
    int minutes;

    GameManager _gameManager;
    LevelManager _levelManager;
    BubbleManager _bubbleManager;
    float _conditionLeft;

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();
        _bubbleManager = FindObjectOfType<BubbleManager>();
        _conditionLeft = _gameManager.conditionLimit;
        conditionLeftText.text = _gameManager.conditionLimit.ToString();

        switch (_gameManager.gameMode) {
            case GAME_MODE.MP_VERSUS:
                gameObject.SetActive(false);
                break;
            case GAME_MODE.SP_CLEAR:
                // No goal or condition text for this mode
                goalText.gameObject.SetActive(false);
                goalRequirement.gameObject.SetActive(false);
                conditionText.gameObject.SetActive(false);
                conditionLeftText.gameObject.SetActive(false);
                break;
            case GAME_MODE.SP_POINTS:
                goalText.text = "Score\n       Needed";
                conditionText.text = "Throws";
                break;
            case GAME_MODE.SP_MATCH:
                goalText.text = "Matches\n       Needed";
                break;
            case GAME_MODE.SURVIVAL:
                goalText.text = "";
                conditionText.text = "Time";
                break;
        }

        goalRequirement.text = _gameManager.goalCount.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if(_gameManager.gameIsOver) {
            return;
        }

        seconds = Mathf.FloorToInt(_levelManager.LevelTimer % 60);
        minutes = Mathf.FloorToInt(_levelManager.LevelTimer / 60);
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
                _conditionLeft = _gameManager.conditionLimit - PlayerController.totalThrowCount;
                conditionLeftText.text = _conditionLeft.ToString();
                break;
            case GAME_MODE.SP_MATCH:
                goalRequirement.text = (_gameManager.goalCount - _bubbleManager.matchCount).ToString();
                break;
        }
    }

}
