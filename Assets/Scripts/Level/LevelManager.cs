﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour {
    public PauseMenu pauseMenu;
    public Text marginMultiplierText;

    public float marginMultiplier = 1.00f;

    float _marginTimer = 0;
    float _marginTime = 96f;
    int _initialTargetPoints = 120;
    int _targetPoints;
    int _marginIterations = 0;
    int _prevTargetPoints;

    float _levelTimer;
    float _pushTimer; // timer for pushing the board down in single player
    float _pushTime = 30;
    bool _gameOver = false;

    GameManager _gameManager;
    BubbleManager _bubbleManager;

    public float LevelTimer {
        get { return _levelTimer; }
    }

    // Use this for initialization
    void Start () {
        _targetPoints = _initialTargetPoints;
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.gameOverEvent.AddListener(GameEnd);

        if(_gameManager.isSinglePlayer) {
            _bubbleManager = FindObjectOfType<BubbleManager>();
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Pause") && !_gameManager.isOnline) {
            // Pause the game
            pauseMenu.Activate();
        }

        if (!_gameOver) {
            _levelTimer += Time.deltaTime;

            // If we are not in single player
            if (!_gameManager.isSinglePlayer) {
                // Update margin stuff
                _marginTimer += Time.deltaTime;
                if (_marginTimer >= _marginTime && _targetPoints > 1 && _marginIterations < 14) {
                    IncreaseMarginMultiplier();
                    _marginTime = 32f;
                    _marginTimer = 0f;
                }
            } else {
                if (_gameManager.timeLimit == 0) {
                    _pushTimer += Time.deltaTime;
                    if (_pushTimer > _pushTime) {
                        // Stop shaking
                        _bubbleManager.StopShaking();

                        // push down the board
                        _bubbleManager.PushBoardDown();


                        _pushTimer = 0;
                    } else if(_pushTime - _pushTimer < 5) {
                        // Shake the bubble manager
                        _bubbleManager.StartShaking();
                    }
                }
            }
        }
    }

    void IncreaseMarginMultiplier() {
        int curTargetPoints = _targetPoints;
        if(_marginIterations == 0) {
            _targetPoints = (int)(_initialTargetPoints * 0.75f);
        } else {
            _targetPoints = (int)(_prevTargetPoints / 2);
        }

        _prevTargetPoints = curTargetPoints;
        _marginIterations++;

        marginMultiplier = _initialTargetPoints / (float)_targetPoints;
        marginMultiplierText.text = "x" + marginMultiplier.ToString("0.00");
    }

    void GameEnd() {
        _gameOver = true;

        // If we won and are in a timed mode, set the time highscore
        if(_gameManager.gameMode == GAME_MODE.SP_MATCH || _gameManager.gameMode == GAME_MODE.SP_POINTS || _gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            string pref = _gameManager.level + "Highscore";
            if ((int)_levelTimer < PlayerPrefs.GetInt(pref) || PlayerPrefs.GetInt(pref) == 0) {
                PlayerPrefs.SetInt(pref, (int)_levelTimer);
            }
        }
    }
}