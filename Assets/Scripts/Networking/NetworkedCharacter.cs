using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacter : Photon.MonoBehaviour {
    public CharacterSelect characterSelect;
    public GameObject gameSetupText;

    Character _character;
    InputState _serializedInput;

    string _nickname;

    private void Awake() {
        _character = GetComponent<Character>();
        _serializedInput = new InputState();
    }

    public void Start() {
        _nickname = PhotonNetwork.playerName;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Team
            int team = _character.Team;
            stream.Serialize(ref team);

            // Input
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);

            ResetInput();
        } else {
            // Team
            int team = -1;
            stream.Serialize(ref team);
            if(team != _character.Team) {
                SyncTeam(team);
            }

            // Input
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);

            // Take all the input built up between updates
            _character.TakeInput(_serializedInput);

            ResetInput();

            Debug.Log(_serializedInput.left.isJustPressed.ToString());
        }
    }

    // Get event for when a character is activated
    [PunRPC]
    void ActivateCharacter(int controllerNum, int ownerID) {
        // Tell character select to activate a character in it's place
        _character.isLocal = false;
        characterSelect.AddNetworkedCharacter(controllerNum, ownerID);
        _nickname = _character.PhotonView.owner.NickName;
    }

    [PunRPC]
    void ActivateCharacter(int playerNum, int controllerNum, int ownerID) {
        if (!_character.Active) {
            // Tell character select to activate a character in it's place
            _character.isLocal = false;
            characterSelect.AddNetworkedCharacter(playerNum, controllerNum, ownerID);
            _nickname = _character.PhotonView.owner.NickName;
        }
    }

    [PunRPC]
    void DeactivateCharacter(int controllerNum, int ownerID) {
        characterSelect.RemoveNetworkedCharacter(controllerNum+4+ownerID, ownerID);
        _character.isLocal = true;
    }

    [PunRPC]
    void GameSetup(bool doingIt) {
        _character.takeInput = !doingIt;

        if(doingIt) {
            gameSetupText.SetActive(true);
        } else {
            gameSetupText.SetActive(false);
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer otherPlayer) {
        // If we are owned by the local player and active, tell the new player to activate us
        if(_character.PhotonView.owner == PhotonNetwork.player && _character.Active) {
            _character.PhotonView.TransferOwnership(PhotonNetwork.player);
            _character.PhotonView.RPC("ActivateCharacter", PhotonTargets.Others, _character.PlayerNum, _character.InputState.controllerNum + _character.PhotonView.ownerId, _character.PhotonView.ownerId);
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            characterSelect.RemoveNetworkedCharacter(_character.InputState.controllerNum, otherPlayer.ID);
            _character.isLocal = true;
        }
    }

    public void Update() {
        GetOwnerInput();
    }

    void SyncTeam(int team) {
        if(team == -1) { // No team
            if(_character.Team == 0) {
                _character.MoveLeft();
            } else if(_character.Team == 1) {
                _character.MoveRight();
            }
        } else if(team == 0) { // Left team
            if (_character.Team == -1) {
                _character.MoveLeft();
            } else if (_character.Team == 1) {
                _character.MoveLeft();
                _character.MoveLeft();
            }
        } else if (team == 1) { // Right team
            if (_character.Team == -1) {
                _character.MoveRight();
            } else if (_character.Team == 0) {
                _character.MoveRight();
                _character.MoveRight();
            }
        }
    }

    void GetOwnerInput() {
        if (_character.InputState.left.isDown) {
            _serializedInput.left.isDown = true;
        }
        if (_character.InputState.left.isJustPressed) {
            _serializedInput.left.isJustPressed = true;
        }
        if (_character.InputState.right.isDown) {
            _serializedInput.right.isDown = true;
        }
        if (_character.InputState.right.isJustPressed) {
            _serializedInput.right.isJustPressed = true;
        }
    }

    void ResetInput() {
        _serializedInput = new InputState();
    }
}
