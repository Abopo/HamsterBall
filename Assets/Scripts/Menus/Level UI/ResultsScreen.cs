﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public SuperTextMesh winningTeamText;
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
            winningTeamText.gameObject.SetActive(true);
            winningTeamSprite.enabled = false;
        }
    }

    public void SetSinglePlayerResultsText(int result) {
        if (winningTeamSprite != null) {
            winningTeamSprite.gameObject.SetActive(false);
        }
        if (winningTeamText != null) {
            winningTeamText.gameObject.SetActive(true);
            if (result == -1) {
                winningTeamText.text = "You did it!";
            } else {
                winningTeamText.text = "You failed...";
            }
        }
    }

    // result: -1 = left team wins, 0 = draw, 1 = right team wins
    public void Activate(int result) {
        gameObject.SetActive(true);

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
