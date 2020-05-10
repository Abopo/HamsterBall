﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Rewired;

public enum GAME_MODE { SP_POINTS = 0, SP_MATCH, SP_CLEAR, MP_VERSUS, MP_PARTY, SURVIVAL, TEAMSURVIVAL, NUM_MODES }
public enum MENU { STORY = 0, VERSUS, EDITOR };

public class GameManager : MonoBehaviour {
    public bool testMode;
    public bool isOnline = false;
    public bool isSinglePlayer = false;
    public bool isCoop = false;
    public bool gameIsOver = false;
    public int[] stage = new int[2]; // if level is "" it's a local multiplayer match, otherwise it's a story level
    public int flowerRequirement1;
    public int flowerRequirement2;
    public BOARDS selectedBoard;
    public string nextLevel; // level to load next
    public string nextCutscene; // cutscene to load next

    public MENU prevMenu; // Keeps track of the last menu we were in so we can return after a level is finished
    public string prevBoard; // Holds onto the previous board if there was one

    public GAME_MODE gameMode;
    public int goalCount; // the number of points or matches to win the level
    public int conditionLimit; // the condition limit to achieve the goal i.e. time limit, throw limit, etc.
    public int scoreOverflow = 0; // this variable holds onto the player's score between stages with multiple boards
    public float timeOverflow = 0; // same as above but for time

    public int maxPlayers = 4;

    // How many games each team has won
    public int leftTeamGames = 0;
    public int rightTeamGames = 0;
    public bool VersusModeFinish {
        get {
            return (leftTeamGames >= 2 || rightTeamGames >= 2);
        }
    }

    public GameSettings gameSettings;

    public bool isPaused;

    public bool demoMode;

    public List<string> prevPuzzles = new List<string>(); // A list of the puzzles already played on in a puzzle challenge

    public Player playerInput;

    public SuperTextMesh debugText;

    string _levelDoc; // document containing data for the first(or only) level in a story stage
    public string LevelDoc {
        get { return _levelDoc; }
        set { _levelDoc = value; }
    }

    public PlayerManager playerManager;

    public UnityEvent gameOverEvent;

    bool _alive;

    void Awake() {
        SingletonCheck();

        if (_alive) {
            playerManager = GetComponent<PlayerManager>();
            gameSettings = GetComponent<GameSettings>();

            LoadGameData();

            //QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 200;
        }
    }

    void LoadGameData() {
        if (ES3.Load("Initialize", 0) == 0) {
            ES3.Save<int>("Initialize", 1);

            // Player stuff

            // Options
            ES3.Save<float>("MasterVolume", 100f);
            ES3.Save<int>("AimAssist", 1);
        }

        AudioListener.volume = (ES3.Load("MasterVolume", 100f) / 100);
        gameSettings.aimAssistSetting = (AIMASSIST)ES3.Load("AimAssist", 1);
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(transform.gameObject);

        //SetScreenResolution();

        ResetValues();

        SceneManager.sceneLoaded += SceneLoad;

        Random.InitState(System.Environment.TickCount);

        PhotonNetwork.automaticallySyncScene = true;

        playerInput = ReInput.players.GetPlayer(0);

        selectedBoard = BOARDS.NUM_STAGES;
        prevBoard = "";

        SetDemoMode(demoMode);

        isPaused = false;
    }

    void SingletonCheck() {
        GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
        if(obj != null && obj != this.gameObject) {
            _alive = false;
            DestroyImmediate(this.gameObject);
        } else {
            _alive = true;
        }
    }

    void SetScreenResolution() {
        Resolution[] resolutions = Screen.resolutions;

        if (resolutions.Length > 0) {
            int width, height;
            height = resolutions[resolutions.Length - 1].height;
            width = resolutions[resolutions.Length - 1].width;
            //width = (int)(height * 0.625f); // 10:16 ratio

            Screen.SetResolution(width, height, true);
        }
    }

    void ResetValues() {
        gameSettings.DefaultSettings();

    }

    void SceneLoad(Scene scene, LoadSceneMode mode) {
        gameIsOver = false;
        Unpause();
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.U)) {
            Debug.Break();
        }
        
        if(Input.GetKeyDown(KeyCode.I)) {
            Application.targetFrameRate = 5;
        } else if(Input.GetKeyUp(KeyCode.I)) {
            Application.targetFrameRate = 60;
        }
        
        // Failsafe for if anything goes really wrong, reloads to character select
        if(Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.P)) {
            CharacterSelectButton();
        }
        if(Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.O)) {
            VillageButton();
        }

        // Debugging
        //debugText.text = nextLevel;
    }

    public void FullPause() {
        // Pause the game via timeScale
        Time.timeScale = 0;

        isPaused = true;
    }

    public void Unpause() {
        // We do a coroutine here to stop input from overflowing when unpausing from menus
        StartCoroutine("WaitToUnpause");
    }

    IEnumerator WaitToUnpause() {
        yield return null;

        // Unpause the game
        Time.timeScale = 1;

        isPaused = false;
    }

    public void EndGame(int winningTeam, int winScore) {
        if(gameIsOver) {
            return;
        }

        gameOverEvent.Invoke();

        // Pause the game
        // FullPause();

        gameIsOver = true;

        // If we are playing a story level (and if it's a versus stage it's all done)
        if (IsStoryLevel()) {
            // Carry over the highscore
            scoreOverflow = winScore;

            // also the time
            LevelManager lM = FindObjectOfType<LevelManager>();
            timeOverflow += lM.LevelTimer;

            // The player's team won (and there are no further levels left)
            if (winningTeam == 0 && (nextLevel == "" || ((gameMode == GAME_MODE.MP_VERSUS && VersusModeFinish) ||
                                                          gameMode != GAME_MODE.MP_VERSUS))) {
                // Save the highscore
                if(isCoop) {
                    int[,] coopHighscores = ES3.Load<int[,]>("CoopHighScores");
                    SetHighscores(coopHighscores);
                    ES3.Save<int[,]>("CoopHighScores", coopHighscores);
                } else {
                    int[,] soloHighscores = ES3.Load<int[,]>("SoloHighScores");
                    SetHighscores(soloHighscores);
                    ES3.Save<int[,]>("SoloHighScores", soloHighscores);
                }

                // reset overflows
                scoreOverflow = 0;
                timeOverflow = 0;

                // Unlock the next level
                UnlockNextLevel();

                gameSettings.aimAssistSingleplayer = false;
            }

            if (winningTeam != 0 && gameSettings.aimAssistSetting == AIMASSIST.AFTERLOSS) {
                // If the player lost, turn on aimAssist
                gameSettings.aimAssistSingleplayer = true;
            }
        }
    }

    void SetHighscores(int[,] highscoresArray) {
        // Highscore depends on the game mode
        if (gameMode == GAME_MODE.MP_VERSUS) {
            // Higher score is better
            if (scoreOverflow > highscoresArray[stage[0] - 1, stage[1] - 1]) {
                highscoresArray[stage[0] - 1, stage[1] - 1] = scoreOverflow;
            }
        } else if (gameMode == GAME_MODE.SP_CLEAR) {
            // Lower time is better
            if ((int)timeOverflow < highscoresArray[stage[0] - 1, stage[1] - 1] || highscoresArray[stage[0] - 1, stage[1] - 1] == 0) {
                highscoresArray[stage[0] - 1, stage[1] - 1] = (int)timeOverflow;
            }
        } else if (gameMode == GAME_MODE.SP_POINTS) {
            // Fewer throws is better
            if (PlayerController.totalThrowCount < highscoresArray[stage[0] - 1, stage[1] - 1] || highscoresArray[stage[0] - 1, stage[1] - 1] == 0) {
                highscoresArray[stage[0] - 1, stage[1] - 1] = PlayerController.totalThrowCount;
            }
        }
    }

    void UnlockNextLevel() {
        int worldInt = stage[0];
        int levelInt = stage[1];
        int[] newStoryPos = new int[2];
        if (levelInt == 10) {
            newStoryPos[0] += worldInt + 1;
            newStoryPos[1] = 1;
        } else {
            newStoryPos[0] = worldInt;
            newStoryPos[1] = levelInt + 1;
        }

        // Load the furthest the player has gotten
        int[] storyProgress = ES3.Load<int[]>("StoryProgress");
        int furthestWorld = storyProgress[0];
        int furthestLevel = storyProgress[1];
        int newWorldInt = newStoryPos[0];
        int newLevelInt = newStoryPos[1];

        // If the new position is further than the player has gotten so far
        if ((newLevelInt >= furthestLevel && newWorldInt >= furthestWorld) || (newWorldInt > furthestWorld)) {
            // Update the story progress
            ES3.Save<string>("StoryProgress", newStoryPos);
        }
    }

    public void SetGameMode(GAME_MODE mode) {
        gameMode = mode;
        if(gameMode == GAME_MODE.SP_CLEAR || gameMode == GAME_MODE.SP_POINTS || gameMode == GAME_MODE.SURVIVAL) {
            isSinglePlayer = true;
        } else {
            isSinglePlayer = false;
        }
    }
    
    // The below functions won't actually affect the Game Manager object's data!!!
    // Since they are generally called by button events and those don't use the instantiated Game Manager
    public void PlayAgainButton() {
        Unpause();

        ResetGames();

        if (PhotonNetwork.connectedAndReady) {
            //PhotonNetwork.LoadLevel("NetworkedMapSelect");
            PhotonNetwork.LoadLevel("NetworkedMapSelectWheel");
        } else {
            // Reload current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void CharacterSelectButton() {
        CleanUp(true);

        if (PhotonNetwork.connectedAndReady) {
            isOnline = true;
            PhotonNetwork.LoadLevel("NetworkedCharacterSelect");
        } else {
            isOnline = false;
            SceneManager.LoadScene("PlayableCharacterSelect");
        }
    }
    public void StoryButton() {
        CleanUp(true);
        SceneManager.LoadScene("StorySelect");
    }
    public void MapSelectButton() {
        SceneManager.LoadScene("MapSelectWheel");
    }
    public void LocalPlayButton() {
        SceneManager.LoadScene("LocalPlay");
    }
    public void VillageButton() {
        CleanUp(true);

        prevBoard = "";

        if(isOnline) {
            PhotonNetwork.LeaveRoom();
        }

        //SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene("VillageScene");
    }

    public void BoardEditorButton() {
        CleanUp(true);

        SceneManager.LoadScene("BoardEditor");
    }

    public void ContinueLevel() {
        Unpause();

        if (PhotonNetwork.connectedAndReady) {
            //PhotonNetwork.LoadLevel("NetworkedMapSelect");
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        } else {
            // Reload current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // A "full" clean up is used when a set of games is over
    public void CleanUp(bool full) {
        Unpause();

        gameIsOver = false;

        BubbleManager.ClearAllData();

        if(full) {
            if (playerManager == null) {
                // Must fully find game object for the script because the button stuff is dumb.
                playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
            }
            playerManager.ClearAllPlayers();

            gameSettings.aimAssistSingleplayer = false;

            _levelDoc = "";
            ResetGames();
            prevPuzzles.Clear();
            scoreOverflow = 0;
        }
    }

    // Used specifically when retrying a story stage
    public void RetryCleanUp() {
        Unpause();
        gameIsOver = false;
        BubbleManager.ClearAllData();
        ResetGames();
        scoreOverflow = 0;
    }

    private void ResetGames() {
        GameManager gM = FindObjectOfType<GameManager>();
        gM.leftTeamGames = 0;
        gM.rightTeamGames = 0;
    }

    public bool IsStoryLevel() {
        if(stage == null || stage.Length < 2 || stage[0] == 0) {
            return false;
        }

        return true;
    }

    public void SetDemoMode(bool on) {
        if(on) {
            GetComponentInChildren<DemoManager>().enabled = true;
            demoMode = true;
        } else {
            GetComponentInChildren<DemoManager>().enabled = false;
            demoMode = false;
        }
    }
}
