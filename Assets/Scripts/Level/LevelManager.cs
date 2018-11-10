using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Rewired;

public class LevelManager : MonoBehaviour {
    public ResultsScreen mpResultsScreen;
    public ResultsScreen spResultsScreen;
    public ResultsScreen continueScreen;
    public PauseMenu pauseMenu;
    public Text marginMultiplierText;

    public bool continueLevel;
    public bool mirroredLevel;
    public float marginMultiplier = 1f;

    public bool gameStarted = false;
    public bool setOver = false; // If the entire 2/3 set is finished

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
        if (!_gameOver && gameStarted) {
            _levelTimer += Time.deltaTime;


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

    public void PauseGame(int playerID) {
        // Check for pausing
        if (!_gameManager.isPaused && !_gameManager.isOnline) {
            // Pause the game
            pauseMenu.Activate(playerID);
        }
    }

    public void GameEnd() {
        _gameOver = true;

        // If we won and are in a timed mode, set the time highscore
        if(_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            // Add score based on completion speed
            BubbleManager[] _bManagers = FindObjectsOfType<BubbleManager>();
            int timeScore = 5000;
            foreach (BubbleManager _bM in _bManagers) {
                // Time score is lower the longer it takes to finish the level
                timeScore = timeScore - 10 * (int)_levelTimer;
                if(timeScore < 10) {
                    timeScore = 10;
                }
                _bM.IncreaseScore(timeScore);
            }
        } else if(_gameManager.gameMode == GAME_MODE.SURVIVAL) {
            // Add the time survived to the player's score
            BubbleManager[] _bManagers = FindObjectsOfType<BubbleManager>();
            foreach(BubbleManager _bM in _bManagers) {
                _bM.IncreaseScore(50 * (int)_levelTimer);
            }
        }
    }

    // TODO: All of this results screen activation code is pretty unreadable, especially with the singleplayer/versus continue overlaps and crap
    // Need to somehow refactor this to make it less all over the place.

    // result: -1 = loss, 0 = draw, 1 = win
    public void ActivateResultsScreen(int team, int result) {
        // If this was a versus match
        if (!_gameManager.isSinglePlayer || _gameManager.gameMode == GAME_MODE.MP_VERSUS) {
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
        } else if (_gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            // TODO: make a different results screen for these modes
            ActivateFinalResultsScreen(team, 1);
        // If this was a single player level
        } else {
            if (continueLevel) {
                ActivateContinueScreen(team, result);
            } else {
                ActivateFinalResultsScreen(team, result);
            }
        }
    }

    void ActivateContinueScreen(int team, int result) {
        if(continueScreen != null) {
            if(_gameManager.IsStoryLevel() && _gameManager.gameMode != GAME_MODE.MP_VERSUS) {
                // If it's the player
                if (team == 0) {
                    continueScreen.Activate(result == 1);

                // If it's the enemy
                } else {
                    continueScreen.Activate(result == -1);
                }
            } else {
                if (result == 1) {
                    continueScreen.Activate(team);
                } else if(result == -1) {
                    // The other team won so send that team
                    continueScreen.Activate(team == 1 ? 0 : 1);
                } else {
                    // It's a draw so display that
                    continueScreen.Activate(-1);
                }
            }
        }
    }

    void ActivateFinalResultsScreen(int team, int result) {
        setOver = true;
        if(_gameManager.IsStoryLevel() && spResultsScreen != null) {
            // If it's the player
            if (team == 0) {
                // If there is a cutscene after this stage, show a continue screen instead
                if (_gameManager.nextCutscene != "") {
                    continueScreen.Activate(true);
                } else if (_gameManager.gameMode == GAME_MODE.MP_VERSUS && !setOver) {
                    continueScreen.Activate(team);
                } else {
                    spResultsScreen.Activate(result == 1);
                }
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