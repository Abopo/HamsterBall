using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using Rewired;

public enum BOARDS { FOREST = 0, MOUNTAIN, BEACH, CITY, CORPORATION, LABORATORY, AIRSHIP, NUM_STAGES };
public class LevelManager : MonoBehaviour {
    public ResultsScreen mpResultsScreen;
    public ResultsScreen spResultsScreen;
    public ResultsScreen continueScreen;
    public PauseMenu pauseMenu;
    public Text marginMultiplierText;

    public BOARDS board;

    public bool continueLevel;
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

    public bool GameOver {
        get { return _gameOver; }
        set { _gameOver = value; }
    }

    private void Awake() {
        pauseMenu = FindObjectOfType<PauseMenu>();
        _gameManager = FindObjectOfType<GameManager>();
        board = _gameManager.selectedBoard;
        LoadStagePrefab();

        SceneManager.sceneUnloaded += OnSceneExit;
    }

    // Use this for initialization
    void Start() {
        if (_gameManager.isSinglePlayer) {
            _bubbleManager = FindObjectOfType<BubbleManager>();
        }
        if (_gameManager.nextLevel != "" || _gameManager.nextCutscene != "") {
            continueLevel = true;
        } else {
            continueLevel = false;
        }

        _levelUI = GetComponentInChildren<LevelUI>();

        // Failsafe for a missing countdown
        GameCountdown gc = FindObjectOfType<GameCountdown>();
        if (gc == null) {
            gameStarted = true;
        }
    }

    void LoadStagePrefab() {
        string prefabPath = "Prefabs/Level/Boards/Multiplayer/";

		Debug.Log("Stop all events on Scene Switch");
        SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
		//Bubble.MusicVolume = Mathf.Lerp(Bubble.MusicVolume, 1f, Time.deltaTime = 5f);

        switch (board) {
            case BOARDS.FOREST:
                prefabPath += "ForestBoard";
                break;
            case BOARDS.MOUNTAIN:
                prefabPath += "MountainBoard";
                break;
            case BOARDS.BEACH:
                prefabPath += "BeachBoard";
                break;
            case BOARDS.CITY:
                prefabPath += "CityBoard";
                break;
            case BOARDS.CORPORATION:
                prefabPath += "CorporationBoard";
                break;
            case BOARDS.LABORATORY:
                prefabPath += "LaboratoryBoard";
                break;
            case BOARDS.AIRSHIP:
                prefabPath += "Airship Board";
                break;
        }

        Object stageObj = Resources.Load(prefabPath);
        Instantiate(stageObj);
    }

    public void GameStart() {
        gameStarted = true;
        //GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update() {
        if (!_gameOver && gameStarted) {
            _levelTimer += Time.deltaTime;


            // If we are playing versus
            if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY) {
                // Update margin stuff
                _marginTimer += Time.deltaTime;
                if (_marginTimer >= _marginTime) {
                    IncreaseMarginMultiplier();
                    _marginTime = 30f;
                    _marginTimer = 0f;
                }
            // If we are playing the single player Clear mode
            } else if (_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
                // Update pushing down the board stuff
                if (_gameManager.conditionLimit == 0) {
                    _pushTimer += Time.deltaTime;
                    if (_pushTimer > _pushTime) {
                        // Stop shaking
                        _bubbleManager.StopShaking();

                        // push down the board
                        _bubbleManager.PushBoardDown();

                        _pushTimer = 0;
                    } else if (_pushTime - _pushTimer < 5) {
                        // Shake the bubble manager
                        _bubbleManager.StartShaking();
                    }
                }
            }
        }

        // Testing stuff
        if(Input.GetKeyDown(KeyCode.M)) {
            bool paused;
            SoundManager.mainAudio.MusicMainEvent.getPaused(out paused);
            if (paused) {
                SoundManager.mainAudio.MusicMainEvent.setPaused(false);
            } else {
                SoundManager.mainAudio.MusicMainEvent.setPaused(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.K)) {
            GameEnd();
            ActivateResultsScreen(0, 1);
        }
    }

    void IncreaseMarginMultiplier() {
        marginMultiplier += 0.5f;
        if (marginMultiplier > 5) {
            marginMultiplier = 5;
        }

        marginMultiplierText.text = "x" + marginMultiplier.ToString();
        marginMultiplierText.fontSize = 9 + Mathf.CeilToInt(marginMultiplier);
        if (marginMultiplier == 1f) {
            marginMultiplierText.color = Color.black;
        } else if (marginMultiplier == 1.5f) {
            marginMultiplierText.color = Color.blue;
        } else if (marginMultiplier == 2f) {
            marginMultiplierText.color = Color.cyan;
        } else if (marginMultiplier == 2.5f) {
            marginMultiplierText.color = Color.green;
        } else if (marginMultiplier == 3f) {
            marginMultiplierText.color = Color.magenta;
        } else if (marginMultiplier == 3.5f) {
            marginMultiplierText.color = Color.yellow;
        } else if (marginMultiplier >= 4f) {
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

        // Stop the music
        //SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        // If we won and are in a timed mode, set the time highscore
        if (_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            // Add score based on completion speed
            BubbleManager[] _bManagers = FindObjectsOfType<BubbleManager>();
            int timeScore = 5000;
            foreach (BubbleManager _bM in _bManagers) {
                // Time score is lower the longer it takes to finish the level
                timeScore = timeScore - 10 * (int)_levelTimer;
                if (timeScore < 10) {
                    timeScore = 10;
                }
                _bM.IncreaseScore(timeScore);
            }
        } else if (_gameManager.gameMode == GAME_MODE.SURVIVAL) {
            // Add the time survived to the player's score
            BubbleManager[] _bManagers = FindObjectsOfType<BubbleManager>();
            foreach (BubbleManager _bM in _bManagers) {
                _bM.IncreaseScore(50 * (int)_levelTimer);
            }
        }
    }

    // TODO: All of this results screen activation code is pretty unreadable, especially with the singleplayer/versus continue overlaps and crap
    // Need to somehow refactor this to make it less all over the place.

    // team: 0 = left team, 1 = right team
    // result: -1 = lose, 0 = draw, 1 = win
    public void ActivateResultsScreen(int team, int result) {
        int finalResult = DetermineFinalResult(team, result);

        // If this was a versus match
        if (!_gameManager.isSinglePlayer || _gameManager.gameMode == GAME_MODE.MP_VERSUS) {
            // Deal with best 2/3 stuff
            // Draw
            if(finalResult == 0) {
                IncreaseLeftTeamGames();
                IncreaseRightTeamGames();
                if (_gameManager.leftTeamGames >= 2 && _gameManager.rightTeamGames >= 2) {
                    // the whole set was a draw
                    FindObjectOfType<GameEndSequence>().StartSequence(0);
                    //ActivateFinalResultsScreen(0);
                } else if (_gameManager.leftTeamGames >= 2) {
                    // Left team has won the set
                    // Activate final results screen
                    FindObjectOfType<GameEndSequence>().StartSequence(-1);
                    //ActivateFinalResultsScreen(-1);
                } else if (_gameManager.rightTeamGames >= 2) {
                    // Right team has won the set
                    // Activate final results screen
                    FindObjectOfType<GameEndSequence>().StartSequence(1);
                    //ActivateFinalResultsScreen(1);
                } else {
                    // Set not done so activate continue screen
                    continueScreen.Activate(finalResult);
                }
            } else {
                // If the left team has won
                if(finalResult == -1) {
                    IncreaseLeftTeamGames();
                    if(_gameManager.leftTeamGames >= 2) {
                        // Left team has won the set
                        FindObjectOfType<GameEndSequence>().StartSequence(-1);
                        //ActivateFinalResultsScreen(-1);
                    } else {
                        // Set still not won
                        // Activate Continue screen
                        continueScreen.Activate(finalResult);
                    }
                // If right team has won
                } else if(finalResult == 1) {
                    IncreaseRightTeamGames();
                    if (_gameManager.rightTeamGames >= 2) {
                        // Right team has won the set
                        // Activate final results screen with right team winning
                        FindObjectOfType<GameEndSequence>().StartSequence(1);
                        //ActivateFinalResultsScreen(1);
                    } else {
                        // Set still not won
                        // Activate Continue screen
                        continueScreen.Activate(finalResult);
                    }
                }
            }
        } else if (_gameManager.gameMode == GAME_MODE.SURVIVAL || _gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            // TODO: make a different results screen for these modes
            ActivateFinalResultsScreen(finalResult);
        // If this was a single player level
        } else {
            // If the player won and it's a continue level
            if (finalResult == -1 && continueLevel) {
                continueScreen.Activate(finalResult);
            } else {
                ActivateFinalResultsScreen(finalResult);
            }
        }
    }

    // result: -1 = left team wins, 0 = draw, 1 = right team wins
    public void ActivateFinalResultsScreen(int result) {
        setOver = true;

        // If we're in a single player or story stage
        if ((_gameManager.isSinglePlayer || _gameManager.stage[0] != 0) && spResultsScreen != null) {
            // If the player lost
            if (result != -1) {
                // Show a normal results screen
                spResultsScreen.Activate(result);
            // If the player won
            } else {
                // Show a continue screen if there's a cutscene
                if (_gameManager.nextCutscene != "") {
                    continueScreen.Activate(result);
                } else {
                    // Show a normal results screen
                    spResultsScreen.Activate(result);
                }
            }
            // If we are in multiplayer
        } else if (!_gameManager.isSinglePlayer && mpResultsScreen != null) {
            mpResultsScreen.Activate(result);
        } else {// TODO: failsafe some default results screen
            spResultsScreen.Activate(-1);
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

    // Returns a result: -1 = left team wins, 0 = draw, 1 = right team wins
    int DetermineFinalResult(int team, int result) {
        int finalResult = 0;
        if (result == 0) {
            finalResult = 0;
        } else if (team == 0) {
            if (result == -1) {
                // Left team lost/Right team won
                finalResult = 1;
            } else if (result == 1) {
                // Left team won/Right team lost
                finalResult = -1;
            }
        } else if (team == 1) {
            if (result == -1) {
                // Left team won/Right team lost
                finalResult = -1;
            } else if (result == 1) {
                // Left team lost/Right team won
                finalResult = 1;
            }
        }

        return finalResult;
    }

    public void NextGame() {
        // If we're in a story stage with a doc to load
        if (_gameManager.LevelDoc != null) {
            _gameManager.CleanUp(false);

            // If we're playing versus
            if(_gameManager.gameMode == GAME_MODE.MP_VERSUS) {
                // We don't need to reload the doc, just continue the game
                _gameManager.ContinueLevel();
            } else {
                // Otherwise we need to read the next doc and load the stage
                BoardLoader boardLoader = FindObjectOfType<BoardLoader>();
                boardLoader.ReadBoardSetup(_gameManager.LevelDoc);
            }
        } else if (!_gameManager.isOnline) {
            _gameManager.ContinueLevel();
        } else if (PhotonNetwork.connectedAndReady) {
            PhotonNetwork.RPC(GetComponent<PhotonView>(), "ReloadCurrentLevel", PhotonTargets.All, false);
        }
    }

    void OnSceneExit(Scene scene) {
        // Stop looping sounds here
        Debug.Log("Stop all events on Scene Exit");
		SoundManager.mainAudio.MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        SoundManager.mainAudio.MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        //SoundManager.mainAudio.MusicMainEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.ThrowAngleEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.ForestMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.ForestAmbienceEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.MountainMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.SnowAmbienceEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.BeachMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.BeachAmbienceEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		//SoundManager.mainAudio.MusicMainEvent.release();
        //SoundManager.mainAudio.ThrowAngleEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //SoundManager.mainAudio.ForestMusicEvent.release();
		//SoundManager.mainAudio.ForestAmbienceEvent.release();
		//SoundManager.mainAudio.MountainMusicEvent.release();
		//SoundManager.mainAudio.SnowAmbienceEvent.release();
		//SoundManager.mainAudio.BeachMusicEvent.release();
		//SoundManager.mainAudio.BeachAmbienceEvent.release();
	
    }
}