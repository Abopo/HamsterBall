using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class PauseMenu : MonoBehaviour {

    public GameObject optionsMenu;
    public GameObject pauseMenu;

    bool _isActive;
    bool _justActivated;
    Player _player;

    MenuButton[] _buttons;
    GameManager _gameManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();

        _buttons = GetComponentsInChildren<MenuButton>();
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    private void LateUpdate() {
        if (_isActive && _player.GetButtonDown("Start") && !_justActivated) {
            ResumeButton();
        }

        _justActivated = false;
    }

    public void ResumeButton() {
        _gameManager.Unpause();
        _isActive = false;
        pauseMenu.SetActive(false);
    }

    public void Activate() {
        if (_isActive) {
            return;
        }

        pauseMenu.SetActive(true);
        _isActive = true;
        _justActivated = true;

        // Pause the game
        _gameManager.FullPause();
    }

    public void Activate(int playerID) {
        if(_isActive) {
            return;
        }

        _isActive = true;
        _justActivated = true;

        // Get the player that opened the pause menu
        _player = ReInput.players.GetPlayer(playerID);

        // Use that players inputs to control the menu options
        MenuOption[] options = GetComponentsInChildren<MenuOption>();
        foreach(MenuOption option in options) {
            option.SetPlayer(playerID);
        }

        pauseMenu.SetActive(true);

        // Pause the game
        _gameManager.FullPause();
    }

    public void OpenOptionsMenu() {
        // Turn off buttons on pause menu
        foreach(MenuButton mB in _buttons) {
            mB.isReady = false;
        }

        // Enable options menu
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu() {
        // Turn on buttons on pause menu
        foreach (MenuButton mB in _buttons) {
            mB.isReady = true;
        }

        // Disable options menu
        optionsMenu.SetActive(false);
    }

    public void ReturnToPreviousScene() {
        // Return to the scene before this one
        switch (_gameManager.prevMenu) {
            case MENU.STORY:
                _gameManager.StoryButton();
                break;
            case MENU.VERSUS:
                _gameManager.CharacterSelectButton();
                break;
            case MENU.EDITOR:
                _gameManager.BoardEditorButton();
                break;
        }
    }

    public void ReturnToMainMenu() {
        _gameManager.MainMenuButton();
    }
}
