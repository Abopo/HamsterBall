using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public Text winningTeamText;
    public Image winningTeamSprite;
    public MenuButton mainMenuButton;

    MenuOption[] _menuOptions;

    float winTime = 1f;
    float winTimer = 0.0f;
    bool _canInteract = false;

    GameManager _gameManager;
    LevelManager _levelManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();
    }

    // Use this for initialization
    void Start () {
        // Set the text of the previousMenuButton to a proper text
        //_menuOptions = transform.GetComponentsInChildren<MenuOption>();
    }

    // Update is called once per frame
    void Update() {
        winTimer += Time.unscaledDeltaTime;
        if(winTimer > winTime) {
            _canInteract = true;

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
            winningTeamSprite.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Demo-GUI-Assets2")[5];
        } else if(winTeam == 1) {
            winningTeamText.text = "Right Team Wins";
            winningTeamSprite.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Demo-GUI-Assets2")[9];
        } else {
            winningTeamText.text = "Draw";
            winningTeamText.gameObject.SetActive(true);
            winningTeamSprite.enabled = false;
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

        winTimer = 0f;
    }

    // used for single player
    public void Activate(bool won) {
        gameObject.SetActive(true);
        winningTeamSprite.gameObject.SetActive(false);
        winningTeamText.gameObject.SetActive(true);
        if (won) {
            winningTeamText.text = "You did it!";
        } else {
            winningTeamText.text = "You failed...";
        }

        _menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in _menuOptions) {
            mo.isReady = false;
        }

        winTimer = 0f;
        _canInteract = false;
    }

    public void PlayAgain() {
        if(!_canInteract) {
            return;
        }

        _gameManager.PlayAgainButton();
    }

    public void ReturnToPreviousScene() {
        if(!_canInteract) {
            return;
        }

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

    public void ContinueToNextLevel() {
        if (!_canInteract) {
            return;
        }

        _gameManager.Unpause();
        if (_gameManager.nextLevel != "") {
            if (_gameManager.nextLevel == "Puzzle Challenge") {
                // Find a new puzzle and load it
                _gameManager.LoadPuzzleChallenge();
            } else {
                // Load the next level
                _levelManager.GetComponent<BoardLoader>().ReadBoardSetup(_gameManager.nextLevel);
            }
        } else if (_gameManager.nextCutscene != "") {
            // If we are in a verus stage
            if (_gameManager.gameMode == GAME_MODE.MP_VERSUS) {
                // Make sure the entire set is done before playing the cutscene
                if (_levelManager.setOver) {
                    // Load a cutscene
                    CutsceneManager.fileToLoad = _gameManager.nextCutscene;
                    SceneManager.LoadScene("Cutscene");
                    // Otherwise, play the next game in the set
                } else {
                    _levelManager.NextGame();
                }
            } else {
                // Load a cutscene
                CutsceneManager.fileToLoad = _gameManager.nextCutscene;
                SceneManager.LoadScene("Cutscene");
            }
        } else {
            // It's probably a versus match so
            // Replay the current level
            _levelManager.NextGame();
        }
    }
}
