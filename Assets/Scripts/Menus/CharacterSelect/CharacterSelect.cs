using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelect : MonoBehaviour {
    public Character character1;
    public Character character2;
    public Character character3;
    public Character character4;

    public Team teamLeft;
    public Team teamRight;

    public Text gameModeText;
    public GameObject PressStartText;

    public AISetupWindow aiSetupWindow;
    public GameSetupWindow gameSetupWindow;

    public int _assignedPlayers;
    public bool _assignedPlayer1;
    public bool _assignedPlayer2;
    public bool _assignedPlayer3;
    public bool _assignedPlayer4;
    PlayerManager _playerManager;
    GameManager _gameManager;

    bool _isActive;

    bool _assignedController1;
    bool _assignedController2;
    bool _assignedController3;
    bool _assignedController4;
    bool _assignedController5;
    bool _assignedController6;

    // Use this for initialization
    void Start() {
        _assignedPlayers = 0;
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.prevMenu = MENU.VERSUS;

        _isActive = true;

        _assignedPlayer1 = false;
        _assignedPlayer2 = false;
        _assignedPlayer3 = false;
        _assignedPlayer4 = false;
        _assignedController1 = false;
        _assignedController2 = false;
        _assignedController3 = false;
        _assignedController4 = false;
        _assignedController5 = false;
        _assignedController6 = false;

        SetupUI();

        gameSetupWindow.OptionsSetup();
    }

    void SetupUI() {
        PressStartText.SetActive(false);

        switch(_gameManager.gameMode) {
            case GAME_MODE.MP_VERSUS:
                gameModeText.text = "VERSUS";
                break;
            case GAME_MODE.MP_PARTY:
                gameModeText.text = "PARTY";
                break;
            case GAME_MODE.SURVIVAL:
                gameModeText.text = "SURVIVAL";
                break;
            case GAME_MODE.SP_CLEAR:
                gameModeText.text = "PUZZLE CHALLENGE";
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (_isActive) {
            ListenForInput();

            if (PressStartText.activeSelf == true) {
                // Look for input to start game
                if (Input.GetButtonDown("Start")) {
                    OpenSetupMenu();
                }
            }

            UpdateUI();
        }
    }

    void ListenForInput() {
        // Keyboard 1
        if (!_assignedController1 && InputState.GetAnyButtonDown(1)) {
            if (!_assignedController1) {
                AddCharacter(1);
            }
        } else if (_assignedController1) {
            if (Input.GetButtonDown("Bubble 1") && GetCharacterByController(1).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(1);
            } else if (Input.GetButtonDown("Attack 1")) {
                RemoveCharacter(1);
            }
        }
        // Keyboard 2
        if (!_assignedController2 && InputState.GetAnyButtonDown(2)) {
            if (!_assignedController2) {
                AddCharacter(2);
            }
        } else if (_assignedController2) {
            if (Input.GetButtonDown("Bubble 2") && GetCharacterByController(2).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(2);
            } else if (Input.GetButtonDown("Attack 2")) {
                RemoveCharacter(2);
            }
        }

        // Joystick 1
        if (!_assignedController3) {
            if (Input.GetButtonDown("Joystick Attack 1")) {
                BackToMainMenu();
            } else if (InputState.GetAnyButtonDown(3)) {
                AddCharacter(3);
            }
        } else if (_assignedController3) {
            if (Input.GetButtonDown("Joystick Bubble 1") && GetCharacterByController(3).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(3);
            } else if (Input.GetButtonDown("Joystick Attack 1")) {
                RemoveCharacter(3);
            }
        }

        // Joystick 2
        if (!_assignedController4) {
            if (Input.GetButtonDown("Joystick Attack 2")) {
                BackToMainMenu();
            } else if (InputState.GetAnyButtonDown(4)) {
                AddCharacter(4);
            }
        } else if (_assignedController4) {
            if (Input.GetButtonDown("Joystick Bubble 2") && GetCharacterByController(4).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(4);
            } else if (Input.GetButtonDown("Joystick Attack 2")) {
                RemoveCharacter(4);
            }
        }

        // Joystick 3
        if (!_assignedController5) {
            if (Input.GetButtonDown("Joystick Attack 3")) {
                BackToMainMenu();
            } else if (InputState.GetAnyButtonDown(5)) {
                AddCharacter(5);
            }
        } else if (_assignedController5) {
            if (Input.GetButtonDown("Joystick Bubble 3") && GetCharacterByController(5).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(5);
            } else if (Input.GetButtonDown("Joystick Attack 3")) {
                RemoveCharacter(5);
            }
        }

        // Joystick 4
        if (!_assignedController6) {
            if (Input.GetButtonDown("Joystick Attack 4")) {
                BackToMainMenu();
            } else if (InputState.GetAnyButtonDown(6)) {
                AddCharacter(6);
            }
        } else if (_assignedController6) {
            if (Input.GetButtonDown("Joystick Bubble 4") && GetCharacterByController(6).AllAIAssignedToTeam()) {
                // Add AI character
                AddAICharacter(6);
            } else if (Input.GetButtonDown("Joystick Attack 4")) {
                RemoveCharacter(6);
            }
        }
    }

    public void OpenSetupMenu() {
        // If there are AI players, open the AI Setup Window
        if (AnyAICharacters()) {
            aiSetupWindow.Initialize();
            _isActive = false;
            TurnOffCharacters();
        }
        // Otherwise, activate the Game Setup Windows
        else {
            gameSetupWindow.Initialize();
            _isActive = false;
            TurnOffCharacters();
        }
    }

    public void AddCharacter(int controllerNum) {
        _playerManager.AddPlayer(CheckNextAvailablePlayer(), controllerNum);
        _assignedPlayers++;
        ActivateCharacter();
        UpdateAssignedControllers(controllerNum, true);
    }

    public void AddNetworkedCharacter(int controllerNum, int ownerID) {
        _playerManager.AddPlayer(CheckNextAvailablePlayer(), controllerNum, ownerID);
        _assignedPlayers++;
        ActivateCharacter();
    }

    public void AddNetworkedCharacter(int playerNum, int controllerNum, int ownerID) {
        _playerManager.AddPlayer(playerNum, controllerNum, ownerID);
        _assignedPlayers++;
        ActivateCharacter(playerNum);
    }

    public void AddAICharacter(int controllerNum) {
        // TODO: actually make AI character ok online, for now just don't let players make ai characters
        if (_assignedPlayers < 4 && GetCharacterByController(controllerNum).Team != -1 && !PhotonNetwork.connectedAndReady) {
            _playerManager.AddPlayer(CheckNextAvailablePlayer(), -controllerNum);
            _assignedPlayers++;
            ActivateAICharacter(GetCharacterByController(controllerNum));
        }
    }

    void RemoveCharacter(int controllerNum) {
        int playerNum;
        if (GetCharacterByController(controllerNum).TopAIChild != null) {
            playerNum = _playerManager.RemovePlayerByNum(GetCharacterByController(controllerNum).TopAIChild.PlayerNum);
            //UpdateAssignedControllers(-controllerNum, false);
        } else {
            playerNum = _playerManager.RemovePlayerByController(controllerNum);
            UpdateAssignedControllers(controllerNum, false);
        }
        DeactivateCharacter(controllerNum);
        UpdateAssignedPlayers(playerNum);
        _assignedPlayers--;
    }

    public void RemoveNetworkedCharacter(int controllerNum, int ownerID) {
        int playerNum;
        playerNum = _playerManager.RemovePlayerByOwner(ownerID);
        DeactivateCharacter(controllerNum);
        UpdateAssignedPlayers(playerNum);
        _assignedPlayers--;
    }

    int CheckNextAvailablePlayer() {
        if(!_assignedPlayer1) {
            return 1;
        } else if (!_assignedPlayer2) {
            return 2;
        } else if (!_assignedPlayer3) {
            return 3;
        } else if (!_assignedPlayer4) {
            return 4;
        } else {
            return -1;
        }
    }

    void UpdateAssignedPlayers(int playerNum) {
        switch(playerNum) {
            case 1:
                _assignedPlayer1 = false;
                break;
            case 2:
                _assignedPlayer2 = false;
                break;
            case 3:
                _assignedPlayer3 = false;
                break;
            case 4:
                _assignedPlayer4 = false;
                break;
        }
    }

    void ActivateCharacter() {
        if (!character1.Active) {
            character1.Activate(1);
            _assignedPlayer1 = true;
        } else if (!character2.Active) {
            character2.Activate(2);
            _assignedPlayer2 = true;
        } else if (!character3.Active) {
            character3.Activate(3);
            _assignedPlayer3 = true;
        } else if (!character4.Active) {
            character4.Activate(4);
            _assignedPlayer4 = true;
        }
    }

    void ActivateCharacter(int playerNum) {
        switch(playerNum) {
            case 1:
                if (!character1.Active) {
                    character1.Activate(1);
                    _assignedPlayer1 = true;
                }
                break;
            case 2:
                if (!character2.Active) {
                    character2.Activate(2);
                    _assignedPlayer2 = true;
                }
                break;
            case 3:
                if (!character3.Active) {
                    character3.Activate(3);
                    _assignedPlayer3 = true;
                }
                break;
            case 4:
                if (!character4.Active) {
                    character4.Activate(4);
                    _assignedPlayer4 = true;
                }
                break;
        }
    }

    void ActivateAICharacter(Character parent) {
        if (!character1.Active) {
            character1.ActivateAI(1, parent);
            _assignedPlayer1 = true;
        } else if (!character2.Active) {
            character2.ActivateAI(2, parent);
            _assignedPlayer2 = true;
        } else if (!character3.Active) {
            character3.ActivateAI(3, parent);
            _assignedPlayer3 = true;
        } else if (!character4.Active) {
            character4.ActivateAI(4, parent);
            _assignedPlayer4 = true;
        }
    }

    void DeactivateCharacter(int joystickNum) {
        if (character1.JoystickNum == joystickNum) {
            character1.Deactivate();
        } else if (character2.JoystickNum == joystickNum) {
            character2.Deactivate();
        } else if (character3.JoystickNum == joystickNum) {
            character3.Deactivate();
        } else if (character4.JoystickNum == joystickNum) {
            character4.Deactivate();
        }
    }

    public Character GetCharacterByController(int controllerNum) {
        if (character1.ControllerNum == controllerNum) {
            return character1;
        } else if (character2.ControllerNum == controllerNum) {
            return character2;
        } else if (character3.ControllerNum == controllerNum) {
            return character3;
        } else if (character4.ControllerNum == controllerNum) {
            return character4;
        }

        return null;
    }

    void UpdateAssignedControllers(int controllerNum, bool isAssigned) {
        switch (controllerNum) {
            case 1:
                _assignedController1 = isAssigned;
                break;
            case 2:
                _assignedController2 = isAssigned;
                break;
            case 3:
                _assignedController3 = isAssigned;
                break;
            case 4:
                _assignedController4 = isAssigned;
                break;
            case 5:
                _assignedController5 = isAssigned;
                break;
            case 6:
                _assignedController6 = isAssigned;
                break;
        }
    }

    void UpdateUI() {
        // When enough players have been assigned to a team,
        // show the press start screen.
        if (_gameManager.gameMode == GAME_MODE.MP_VERSUS || _gameManager.gameMode == GAME_MODE.MP_PARTY) {
            if (_playerManager.NumPlayers != 0 &&
                teamLeft.numPlayers != 0 && teamRight.numPlayers != 0 &&
                teamLeft.numPlayers + teamRight.numPlayers == _playerManager.NumPlayers) {
                if (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient) {
                    PressStartText.SetActive(true);
                } else if (!PhotonNetwork.connectedAndReady) {
                    PressStartText.SetActive(true);
                }
            } else {
                PressStartText.SetActive(false);
            }
        } else if(_gameManager.gameMode == GAME_MODE.SURVIVAL || _gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            if(_playerManager.NumPlayers != 0 && 
                (teamLeft.numPlayers != 0 || teamRight.numPlayers != 0) &&
                teamLeft.numPlayers + teamRight.numPlayers == _playerManager.NumPlayers) {
                PressStartText.SetActive(true);
            } else {
                PressStartText.SetActive(false);
            }
        }
    }

    public bool AnyAICharacters() {
        if(character1.Active && character1.isAI) {
            return true;
        }
        if (character2.Active && character2.isAI) {
            return true;
        }
        if (character3.Active && character3.isAI) {
            return true;
        }
        if (character4.Active && character4.isAI) {
            return true;
        }

        return false;
    }

    void TurnOffCharacters() {
        character1.takeInput = false;
        character2.takeInput = false;
        character3.takeInput = false;
        character4.takeInput = false;
        
        // if we're online
        if (_gameManager.isOnline) {
            // Send a message saying the game is being setup
            character1.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, true);
            character2.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, true);
            character3.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, true);
            character4.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, true);
        }
    }

    public void Reactivate() {
        _isActive = true;
        character1.takeInput = true;
        character2.takeInput = true;
        character3.takeInput = true;
        character4.takeInput = true;

        // if we're online
        if (_gameManager.isOnline) {
            // Send a message saying the game isn't being setup anymore
            character1.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, false);
            character2.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, false);
            character3.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, false);
            character4.GetComponent<PhotonView>().RPC("GameSetup", PhotonTargets.Others, false);
        }
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
