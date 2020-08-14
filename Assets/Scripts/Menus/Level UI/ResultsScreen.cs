using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultsScreen : MonoBehaviour {
    public NumberTick currencyText;
    public SuperTextMesh winningTeamText;
    public Image winningTeamSprite;
    public MenuButton mainMenuButton;
    public Image[] flowers; // These are the "stars" players earn depending on how good they did
    public bool isContinue;

    MenuOption[] _menuOptions;

    float _winTime = 1f;
    float _winTimer = 0.8f;
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

            if (_menuOptions == null || _menuOptions.Length == 0) {
                _menuOptions = transform.GetComponentsInChildren<MenuOption>();
            } else {
                foreach (MenuOption mo in _menuOptions) {
                    if (mo != null) {
                        mo.isReady = true;
                    }
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
                winningTeamText.text = "Left Team Wins!";
            }
            winningTeamSprite.sprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Demo-GUI-Assets2")[5];
        } else if(winTeam == 1) {
            if (winningTeamText != null) {
                winningTeamText.text = "Right Team Wins!";
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
            if(isContinue) {
                winningTeamText.text = "Board Clear!";
                winningTeamText.textMaterial = Resources.Load<Material>("Materials/Text/OutlineGreen");
            } else if (result == -1) {
                winningTeamText.text = "<w=simple>Stage Cleared";
                winningTeamText.textMaterial = Resources.Load<Material>("Materials/Text/OutlineGreen");
            } else {
                winningTeamText.text = "Stage failed...";
                winningTeamText.textMaterial = Resources.Load<Material>("Materials/Text/OutlineRed");
            }
        }
    }

    // result: -1 = left team wins, 0 = draw, 1 = right team wins
    public void Activate(int result) {
        gameObject.SetActive(true);
        if(_gameManager == null) {
            _gameManager = FindObjectOfType<GameManager>();
        }

        if (_gameManager.IsStoryLevel()) {
            SetSinglePlayerResultsText(result);

            // If this is the last level and the player won
            if (_gameManager.IsLastLevel() && result == -1) {
                EarnFlowers();
            }
        } else {
            SetWinningTeamText(result);
        }

        if (currencyText != null) {
            SetCurrency();
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

        _winTimer = 0.8f;
        _canInteract = false;
    }

    // If the results of this seem off, make sure gameManager variables are being updated BEFORE this function
    void EarnFlowers() {
        if(flowers == null) {
            Debug.LogError("Null flowers");
            return;
        }
        int flowerCount = 1;

        // First flower is earned just by beating the stage
        flowers[0].gameObject.SetActive(true);

        // TODO: also adjust if playing solo or coop
        // Next two are earned depending on the game mode
        switch(_gameManager.gameMode) {
            case GAME_MODE.SP_CLEAR:
                // Compare time
                flowerCount += SetFlowers((int)_gameManager.timeOverflow);
                break;
            case GAME_MODE.SP_POINTS:
                // Throws used
                flowerCount += SetFlowers(PlayerController.totalThrowCount);
                break;
            case GAME_MODE.MP_VERSUS:
                // Points?
                flowerCount += SetFlowers(_gameManager.scoreOverflow);
                break;
        }

        // Save the flower count (if it's better!)
        if (_gameManager.isCoop) {
            int[,] coopFlowers = ES3.Load<int[,]>("CoopFlowers");
            if (flowerCount > coopFlowers[_gameManager.stage[0] - 1, _gameManager.stage[1] - 1]) {
                coopFlowers[_gameManager.stage[0] - 1, _gameManager.stage[1] - 1] = flowerCount;
                ES3.Save<int[,]>("CoopFlowers", coopFlowers);
            }
        } else {
            int[,] soloFlowers = ES3.Load<int[,]>("SoloFlowers");
            if (flowerCount > soloFlowers[_gameManager.stage[0] - 1, _gameManager.stage[1] - 1]) {
                soloFlowers[_gameManager.stage[0] - 1, _gameManager.stage[1] - 1] = flowerCount;
                ES3.Save<int[,]>("SoloFlowers", soloFlowers);
            }
        }

    }

    // Returns how many flowers were set
    int SetFlowers(int goal) {
        int flowerCount = 0;

        switch (_gameManager.gameMode) {
            case GAME_MODE.SP_CLEAR:
            case GAME_MODE.SP_POINTS:
                if (goal < _gameManager.flowerRequirement1) {
                    flowers[1].gameObject.SetActive(true);
                    flowerCount++;
                }
                if (goal < _gameManager.flowerRequirement2) {
                    flowers[2].gameObject.SetActive(true);
                    flowerCount++;
                }
                break;
            case GAME_MODE.MP_VERSUS:
                if (goal > _gameManager.flowerRequirement1) {
                    flowers[1].gameObject.SetActive(true);
                    flowerCount++;
                }
                if (goal > _gameManager.flowerRequirement2) {
                    flowers[2].gameObject.SetActive(true);
                    flowerCount++;
                }
                break;
        }

        return flowerCount;
    }

    void SetCurrency() {
        int combinedScore = 0, gainedCurrency = 0;

        // Find the score managers and add their scores together
        ScoreManager[] _scoreManagers = FindObjectsOfType<ScoreManager>();
        foreach(ScoreManager sm in _scoreManagers) {
            sm.CombineScore();
            combinedScore += sm.TotalScore;
        }

        // The currency gained is 10% of the combined scores
        gainedCurrency = combinedScore / 10;

        // TODO: The above DEFINITELY makes certain game modes more lucrative than others
        // Trying to find a way to earn around the same amount of currency for each mode
        // It may still vary wildly depending on what happens/what the stages are
        switch (_gameManager.gameMode) {
            case GAME_MODE.MP_VERSUS:
            case GAME_MODE.MP_PARTY:
            case GAME_MODE.SP_CLEAR:
                gainedCurrency /= 3;
                break;
            case GAME_MODE.SP_POINTS:
                gainedCurrency *= 2;
                break;
            case GAME_MODE.SURVIVAL:
                gainedCurrency /= 2;
                break;
            case GAME_MODE.TEAMSURVIVAL:
                gainedCurrency /= 4;
                break;
        }

        currencyText.StartTick(0, gainedCurrency);

        // Add and save currency based on score
        int totalCurrency = ES3.Load<int>("Currency", 0);
        totalCurrency += gainedCurrency;
        ES3.Save<int>("Currency", totalCurrency);
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
            // Hold onto the current level?
            _gameManager.prevLevel = _gameManager.stageName;

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
            _gameManager.RetryCleanUp();

            // Remove the ai players cuz they will be reloaded via BoardLoader
            _gameManager.playerManager.ClearAIPlayers();

            BoardLoader boardLoader = FindObjectOfType<BoardLoader>();
            boardLoader.ReadBoardSetup(_gameManager.LevelDoc);
        } else {
            _gameManager.PlayAgainButton();
        }
    }

    public void ReturnToVillage() {
        _gameManager.VillageButton();
    }
}
