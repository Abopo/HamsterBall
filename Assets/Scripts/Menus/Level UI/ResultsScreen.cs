using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public SuperTextMesh winningTeamText;
    public Image winningTeamSprite;
    public MenuButton mainMenuButton;
    public bool isContinue;

    MenuOption[] _menuOptions;

    float _winTime = 1f;
    float _winTimer = 0.0f;
    bool _canInteract = false;

    float _demoWaitTime = 5f;

    GameManager _gameManager;
    LevelManager _levelManager;
    DemoManager _demoManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();
        _demoManager = _gameManager.GetComponentInChildren<DemoManager>();
    }

    // Use this for initialization
    void Start () {
        // Set the text of the previousMenuButton to a proper text
        //_menuOptions = transform.GetComponentsInChildren<MenuOption>();
    }

    // Update is called once per frame
    void Update() {
        _winTimer += Time.unscaledDeltaTime;
        if(_winTimer > _winTime) {
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

        // If we are in demo mode and it's a com match
        if(_gameManager.demoMode && _demoManager.ComMatch && _winTimer >= _demoWaitTime) {
            // If the whole set is over
            if(_levelManager.setOver) {
                // Start a new random com match
                _demoManager.StartComMatch();
            } else {
                // Continue to next game of the match
                _levelManager.NextGame();
            }
        }
    }

    public void SetWinningTeamText(int winTeam) {
        if(winTeam == -1) {
            if (winningTeamText != null) {
                winningTeamText.text = "Left Team Wins";
            }
            winningTeamSprite.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Demo-GUI-Assets2")[5];
        } else if(winTeam == 1) {
            if (winningTeamText != null) {
                winningTeamText.text = "Right Team Wins";
            }
            winningTeamSprite.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Demo-GUI-Assets2")[9];
        } else {
            if (winningTeamText != null) {
                winningTeamText.text = "Draw";
            }
            //winningTeamText.gameObject.SetActive(true);
            winningTeamSprite.enabled = false;
        }
    }

    public void SetSinglePlayerResultsText(int result) {
        if (winningTeamSprite != null) {
            winningTeamSprite.gameObject.SetActive(false);
        }
        if (winningTeamText != null) {
            winningTeamText.gameObject.SetActive(true);
            if(isContinue) {
                winningTeamText.text = "Board Clear!";
                winningTeamText.textMaterial = Resources.Load<Material>("DefaultSTMMaterials/OutlineGreen");
            } else if (result == -1) {
                winningTeamText.text = "<w=simple>Stage Cleared";
                winningTeamText.textMaterial = Resources.Load<Material>("DefaultSTMMaterials/OutlineGreen");
            } else {
                winningTeamText.text = "Stage failed...";
                winningTeamText.textMaterial = Resources.Load<Material>("DefaultSTMMaterials/OutlineRed");
            }
        }
    }

    // result: -1 = left team wins, 0 = draw, 1 = right team wins
    public void Activate(int result) {
        gameObject.SetActive(true);
        if(_gameManager == null) {
            _gameManager = FindObjectOfType<GameManager>();
        }

        if (_gameManager.isSinglePlayer) {
            SetSinglePlayerResultsText(result);
        } else {
            SetWinningTeamText(result);
        }

        _menuOptions = transform.GetComponentsInChildren<MenuOption>();
        foreach (MenuOption mo in _menuOptions) {
            mo.isReady = false;
        }

        // If we are online and not the master client
        if (PhotonNetwork.connectedAndReady && !PhotonNetwork.isMasterClient) {
            // We shouldn't be able to use any of the buttons here
            foreach (MenuOption mo in _menuOptions) {
                mo.gameObject.SetActive(false);
            }
        }

        _winTimer = 0f;
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
            // Load the next level
            _levelManager.GetComponent<BoardLoader>().ReadBoardSetup(_gameManager.nextLevel);
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

    public void Retry() {
        if (_gameManager.LevelDoc != null) {
            _gameManager.CleanUp(false);

            BoardLoader boardLoader = FindObjectOfType<BoardLoader>();
            boardLoader.ReadBoardSetup(_gameManager.LevelDoc);
        } else {
            _gameManager.PlayAgainButton();
        }
    }
}
