using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMenu : MonoBehaviour {
    public Text goalText;
    public Text goalRequirement;
    public Text timeLeftText;

    GameManager _gameManager;
    BubbleManager _bubbleManager;
    float _timeLeft;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _bubbleManager = FindObjectOfType<BubbleManager>();
        _timeLeft = _gameManager.timeLimit;
        
        switch(_gameManager.gameMode) {
            case GAME_MODE.SP_POINTS:
                goalText.text = "Score\n       Needed";
                break;
            case GAME_MODE.SP_MATCH:
                goalText.text = "Matches\n       Needed";
                break;
        }

        goalRequirement.text = _gameManager.goalCount.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if(_gameManager.gameMode == GAME_MODE.SP_MATCH) {
            goalRequirement.text = (_gameManager.goalCount - _bubbleManager.matchCount).ToString();
        }

        _timeLeft -= Time.deltaTime;
        timeLeftText.text = string.Format("{0}:{1:00}", (int)_timeLeft / 60, (int)_timeLeft % 60);
	}
}
