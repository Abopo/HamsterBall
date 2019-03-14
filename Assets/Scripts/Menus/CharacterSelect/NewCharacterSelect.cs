using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewCharacterSelect : MonoBehaviour {

    public GameObject pressStartText;
    public AISetupWindow aiSetupWindow;
    public GameSetupWindow gameSetupWindow;

    int _numPlayers = 0;
    int _numAI = 0;
    CharacterSelector[] charaSelectors = new CharacterSelector[4];

    bool _isActive;

    PlayerManager _playerManager;
    GameManager _gameManager;

    public int NumPlayers {
        get { return _numPlayers; }
    }

    // Use this for initialization
    void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.prevMenu = MENU.VERSUS;
        //if(_gameManager.numPlayers == 0) {
        //    _gameManager.numPlayers = 1;
        //    _numPlayers = 1;
        //} else {
            _numPlayers = _gameManager.numPlayers;
            _numAI = _gameManager.numAI;
        //}

        _playerManager = _gameManager.GetComponent<PlayerManager>();
        _playerManager.ClearAllPlayers();

        _isActive = true;
        pressStartText.SetActive(false);

        // If we are online, the selectors will be made elsewhere
        if (!_gameManager.isOnline) {
            SetupSelectors();
        }
	}

    // Assign controllers to every player
    void SetupSelectors() {
        charaSelectors = FindObjectsOfType<CharacterSelector>();

        // Sort by player number
        CharacterSelector temp;
        for (int write = 0; write < 4; ++write) {
            for(int sort = 0; sort < 3; ++sort) {
                if(charaSelectors[sort].playerNum > charaSelectors[sort+1].playerNum) {
                    temp = charaSelectors[sort + 1];
                    charaSelectors[sort + 1] = charaSelectors[sort];
                    charaSelectors[sort] = temp;
                }
            }
        }

        // Setup and activate selectors that have controllers
        for (int i = 0; i < _numPlayers; ++i) {
            charaSelectors[i].Activate(false);
        }

        // Do the same for AI
        for (int i = _numPlayers; i < _numPlayers+_numAI; ++i) {
            // Set ai to have same controller as Player 1
            charaSelectors[i].Activate(true);
            // Add ai to player 1's list
            charaSelectors[0].aiList.Add(charaSelectors[i]);
        }

        // Deactivate the other selectors
        for (int i = _numPlayers+_numAI; i < 4; ++i) {
            charaSelectors[i].Deactivate();
        }
    }

    // Update is called once per frame
    void Update () {
        if (_isActive) {
            if (pressStartText.activeSelf == true) {
                // Look for input to start game
                if (_gameManager.playerInput.GetButtonDown("Start")) {
                    LoadNextScene();
                }
            }

            UpdateUI();
        }
    }

    public void OpenSetupMenu() {
        // If there are AI players, open the AI Setup Window
        if (_numAI > 0) {
            aiSetupWindow.Initialize();
            Deactivate();
        }
        // Otherwise, activate the Game Setup Windows
        else {
            if (_gameManager.demoMode) {
                gameSetupWindow.DemoSetup();
            } else {
                gameSetupWindow.Initialize();
                Deactivate();
            }
        }
    }

    void Reactivate() {
        _isActive = true;

        foreach (CharacterSelector charaSelector in charaSelectors) {
            charaSelector.takeInput = true;
        }
    }
    void Deactivate() {
         _isActive = false;

        foreach (CharacterSelector charaSelector in charaSelectors) {
            charaSelector.takeInput = false;
        }
    }

    void UpdateUI() {
        // When all players have locked their characters in,
        // show the press start screen.
        if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY) {
            if (AllPlayersLockedIn()) {
                if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                    pressStartText.SetActive(true);
                } else if (!PhotonNetwork.connectedAndReady) {
                    pressStartText.SetActive(true);
                }
            } else {
                pressStartText.SetActive(false);
            }
        } else /*if (_gameManager.gameMode == GAME_MODE.SURVIVAL || _gameManager.gameMode == GAME_MODE.SP_CLEAR)*/ {
            if (AllPlayersLockedIn()) {
                pressStartText.SetActive(true);
            } else {
                pressStartText.SetActive(false);
            }
        }
    }

    bool AllPlayersLockedIn() {
        for(int i = 0; i < _numPlayers+_numAI; ++i) {
            if(charaSelectors[i] != null && !charaSelectors[i].lockedIn) {
                return false;
            }
        }

        return true;
    }

    public void AddSelector(CharacterSelector selector) {
        // Make sure we don't add a duplicate selector
        foreach (CharacterSelector charaSelector in charaSelectors) {
            if(charaSelector == selector) {
                return;
            }
        }

        // If no duplicates were found

        // Add it to the array
        charaSelectors[_numPlayers] = selector;

        _numPlayers++;

        Debug.Log("Added player" + " Num players: " + _numPlayers);
    }
    // Only used for networking
    public void RemoveSelector(int ownerId) {
        // Find the selector with the same owner
        for (int i = 0; i < _numPlayers; ++i) {
            if (charaSelectors[i].ownerId == ownerId) {
                // Deactivate that selector
                charaSelectors[i].Deactivate();
                _numPlayers--;
                break;
            }
        }

    }

    public void LoadNextScene() {
        // Load the chosen characters to the player manager
        foreach(CharacterSelector charaSelector in charaSelectors) {
            if (charaSelector.gameObject.activeSelf) {
                charaSelector.LoadCharacter();
            }
        }

        if (_gameManager.gameMode == GAME_MODE.SP_CLEAR || _gameManager.gameMode == GAME_MODE.SURVIVAL) {
            SceneManager.LoadScene("MapSelectWheel");
        } else {
            if (!_gameManager.isOnline) {
                SceneManager.LoadScene("TeamSelect");
            } else {
                SceneManager.LoadScene("NetworkedTeamSelect");
            }
        }
    }
}
