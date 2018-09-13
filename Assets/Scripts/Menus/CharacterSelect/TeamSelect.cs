using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelect : MonoBehaviour {
    public Character[] characters;

    public Team teamLeft;
    public Team teamRight;

    public GameObject pressStartText;

    public AISetupWindow aiSetupWindow;
    public GameSetupWindow gameSetupWindow;

    PlayerManager _playerManager;
    GameManager _gameManager;

    bool _isActive;

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _playerManager = _gameManager.GetComponent<PlayerManager>();
        _gameManager.prevMenu = MENU.VERSUS;

        _isActive = true;

        pressStartText.SetActive(false);

        gameSetupWindow.OptionsSetup();

        SetupCharacters();
    }

    void SetupCharacters() {
        PlayerInfo tempInfo;
        for (int i = 0; i < _gameManager.numPlayers + _gameManager.numAI; ++i) {
            tempInfo = _playerManager.GetPlayerByIndex(i);
            tempInfo.team = i % 2; // Alternate teams
            characters[i].Initialize(tempInfo);
        }

        // Deactivate other characters
        for (int i = _gameManager.numPlayers + _gameManager.numAI; i < 4; ++i) {
            characters[i].Deactivate();
        }
    }

    // Update is called once per frame
    void Update () {
        if (_isActive) {
            if (pressStartText.activeSelf == true) {
                // Look for input to start game
                if (Input.GetButtonDown("Start")) {
                    OpenSetupMenu();
                }
            }

            // If all players are ready
            if (PlayersAreReady()) {
                // Show pressstarttext
                ShowPressStartText(true);
            } else {
                ShowPressStartText(false);
            }
        }
    }

    public void OpenSetupMenu() {
        // If there are AI players, open the AI Setup Window
        if (_gameManager.numAI > 0) {
            aiSetupWindow.Initialize();
            _isActive = false;
            TurnOffCharacters();
        }
        // Otherwise, activate the Game Setup Windows
        else {
            if (_gameManager.demoMode) {
                gameSetupWindow.DemoSetup();
            } else {
                gameSetupWindow.Initialize();
                _isActive = false;
                TurnOffCharacters();
            }
        }

        if (_gameManager.isOnline) {
            GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.AllBuffered, true);
        }
    }

    bool PlayersAreReady() {
        // If any players are not on a team
        foreach(Character chara in characters) {
            if(chara != null && chara.Team == -1) {
                return false;
            }
        }

        // Or any team doesn't have a player
        if(teamLeft.numPlayers == 0 || teamRight.numPlayers == 0) {
            return false;
        }

        return true;
    }

    void ShowPressStartText(bool show) {
        if(show) {
            if(!_gameManager.isOnline || (_gameManager.isOnline && PhotonNetwork.isMasterClient)) {
                pressStartText.SetActive(true);
            }
        } else {
            pressStartText.SetActive(false);
        }
    }

    public void TurnOnCharacters() {
        _isActive = true;

        foreach (Character chara in characters) {
            chara.takeInput = true;
        }

        if (_gameManager.isOnline) {
            GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.AllBuffered, false);
        }
    }
    public void TurnOffCharacters() {
        foreach(Character chara in characters) {
            chara.takeInput = false;
        }
    }
}
