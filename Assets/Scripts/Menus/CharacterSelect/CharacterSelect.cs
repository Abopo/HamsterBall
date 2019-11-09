﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class CharacterSelect : MonoBehaviour {
    public GameObject pressStartText;
    public AISetupWindow aiSetupWindow;
    public GameSetupWindow gameSetupWindow;
    public ExitMenu exitMenu;

    public TeamBox leftTeam;
    public TeamBox rightTeam;

    public int numPlayers = 0;
    public int numAI = 0;
    public int ActivePlayers {
        get { return numPlayers + numAI; }
    }

    CharacterSelector[] _charaSelectors = new CharacterSelector[4];
    CSPlayerController[] _players;

    Player _tempPlayer;
    List<Player> _assignedPlayers = new List<Player>();
    int _waitFrames;

    bool _isActive;

    GameManager _gameManager;

    private void Awake() {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.selectedBoard = BOARDS.NUM_STAGES;
    }
    // Use this for initialization
    void Start () {
        _gameManager.prevMenu = MENU.VERSUS;

        _gameManager.playerManager.ClearAllPlayers();

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

        SetupSelectors();
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
    }

    // Update is called once per frame
    void Update () {
        if (_isActive && _waitFrames > 5) {
            // If any player hits back while no characters are active
            if (ActivePlayers == 0 && !_gameManager.demoMode) {
                if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
                    // Show menu asking if player really wants to go back
                    exitMenu.Activate();
                    Deactivate();
                    return;
                }
            }
            // If there's still space for a player (and we're not online)
            if (IsStillSpace() && (!PhotonNetwork.connectedAndReady || Input.GetKey(KeyCode.O))) {
                // Look for player inputs
                _tempPlayer = InputState.AnyButtonOnAnyControllerPressed();
                if (_tempPlayer != null && !_assignedPlayers.Contains(_tempPlayer)) {
                    // Somehow make sure a player is only assigned once?
                    ActivateCharacter();
                }
            }
            if (pressStartText.activeSelf == true) {
                // Look for input to start game
                if (InputState.GetButtonOnAnyControllerPressed("Start")) {
                    //LoadNextScene();
                    OpenSetupMenu();
                }
            }

            UpdateUI();
        } else {
            _waitFrames++;
        }
    }

    public void OpenSetupMenu() {
        // If there are AI players, open the AI Setup Window
        if (numAI > 0) {
            Deactivate();
            aiSetupWindow.Initialize();
        }
        // Otherwise, activate the Game Setup Windows
        else {
            if (_gameManager.demoMode) {
                Deactivate();
                gameSetupWindow.DemoSetup();
            } else {
                Deactivate();
                gameSetupWindow.Initialize();
            }
        }

        if (_gameManager.isOnline) {
            GetComponent<PhotonView>().RPC("GameSetupStart", PhotonTargets.AllBuffered, true);
        }
    }

    public void ActivateCharacter() {
        // Find the next available character selector
        foreach(CharacterSelector cs in _charaSelectors) {
            if(!cs.isActive) {
                cs.Activate(_tempPlayer);
                break;
            }
        }

        // Add player to list of assigned
        _assignedPlayers.Add(_tempPlayer);

        numPlayers++;

        // Reset tempController to null
        _tempPlayer = null;
    }
    public void ActivateAI(CharacterSelector activatingPlayer) {
        // Activate the AI character selector
        _charaSelectors[ActivePlayers].ActivateAsAI(activatingPlayer);

        // Stop the activating player from receiving input
        activatingPlayer.takeInput = false;

        numAI++;
    }

    public void Reactivate() {
        _isActive = true;
        _waitFrames = 0;

        foreach (CharacterSelector charaSelector in _charaSelectors) {
            charaSelector.takeInput = true;
        }
        foreach(CSPlayerController player in _players) {
            if (player.inPlayArea) {
                player.underControl = true;
            }
        }

        // Clear players from player manager
        _gameManager.playerManager.ClearAllPlayers();
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
            if (charaSelector.isActive) {
                charaSelector.LoadCharacter();
            }
        }
    }

    void UpdateUI() {
        // When all players have locked their characters in,
        // show the press start screen.
        if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY || _gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            if (AllPlayersOnTeams()) {
                if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                    pressStartText.SetActive(true);
                } else if (!PhotonNetwork.connectedAndReady) {
                    pressStartText.SetActive(true);
                }
            } else {
                pressStartText.SetActive(false);
            }
        } else if (_gameManager.gameMode == GAME_MODE.SURVIVAL || _gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            if (AllPlayersSelected()) {
                pressStartText.SetActive(true);
            } else {
                pressStartText.SetActive(false);
            }
        }
    }

    bool AllPlayersOnTeams() {
        // If any players are not on a team
        for(int i = 0; i < ActivePlayers; i++) {
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

    bool AllPlayersSelected() {
        for (int i = 0; i < ActivePlayers; ++i) {
            // If there is a player that's not underControl yet
            if (!_players[i].underControl) {
                return false;
            }
        }

        return true;
    }

    public void RemovePlayer(Player player) {
        _assignedPlayers.Remove(player);
        _waitFrames = 0;
    }

    public bool IsStillSpace() {
        if(ActivePlayers < _gameManager.maxPlayers) {
            return true;
        }

        return false;
    }
}
