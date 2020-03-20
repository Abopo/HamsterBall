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
    public string stage; // if level is "" it's a local multiplayer match, otherwise it's a story level
    public BOARDS selectedBoard;
    public string nextLevel; // level to load next
    public string nextCutscene; // cutscene to load next

    public MENU prevMenu; // Keeps track of the last menu we were in so we can return after a level is finished
    public string prevBoard; // Holds onto the previous board if there was one

    public GAME_MODE gameMode;
    public int goalCount; // the number of points or matches to win the level
    public int conditionLimit; // the condition limit to achieve the goal i.e. time limit, throw limit, etc.
    public int scoreOverflow = 0; // this variable holds onto the player's score between stages with multiple boards

    public int maxPlayers = 4;

    // How many games each team has won
    public int leftTeamGames = 0;
    public int rightTeamGames = 0;

    public int leftTeamHandicap = 9;
    public int rightTeamHandicap = 9;
    public bool aimAssist;
    public AIMASSIST aimAssistSetting = AIMASSIST.AFTERLOSS;
    public SPECIALSPAWNMETHOD specialSpawnMethod = SPECIALSPAWNMETHOD.BOTH;
    public bool SpecialBallsOn {
        get { return (specialSpawnMethod == SPECIALSPAWNMETHOD.BOTH || specialSpawnMethod == SPECIALSPAWNMETHOD.BALLS) && HamsterSpawner.AnySpecials; }
    }
    public bool SpecialPipeOn {
        get { return specialSpawnMethod == SPECIALSPAWNMETHOD.BOTH || specialSpawnMethod == SPECIALSPAWNMETHOD.PIPE; }
    }

    public bool isPaused;

    public bool demoMode;

    public List<string> prevPuzzles = new List<string>(); // A list of the puzzles already played on in a puzzle challenge

    public Player playerInput;

    public SuperTextMesh debugText;

    string _levelDoc; // document containing data for a single player level
    int _hamsterSpawnMax = 8;

    public int HamsterSpawnMax {
        get { return _hamsterSpawnMax; }

        set {
            _hamsterSpawnMax = value;
            if(_hamsterSpawnMax < 4) {
                _hamsterSpawnMax = 4;
            } else if(_hamsterSpawnMax > 12) {
                _hamsterSpawnMax = 12;
            }
        }
    }

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

            PlayerPrefSetup();

            //QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 200;
        }
    }

    void PlayerPrefSetup() {
        //PlayerPrefs.SetInt("Initialize", 0);

        if (PlayerPrefs.GetInt("Initialize") == 0) {
            PlayerPrefs.SetInt("Initialize", 1);
            // Player stuff

            // Options
            PlayerPrefs.SetFloat("MasterVolume", 100);
            PlayerPrefs.SetInt("AimAssist", 1);
        }

        AudioListener.volume = (PlayerPrefs.GetFloat("MasterVolume") / 100);
        aimAssistSetting = (AIMASSIST)PlayerPrefs.GetInt("AimAssist");
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
        leftTeamHandicap = 9;
        rightTeamHandicap = 9;

        aimAssist = false;

        _hamsterSpawnMax = 8;
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
            MainMenuButton();
        }

        // Debugging
        debugText.text = nextLevel;
    }

    // 0 = left team; 1 = right team
    public void SetTeamHandicap(int team, int handicap) {
        if(team == 0) {
            leftTeamHandicap = handicap;
        } else if(team == 1) {
            rightTeamHandicap = handicap;
        }
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

        // If we are playing a story level 
        if (IsStoryLevel()) {
            // If there's still levels to play
            if (nextLevel != "") {
                // Carry over the highscore
                scoreOverflow = winScore;

            // The player's team won
            } else if(winningTeam == 0) {
                // Save the highscore
                string pref;
                if(isCoop) {
                    pref = stage.ToString() + "HighscoreCoop";
                } else {
                    pref = stage.ToString() + "HighscoreSolo";
                }
                // If their new score is better than the old one
                if (winScore > PlayerPrefs.GetInt(pref)) {
                    // Set their highscore
                    PlayerPrefs.SetInt(pref, winScore);
                }

                // reset score overflow
                scoreOverflow = 0;

                // Unlock the next level
                UnlockNextLevel();

                aimAssist = false;
            } else if (aimAssistSetting == AIMASSIST.AFTERLOSS) {
                // If the player lost, turn on aimAssist
                aimAssist = true;
            }
        }
    }

    void UnlockNextLevel() {
        int worldInt = int.Parse(stage[0].ToString());
        int levelInt = stage.Length == 3 ? int.Parse(stage[2].ToString()) : int.Parse(stage[2].ToString() + stage[3].ToString());
        string newStoryPos = "";
        if (levelInt == 10) {
            newStoryPos += (worldInt + 1).ToString();
            newStoryPos += "-";
            newStoryPos += "1";
        } else {
            newStoryPos += worldInt.ToString();
            newStoryPos += "-";
            newStoryPos += (levelInt+1).ToString();
        }

        // Load the furthest the player has gotten
        string storyProgress = PlayerPrefs.GetString("StoryProgress");
        int furthestWorld = int.Parse(storyProgress[0].ToString());
        int furthestLevel = storyProgress.Length == 3 ? int.Parse(storyProgress[2].ToString()) : int.Parse(storyProgress[2].ToString() + storyProgress[3].ToString());
        int newWorldInt = int.Parse(newStoryPos[0].ToString());
        int newLevelInt = newStoryPos.Length == 3 ? int.Parse(newStoryPos[2].ToString()) : int.Parse(newStoryPos[2].ToString() + newStoryPos[3].ToString());

        // If the new position is further than the player has gotten so far
        if ((newLevelInt >= furthestLevel && newWorldInt >= furthestWorld) || (newWorldInt > furthestWorld)) {
            // Update the story progress
            PlayerPrefs.SetString("StoryProgress", newStoryPos);
        }

        PlayerPrefs.SetString("StoryPos", newStoryPos);
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
    public void MainMenuButton() {
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

            ResetGames();
            prevPuzzles.Clear();
            scoreOverflow = 0;
        }
    }

    private void ResetGames() {
        GameManager gM = FindObjectOfType<GameManager>();
        gM.leftTeamGames = 0;
        gM.rightTeamGames = 0;
    }

    public bool IsStoryLevel() {
        if(stage == "") {
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
