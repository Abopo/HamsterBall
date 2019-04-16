﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour {
    public GameObject pressStartText;
    public AISetupWindow aiSetupWindow;
    public GameSetupWindow gameSetupWindow;

    public TeamBox leftTeam;
    public TeamBox rightTeam;

    int _numPlayers = 0;
    int _numAI = 0;
    CharacterSelector[] _charaSelectors = new CharacterSelector[4];
    CSPlayerController[] _players;

    bool _isActive;

    PlayerManager _playerManager;
    GameManager _gameManager;

    public int NumPlayers {
        get { return _numPlayers; }
    }
    public int NumAI {
        get { return _numAI; }
    }

    // Use this for initialization
    void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.prevMenu = MENU.VERSUS;
        _numPlayers = _gameManager.numPlayers;
        _numAI = _gameManager.numAI;

        _playerManager = _gameManager.GetComponent<PlayerManager>();
        _playerManager.ClearAllPlayers();

        _players = FindObjectsOfType<CSPlayerController>();
        // Sort players by playerNum
        CSPlayerController tempPlayer;
        for(int i = 0; i < 3; ++i) {
            for(int j = 0; j < 3-i; ++j) {
                if(_players[j].playerNum > _players[j+1].playerNum) {
                    // Swap
                    tempPlayer = _players[j + 1];
                    _players[j + 1] = _players[j];
                    _players[j] = tempPlayer;
                }
            }
        }

        _isActive = true;
        pressStartText.SetActive(false);

        // If we are online, the selectors will be made elsewhere
        if (!_gameManager.isOnline) {
            SetupSelectors();
        }
	}

    // Assign controllers to every player
    void SetupSelectors() {
        _charaSelectors = FindObjectsOfType<CharacterSelector>();

        // Sort by player number
        CharacterSelector temp;
        for (int write = 0; write < 4; ++write) {
            for(int sort = 0; sort < 3; ++sort) {
                if(_charaSelectors[sort].playerNum > _charaSelectors[sort+1].playerNum) {
                    temp = _charaSelectors[sort + 1];
                    _charaSelectors[sort + 1] = _charaSelectors[sort];
                    _charaSelectors[sort] = temp;
                }
            }
        }

        // Setup and activate selectors that have controllers
        for (int i = 0; i < _numPlayers; ++i) {
            _charaSelectors[i].Activate(false);
        }

        // Do the same for AI
        for (int i = _numPlayers; i < _numPlayers+_numAI; ++i) {
            // Set ai to have same controller as Player 1
            _charaSelectors[i].Activate(true);
            // Add ai to player 1's list
            _charaSelectors[0].aiList.Add(_charaSelectors[i]);
        }

        // Deactivate the other selectors
        for (int i = _numPlayers+_numAI; i < 4; ++i) {
            _charaSelectors[i].Deactivate();
        }
    }

    // Update is called once per frame
    void Update () {
        if (_isActive) {
            if (pressStartText.activeSelf == true) {
                // Look for input to start game
                if (_gameManager.playerInput.GetButtonDown("Start")) {
                    //LoadNextScene();
                    OpenSetupMenu();
                }
            }

            UpdateUI();
        }
    }

    public void OpenSetupMenu() {
        // If there are AI players, open the AI Setup Window
        if (_numAI > 0) {
            Deactivate();
            aiSetupWindow.Initialize();
        }
        // Otherwise, activate the Game Setup Windows
        else {
            if (_gameManager.demoMode) {
                gameSetupWindow.DemoSetup();
            } else {
                Deactivate();
                gameSetupWindow.Initialize();
            }
        }

        if (_gameManager.isOnline) {
            GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.AllBuffered, true);
        }
    }

    void Reactivate() {
        _isActive = true;

        foreach (CharacterSelector charaSelector in _charaSelectors) {
            charaSelector.takeInput = true;
        }
        foreach(CSPlayerController player in _players) {
            player.underControl = true;
        }

        // Clear players from player manager
        _playerManager.ClearAllPlayers();
    }
    void Deactivate() {
         _isActive = false;

        foreach (CharacterSelector charaSelector in _charaSelectors) {
            charaSelector.takeInput = false;
        }
        foreach (CSPlayerController player in _players) {
            player.underControl = false;
        }

        // Load the chosen characters to the player manager
        foreach (CharacterSelector charaSelector in _charaSelectors) {
            if (charaSelector.gameObject.activeSelf) {
                charaSelector.LoadCharacter();
            }
        }
    }

    void UpdateUI() {
        // When all players have locked their characters in,
        // show the press start screen.
        if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY) {
            if (AllPlayersReady()) {
                if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                    pressStartText.SetActive(true);
                } else if (!PhotonNetwork.connectedAndReady) {
                    pressStartText.SetActive(true);
                }
            } else {
                pressStartText.SetActive(false);
            }
        } else /*if (_gameManager.gameMode == GAME_MODE.SURVIVAL || _gameManager.gameMode == GAME_MODE.SP_CLEAR)*/ {
            if (AllPlayersReady()) {
                pressStartText.SetActive(true);
            } else {
                pressStartText.SetActive(false);
            }
        }
    }

    bool AllPlayersReady() {
        // If any players are not on a team
        for(int i = 0; i < NumPlayers+NumAI; i++) {
            if(_players[i].team < 0) {
                return false;
            }
        }

        // Or any team doesn't have a player
        if (leftTeam.numPlayers == 0 || rightTeam.numPlayers == 0) {
            return false;
        }

        return true;
    }

    // Only used for networking
    public void AddSelector(CharacterSelector selector) {
        // Make sure we don't add a duplicate selector
        foreach (CharacterSelector charaSelector in _charaSelectors) {
            if(charaSelector == selector) {
                return;
            }
        }

        // If no duplicates were found

        // Add it to the array
        _charaSelectors[_numPlayers] = selector;

        _numPlayers++;

        Debug.Log("Added player" + " Num players: " + _numPlayers);
    }
    public void RemoveSelector(int ownerId) {
        // Find the selector with the same owner
        for (int i = 0; i < _numPlayers; ++i) {
            if (_charaSelectors[i].ownerId == ownerId) {
                // Deactivate that selector
                _charaSelectors[i].Deactivate();
                _numPlayers--;
                break;
            }
        }
    }
}