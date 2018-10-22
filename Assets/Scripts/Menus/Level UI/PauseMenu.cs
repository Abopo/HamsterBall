using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public Button previousMenuButton;
    public GameObject optionsMenu;

    MenuButton[] _buttons;
    GameManager _gameManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();

        _buttons = GetComponentsInChildren<MenuButton>();
    }

    // Use this for initialization
    void Start() {
        // Set the text of the previousMenuButton to a proper text
        /*
        switch(_gameManager.prevMenu) {
            case MENU.STORY:
                previousMenuButton.GetComponentInChildren<Text>().text = "Story Select";
                break;
            case MENU.VERSUS:
                previousMenuButton.GetComponentInChildren<Text>().text = "Character Select";
                break;
            case MENU.EDITOR:
                previousMenuButton.GetComponentInChildren<Text>().text = "Board Editor";
                break;
        }
        */
    }

    // Update is called once per frame
    void Update() {

    }

    public void ResumeButton() {
        _gameManager.Unpause();
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);

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
