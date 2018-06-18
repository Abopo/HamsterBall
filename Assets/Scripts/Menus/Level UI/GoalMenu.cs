using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMenu : MonoBehaviour {
    public Text goalText;
    public Text goalRequirement;
    public Text conditionText;
    public Text conditionLeftText;

    GameManager _gameManager;
    BubbleManager _bubbleManager;
    float _conditionLeft;

    PlayerController[] pCons;

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _bubbleManager = FindObjectOfType<BubbleManager>();
        _conditionLeft = _gameManager.conditionLimit;
        conditionLeftText.text = _gameManager.conditionLimit.ToString();

        switch (_gameManager.gameMode) {
            case GAME_MODE.SP_CLEAR:
                // No goal text for this mode
                goalText.enabled = false;
                goalRequirement.enabled = false;
                conditionText.text = "Time";
                break;
            case GAME_MODE.SP_POINTS:
                goalText.text = "Score\n       Needed";
                conditionText.text = "Throws";
                // Get throw events from players
                pCons = FindObjectsOfType<PlayerController>();
                break;
            case GAME_MODE.SP_MATCH:
                goalText.text = "Matches\n       Needed";
                break;
        }

        goalRequirement.text = _gameManager.goalCount.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if(_gameManager.gameIsOver) {
            return;
        }

        switch(_gameManager.gameMode) {
            case GAME_MODE.SP_CLEAR:
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
