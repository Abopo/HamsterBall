using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour {
    public ResultsScreen mpResultsScreen;
    public ResultsScreen spResultsScreen;
    public ResultsScreen spContinueScreen;
    public PauseMenu pauseMenu;
    public Text marginMultiplierText;

    public bool continueLevel;
    public bool mirroredLevel;
    public float marginMultiplier = 1f;

    float _marginTimer = 0;
    float _marginTime = 120f;

    float _levelTimer;
    float _pushTimer; // timer for pushing the board down in single player
    float _pushTime = 30;
    bool _gameOver = false;

    GameManager _gameManager;
    BubbleManager _bubbleManager;
    LevelUI _levelUI;

    public float LevelTimer {
        get { return _levelTimer; }
    }

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Use this for initialization
    void Start () {
        _gameManager.gameOverEvent.AddListener(GameEnd);

        if (_gameManager.isSinglePlayer) {
            _bubbleManager = FindObjectOfType<BubbleManager>();
        }
        if (_gameManager.nextLevel != "" || _gameManager.nextCutscene != "") {
            continueLevel = true;
        } else {
            continueLevel = false;
        }

        _levelUI = GetComponentInChildren<LevelUI>();
    }

    // Update is called once per frame
    void Update () {
        if (!_gameOver) {
            _levelTimer += Time.deltaTime;

            // Check for pausing
            if (Input.GetButtonDown("Pause") && !_gameManager.isPaused && !_gameManager.isOnline) {
                // Pause the game
                pauseMenu.Activate();
            }

            // If we are playing versus
            if (_gameManager.gameMode == GAME_MODE.MP_VERSUS) {
                // Update margin stuff
                _marginTimer += Time.deltaTime;
                if (_marginTimer >= _marginTime) {
                    IncreaseMarginMultiplier();
                    _marginTime = 30f;
                    _marginTimer = 0f;
                }
            // If we are playing the single player Clear mode
            } else if(_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
                // Update pushing down the board stuff
                if (_gameManager.conditionLimit == 0) {
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
        marginMultiplier += 0.5f;
        if(marginMultiplier > 9) {
            marginMultiplier = 9;
        }
        
        marginMultiplierText.text = "x" + marginMultiplier.ToString();
        marginMultiplierText.fontSize = 10 + 2 * Mathf.CeilToInt(marginMultiplier);
        if (marginMultiplier == 1f) {
            marginMultiplierText.color = Color.black;
        } else if(marginMultiplier == 1.5f) {
            marginMultiplierText.color = Color.blue;
        } else if (marginMultiplier == 2f) {
            marginMultiplierText.color = Color.cyan;
        } else if (marginMultiplier == 2.5f) {
            marginMultiplierText.color = Color.green;
        } else if (marginMultiplier == 3f) {
            marginMultiplierText.color = Color.magenta;
        } else if (marginMultiplier == 3.5f) {
            marginMultiplierText.color = Color.yellow;
        } else if (marginMultiplier == 4f) {
            marginMultiplierText.color = Color.red;
        } else if (marginMultiplier == 4.5f) {
            marginMultiplierText.color = Color.red;
        } else if (marginMultiplier == 5f) {
            marginMultiplierText.color = Color.red;
        }
    }

    void GameEnd() {
        _gameOver = true;

        // If we won and are in a timed mode, set the time highscore
        if(_gameManager.gameMode == GAME_MODE.SP_MATCH || _gameManager.gameMode == GAME_MODE.SP_POINTS || _gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            string pref = _gameManager.stage + "Highscore";
            if ((int)_levelTimer < PlayerPrefs.GetInt(pref) || PlayerPrefs.GetInt(pref) == 0) {
                PlayerPrefs.SetInt(pref, (int)_levelTimer);
            }
        } else if(_gameManager.gameMode == GAME_MODE.SURVIVAL) {
            // Add the time survived to the player's score
            BubbleManager[] _bManagers = FindObjectsOfType<BubbleManager>();
            foreach(BubbleManager _bM in _bManagers) {
                _bM.IncreaseScore(50 * (int)_levelTimer);
            }
        }
    }

    // result: -1 = loss, 0 = draw, 1 = win
    public void ActivateResultsScreen(int team, int result) {
        // If this was a versus match
        if (!_gameManager.isSinglePlayer) {

            if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
                // TODO: make a different results screen for these modes
                ActivateFinalResultsScreen(team, 1);
            } else {
                // Deal with best 2/3 stuff
                // Draw
                if (result == 0) {
                    IncreaseLeftTeamGames();
                    IncreaseRightTeamGames();
                    if (_gameManager.leftTeamGames >= 2 && _gameManager.rightTeamGames >= 2) {
                        // the whole set was a draw
                        ActivateFinalResultsScreen(team, 0);
                    } else if (_gameManager.leftTeamGames >= 2) {
                        // Left team has won the set
                        // Activate final results screen
                        ActivateFinalResultsScreen(team, result);
                    } else if (_gameManager.rightTeamGames >= 2) {
                        // Right team has won the set
                        // Activate final results screen
                        ActivateFinalResultsScreen(team, result);
                    } else {
                        // Set not done so activate continue screen
                        ActivateContinueScreen(0, 0);
                    }
                } else {
                    // If Left team wins
                    if (team == 0 && result == 1 || team == 1 && result == -1) {
                        IncreaseLeftTeamGames();
                        if (_gameManager.leftTeamGames >= 2) {
                            // Left team has won the set
                            // Activate final results screen with left team winning
                            ActivateFinalResultsScreen(0, 1);
                        } else {
                            // Set still not won
                            // Activate Continue screen
                            ActivateContinueScreen(0, 1);
                        }
                        // If Right team wins
                    } else if (team == 1 && result == 1 || team == 0 && result == -1) {
                        IncreaseRightTeamGames();
                        if (_gameManager.rightTeamGames >= 2) {
                            // Right team has won the set
                            // Activate final results screen with right team winning
                            ActivateFinalResultsScreen(1, 1);
                        } else {
                            // Set still not won
                            // Activate Continue screen
                            ActivateContinueScreen(1, 1);
                        }
                    }
                }
            }
        // If this was a single player level
        } else {
            if(continueLevel) {
                ActivateContinueScreen(team, result);
            } else {
                ActivateFinalResultsScreen(team, result);
            }
        }
    }

    void ActivateContinueScreen(int team, int result) {
        if(spContinueScreen != null) {
            if(_gameManager.IsStoryLevel()) {
                // If it's the player
                if (team == 0) {
                    spContinueScreen.Activate(result == 1);

                // If it's the enemy
                } else {
                    spContinueScreen.Activate(result == -1);
                }
            } else {
                if (result == 1) {
                    spContinueScreen.Activate(team);
                } else if(result == -1) {
                    // The other team won so send that team
                    spContinueScreen.Activate(team == 1 ? 0 : 1);
                } else {
                    // It's a draw so display that
                    spContinueScreen.Activate(-1);
                }
            }
        }
    }

    void ActivateFinalResultsScreen(int team, int result) {
        if(_gameManager.IsStoryLevel() && spResultsScreen != null) {
            // If it's the player
            if (team == 0) {
                spResultsScreen.Activate(result == 1);
            } else {
                spResultsScreen.Activate(result == -1);
            }
        } else if(!_gameManager.IsStoryLevel() && mpResultsScreen != null) {
            if (result != 0) {
                mpResultsScreen.Activate(team);
            } else {
                mpResultsScreen.Activate(-1);
            }
        }
    }

    void IncreaseLeftTeamGames() {
        _gameManager.leftTeamGames++;
        _levelUI.FillInGameMarker(0);
    }

    void IncreaseRightTeamGames() {
        _gameManager.rightTeamGames++;
        _levelUI.FillInGameMarker(1);
    }

    public void NextGame() {
        if (_gameManager.LevelDoc != null) {
            _gameManager.CleanUp(false);
            BoardLoader boardLoader = FindObjectOfType<BoardLoader>();
            boardLoader.ReadBoardSetup(_gameManager.LevelDoc);
        } else if(!_gameManager.isOnline) {
            _gameManager.ContinueLevel();
        } else if(PhotonNetwork.connectedAndReady) {
            PhotonNetwork.RPC(GetComponent<PhotonView>(), "ReloadCurrentLevel", PhotonTargets.All, false);
        }
    }
}