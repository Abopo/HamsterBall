﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public enum GAME_MODE { SP_POINTS = 0, SP_MATCH, MP_VERSUS, NUM_MODES }

public class GameManager : MonoBehaviour {
    public bool testMode;
    public bool isOnline = false;
    public bool isSinglePlayer = false;
    public int level;

    public GAME_MODE gameMode;
    public int goalCount; // the number of points or matches to win the level

    public int leftTeamHandicap;
    public int rightTeamHandicap;

    public string nextCutscene;

    int _hamsterSpawnMax = 1;

    public int HamsterSpawnMax {
        get { return _hamsterSpawnMax; }

        set {
            _hamsterSpawnMax = value;
            if(_hamsterSpawnMax < 6) {
                _hamsterSpawnMax = 6;
            } else if(_hamsterSpawnMax > 14) {
                _hamsterSpawnMax = 14;
            }
        }
    }

    public PlayerManager _playerManager;

    public UnityEvent gameOverEvent;

    void Awake() {
        SingletonCheck();

        PlayerPrefSetup();

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 200;
    }

    void PlayerPrefSetup() {
        //PlayerPrefs.SetInt("Initialize", 0);

        if (PlayerPrefs.GetInt("Initialize") == 0) {
            PlayerPrefs.SetInt("Initialize", 1);
            // Player stuff

            // Options
            PlayerPrefs.SetFloat("MasterVolume", 100);
        }

        AudioListener.volume = (PlayerPrefs.GetFloat("MasterVolume") / 100);
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(transform.gameObject);

        ResetValues();

        _playerManager = GetComponent<PlayerManager>();

        Random.InitState(System.Environment.TickCount);

        PhotonNetwork.automaticallySyncScene = true;
    }

    void SingletonCheck() {
        GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
        if(obj != null && obj != this.gameObject) {
            DestroyImmediate(this.gameObject);
        }
    }

    void ResetValues() {
        leftTeamHandicap = 9;
        rightTeamHandicap = 9;

        _hamsterSpawnMax = 10;
    }

    // Update is called once per frame
    void Update () {
    }

    // 0 = left team; 1 = right team
    public void SetTeamHandicap(int team, int handicap) {
        if(team == 0) {
            leftTeamHandicap = handicap;
        } else if(team == 1) {
            rightTeamHandicap = handicap;
        }
    }

    public void Pause() {
        // Pause the game
        Time.timeScale = 0;
    }

    public void Unpause() {
        // Unpause the game
        Time.timeScale = 1;
    }

    public void EndGame(int wonTeam) {
        gameOverEvent.Invoke();
    }

    public void SetGameMode(GAME_MODE mode) {
        gameMode = mode;
        if(gameMode == GAME_MODE.MP_VERSUS) {
            isSinglePlayer = false;
        } else {
            isSinglePlayer = true;
        }
    }
    
    // The below functions don't actually affect the Game Manager object's data!!!
    public void PlayAgainButton() {
        Unpause();

        if (PhotonNetwork.connectedAndReady) {
            PhotonNetwork.LoadLevel("NetworkedMapSelect");
        } else {
            // Reload current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void CharacterSelectButton() {
        CleanUp();

        if (PhotonNetwork.connectedAndReady) {
            PhotonNetwork.LoadLevel("NetworkedCharacterSelect");
        } else {
            SceneManager.LoadScene("CharacterSelect");
        }
    }

    public void StageSelectButton() {
        CleanUp();
        SceneManager.LoadScene("StorySelect");
    }

    public void MainMenuButton() {
        CleanUp();
        SceneManager.LoadScene("MainMenu");
    }

    void CleanUp() {
        Unpause();
        // Load character select screen.
        if (_playerManager == null) {
            // Must fully find game object for the script because the button stuff is dumb.
            _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        }
        _playerManager.ClearAllPlayers();
    }
}
