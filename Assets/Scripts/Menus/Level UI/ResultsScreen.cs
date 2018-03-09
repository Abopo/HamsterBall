using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public Text winningTeamText;
    public Button previousMenuButton;

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
                mo.isReady = true;
            }
        }
    }

    public void SetWinningTeamText(int lostTeam) {
        if(lostTeam == 1) {
            winningTeamText.text = "Left Team Wins";
        } else if(lostTeam == 0) {
            winningTeamText.text = "Right Team Wins";
        } else {
            winningTeamText.text = "What happened? No team was given.";
        }
    }

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
    public void Activate() {
        gameObject.SetActive(true);
        winningTeamText.text = "You did it!";
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
