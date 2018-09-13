using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacter : Photon.MonoBehaviour {

    Character _character;
    InputState _serializedInput;
    NetworkedTeamSelect _teamSelect;

    string _nickname;

    private void Awake() {
        _character = GetComponent<Character>();
        _serializedInput = new InputState();
        _teamSelect = FindObjectOfType<NetworkedTeamSelect>();
    }

    public void Start() {
        _nickname = PhotonNetwork.playerName;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Team
            int team = _character.Team;
            stream.Serialize(ref team);

            // Character
            //int characterName = (int)_character.CharacterName;
            //stream.Serialize(ref characterName);

            // Input
            //stream.Serialize(ref _serializedInput.left.isDown);
            //stream.Serialize(ref _serializedInput.left.isJustPressed);
            //stream.Serialize(ref _serializedInput.right.isDown);
            //stream.Serialize(ref _serializedInput.right.isJustPressed);

            ResetInput();
        } else {
            // Team
            int team = -1;
            stream.Serialize(ref team);
            if(team != _character.Team) {
                SyncTeam(team);
            }

            // Character
            //int characterName = 0;
            //stream.Serialize(ref characterName);
            //_character.SetCharacter((CHARACTERNAMES)characterName);

            // Input
            //stream.Serialize(ref _serializedInput.left.isDown);
            //stream.Serialize(ref _serializedInput.left.isJustPressed);
            //stream.Serialize(ref _serializedInput.right.isDown);
            //stream.Serialize(ref _serializedInput.right.isJustPressed);

            // Take all the input built up between updates
            //_character.TakeInput(_serializedInput);

            //ResetInput();
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer otherPlayer) {
        // If we are owned by the local player and active, tell the new player to activate us
        //if(_character.PhotonView.owner == PhotonNetwork.player && _character.Active) {
        //    _character.PhotonView.TransferOwnership(PhotonNetwork.player);
        //    _character.PhotonView.RPC("ActivateCharacter", PhotonTargets.Others, _character.PlayerNum, _character.InputState.controllerNum + _character.PhotonView.ownerId, _character.PhotonView.ownerId);
        //}
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            _teamSelect.RemoveNetworkedCharacter(_character.InputState.controllerNum, otherPlayer.ID);
            _character.isLocal = true;
        }
    }

    public void Update() {
        GetOwnerInput();
    }

    void SyncTeam(int team) {
        if(team == -1) { // No team
            if(_character.Team == 0) {
                _character.MoveRight();
            } else if(_character.Team == 1) {
                _character.MoveLeft();
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
