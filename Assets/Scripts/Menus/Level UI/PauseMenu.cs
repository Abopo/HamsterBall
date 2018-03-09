using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public Button previousMenuButton;
    GameManager _gameManager;

    // Use this for initialization
    void Start() {
        // Set the text of the previousMenuButton to a proper text
        _gameManager = FindObjectOfType<GameManager>();
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
    }

    // Update is called once per frame
    void Update() {

    }

    public void ResumeButton() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);

        // Pause the game
        Time.timeScale = 0;
    }

    public void ReturnToPreviousScene() {
        // Return to the scene before this one
        switch (_gameManager.prevMenu) {
            case MENU.STORY:
                _gameManager.StageSelectButton();
                break;
            case MENU.VERSUS:
                _gameManager.CharacterSelectButton();
                break;
            case MENU.EDITOR:
                _gameManager.BoardEditorButton();
                break;
        }
    }
}
