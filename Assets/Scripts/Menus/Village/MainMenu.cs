using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.IO;

public class MainMenu : MonoBehaviour {
    public bool resetPrefs;

    PauseMenu _pauseMenu;
    HowToPlayMenu _howToPlay;
    GameManager _gameManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _pauseMenu = GetComponent<PauseMenu>();
        _howToPlay = FindObjectOfType<HowToPlayMenu>();
    }
    // Use this for initialization
    void Start () {
        _gameManager.isOnline = false;
    }

    void InitDirectories() {
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Created Boards");
    }

    // Update is called once per frame
    void Update () {
        if (InputState.GetButtonOnAnyControllerPressed("Pause") && !_pauseMenu.IsActive) {
            // Open the menu
            _pauseMenu.Activate(0);
        }

    }

    public void LoadStoryMode() {
        _gameManager.StoryButton();
        //_gameManager.isOnline = false;
        //SceneManager.LoadScene("StorySelect");
    }

    public void LoadLocalMultiplayer() {
        _gameManager.LocalPlayButton();
        //SceneManager.LoadScene("LocalPlay");
    }

    public void LoadOnline() {
        _gameManager.isOnline = true;
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
        SceneManager.LoadScene("OnlineLobby");
    }

    public void LoadBoardEditor() {
        SceneManager.LoadScene("BoardEditor");
    }

    public void LoadOptions() {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void HowToPlay() {
        _howToPlay.Activate();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
