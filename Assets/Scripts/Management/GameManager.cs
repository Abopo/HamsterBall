using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

public enum GAME_MODE { SP_POINTS = 0, SP_MATCH, SP_CLEAR, MP_VERSUS, NUM_MODES }
public enum MENU { STORY = 0, VERSUS, EDITOR };

public class GameManager : MonoBehaviour {
    public bool testMode;
    public bool isOnline = false;
    public bool isSinglePlayer = false;
    public bool gameIsOver = false;
    public string stage; // if level is "" it's a local multiplayer match, otherwise it's a story level
    public string nextLevel; // level to load next

    public MENU prevMenu; // Keeps track of the last menu we were in so we can return after a level is finished
    public string prevBoard; // Holds onto the previous board if there was one

    public GAME_MODE gameMode;
    public int goalCount; // the number of points or matches to win the level
    public int timeLimit; // the time limit to achieve the goal

    public int leftTeamHandicap;
    public int rightTeamHandicap;
    public bool aimAssist;

    public string nextCutscene;

    string _levelDoc; // document containing data for a single player level
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

    public string LevelDoc {
        get { return _levelDoc; }
        set { _levelDoc = value; }
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

        SceneManager.sceneLoaded += SceneLoad;

        _playerManager = GetComponent<PlayerManager>();

        Random.InitState(System.Environment.TickCount);

        PhotonNetwork.automaticallySyncScene = true;

        prevBoard = "";
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

        aimAssist = false;

        _hamsterSpawnMax = 10;
    }

    void SceneLoad(Scene scene, LoadSceneMode mode) {
        gameIsOver = false;
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

    public void FullPause() {
        // Pause the game via timeScale
        Time.timeScale = 0;
    }

    public void Unpause() {
        // Unpause the game
        Time.timeScale = 1;
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
            // and the player's team won
            if(winningTeam == 0) {
                if (gameMode == GAME_MODE.MP_VERSUS) {
                    string pref = stage.ToString() + "Highscore";

                    // If their new score is better than the old one
                    if (winScore > PlayerPrefs.GetInt(pref)) {
                        // Set their highscore
                        PlayerPrefs.SetInt(pref, winScore);
                    }

                    // Unlock the next level
                    UnlockNextLevel();
                }

                aimAssist = false;
            } else {
                // If the player lost, turn on aimAssist
                aimAssist = true;
            }
        }
    }

    void UnlockNextLevel() {
        int worldInt = int.Parse(stage[0].ToString());
        int levelInt = int.Parse(stage[2].ToString());
        string storyProgress = "";
        if (levelInt == 6) {
            storyProgress += (worldInt + 1).ToString();
            storyProgress += "-";
            storyProgress += "1";
        } else {
            storyProgress += worldInt.ToString();
            storyProgress += "-";
            storyProgress += (levelInt+1).ToString();
        }

        PlayerPrefs.SetString("StoryProgress", storyProgress);
        PlayerPrefs.SetString("StoryPos", storyProgress);
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
            isOnline = true;
            PhotonNetwork.LoadLevel("NetworkedCharacterSelect");
        } else {
            isOnline = false;
            SceneManager.LoadScene("CharacterSelect");
        }
    }

    public void StageSelectButton() {
        CleanUp();
        SceneManager.LoadScene("StorySelect");
    }

    public void MainMenuButton() {
        CleanUp();

        prevBoard = "";

        if(isOnline) {
            PhotonNetwork.LeaveRoom();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void BoardEditorButton() {
        CleanUp();

        SceneManager.LoadScene("BoardEditor");
    }

    public void CleanUp() {
        Unpause();

        gameIsOver = false;

        // Load character select screen.
        if (_playerManager == null) {
            // Must fully find game object for the script because the button stuff is dumb.
            _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        }
        _playerManager.ClearAllPlayers();

        BubbleManager.ClearStartingBubbles();
    }

    public bool IsStoryLevel() {
        if(stage == "") {
            return false;
        }

        return true;
    }
}
