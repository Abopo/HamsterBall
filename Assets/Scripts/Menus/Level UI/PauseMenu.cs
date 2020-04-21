using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class PauseMenu : Menu {

    public GameObject optionsMenu;
    public GameObject pauseMenu;
    public SuperTextMesh aimAssist;

    bool _isActive;
    bool _justActivated;
    Player _player;

    MenuButton[] _buttons;

    protected override void Awake() {
        base.Awake();
        _buttons = GetComponentsInChildren<MenuButton>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    private void LateUpdate() {
        if (_isActive && (_player != null && _player.GetButtonDown("Pause")) && !_justActivated) {
            ResumeButton();
        }

        _justActivated = false;
    }

    public void ResumeButton() {
        Deactivate();
    }

    public override void Activate() {
        base.Activate();

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
        base.Activate();

        if(_isActive) {
            return;
        }

        _isActive = true;
        _justActivated = true;

        // Get the player that opened the pause menu
        _player = ReInput.players.GetPlayer(playerID);

        // Use that players inputs to control the menu options
        MenuOption[] options = GetComponentsInChildren<MenuOption>(true);
        foreach(MenuOption option in options) {
            option.SetPlayer(playerID);
        }

        SetAimAssistText();

        pauseMenu.SetActive(true);

        // Pause the game
        _gameManager.FullPause();
    }

    public override void Deactivate() {
        base.Deactivate();

        _gameManager.Unpause();
        _isActive = false;
        pauseMenu.SetActive(false);
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

    public void AimAssistButton() {
        _gameManager.gameSettings.aimAssistMultiplayer = !_gameManager.gameSettings.aimAssistMultiplayer;
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach(PlayerController pC in players) {
            pC.aimAssist = _gameManager.gameSettings.aimAssistMultiplayer;
        }
        SetAimAssistText();
    }

    void SetAimAssistText() {
        if (aimAssist != null) {
            aimAssist.text = "Aim Assist: " + (_gameManager.gameSettings.aimAssistMultiplayer ? "ON" : "OFF");
        }
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
        _gameManager.VillageButton();
    }
}
