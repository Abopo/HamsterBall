using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public Text winningTeamText;
    public Button previousMenuButton;
    public MenuButton mainMenuButton;

    MenuOption[] _menuOptions;

    float winTime = 1.0f;
    float winTimer = 0.0f;

    GameManager _gameManager;
    
    // Use this for initialization
    void Start () {
        // Set the text of the previousMenuButton to a proper text
        _gameManager = FindObjectOfType<GameManager>();
        switch (_gameManager.prevMenu) {
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
        // Game is paused here, so just use a fake delta time
        winTimer += 0.03f;
        if(winTimer > winTime) {
            foreach (MenuOption mo in _menuOptions) {
                if (mo != null) {
                    mo.isReady = true;
                }
            }
        }

        // Don't allow players to return to the main menu in demo mode
        if (mainMenuButton != null && _gameManager.demoMode) {
            mainMenuButton.isReady = false;
            mainMenuButton.gameObject.SetActive(false);
        }
    }

    public void SetWinningTeamText(int winTeam) {
        if(winTeam == 0) {
            winningTeamText.text = "Left Team Wins";
        } else if(winTeam == 1) {
            winningTeamText.text = "Right Team Wins";
        } else {
            winningTeamText.text = "Draw";
        }
    }

    // team = the winning team
    public void Activate(int team) {
        gameObject.SetActive(true);
        SetWinningTeamText(team);
        _menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in _menuOptions) {
            mo.isReady = false;
        }

        // If we are online and not the master client
        if(PhotonNetwork.connectedAndReady && !PhotonNetwork.isMasterClient) {
            // We shouldn't be able to use any of the buttons here
            foreach(MenuOption mo in _menuOptions) {
                mo.gameObject.SetActive(false);
            }
        }
    }

    // used for single player
    public void Activate(bool won) {
        gameObject.SetActive(true);
        if (won) {
            winningTeamText.text = "You did it!";
        } else {
            winningTeamText.text = "You failed...";
        }
        _menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in _menuOptions) {
            mo.isReady = false;
        }
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
