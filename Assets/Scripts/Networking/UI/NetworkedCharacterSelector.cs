using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    NewCharacterSelect _characterSelect;

    CharacterSelector _selector;
    InputState _serializedInput;

    string _nickname;

    private void Awake() {
        _selector = GetComponent<CharacterSelector>();
        _serializedInput = new InputState();
    }
    
    // Use this for initialization
    void Start () {
        _characterSelect = FindObjectOfType<NewCharacterSelect>();

        _nickname = PhotonNetwork.playerName;

        Debug.Log(_selector.PhotonView.owner.ToString());
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Character
            //int characterName = (int)_character.CharacterName;
            //stream.Serialize(ref characterName);

            // Input
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);
            stream.Serialize(ref _serializedInput.up.isDown);
            stream.Serialize(ref _serializedInput.up.isJustPressed);
            stream.Serialize(ref _serializedInput.down.isDown);
            stream.Serialize(ref _serializedInput.down.isJustPressed);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            stream.Serialize(ref _serializedInput.attack.isJustPressed);

            ResetInput();
        } else {
            // Character
            //int characterName = 0;
            //stream.Serialize(ref characterName);
            //_selector.SetCharacter((CHARACTERNAMES)characterName);

            // Input
            stream.Serialize(ref _serializedInput.left.isDown);
            stream.Serialize(ref _serializedInput.left.isJustPressed);
            stream.Serialize(ref _serializedInput.right.isDown);
            stream.Serialize(ref _serializedInput.right.isJustPressed);
            stream.Serialize(ref _serializedInput.up.isDown);
            stream.Serialize(ref _serializedInput.up.isJustPressed);
            stream.Serialize(ref _serializedInput.down.isDown);
            stream.Serialize(ref _serializedInput.down.isJustPressed);
            stream.Serialize(ref _serializedInput.swing.isJustPressed);
            stream.Serialize(ref _serializedInput.attack.isJustPressed);

            // Take all the input built up between updates
            _selector.TakeInput(_serializedInput);

            ResetInput();
        }
    }

    void ResetInput() {
        _serializedInput = new InputState();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer otherPlayer) {
        // If we are owned by the local player and active, tell the new player to activate us
        if (_selector.PhotonView.owner == PhotonNetwork.player && _selector.Active) {
            _selector.PhotonView.TransferOwnership(PhotonNetwork.player);
            _selector.PhotonView.RPC("ActivateSelector", PhotonTargets.Others, _selector.InputState.controllerNum + _selector.PhotonView.ownerId, _selector.PhotonView.ownerId);
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            _characterSelect.RemoveNetworkedCharacter(_selector.InputState.controllerNum, otherPlayer.ID);
            _selector.isLocal = true;
        }
    }

    [PunRPC]
    void ActivateSelector(int controllerNum, int ownerID) {
        if (!_selector.Active) {
            // Tell character select to activate a character in it's place
            _selector.isLocal = false;
            _characterSelect.AddNetworkedCharacter(controllerNum, ownerID);
            _nickname = _selector.PhotonView.owner.NickName;
        }
    }

    [PunRPC]
    void DeactivateCharacter(int controllerNum, int ownerID) {
        _characterSelect.RemoveNetworkedCharacter(controllerNum + 4 + ownerID, ownerID);
        _selector.isLocal = true;
    }
}
