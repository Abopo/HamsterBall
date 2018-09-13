using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    NetworkedCharacterSelect _characterSelect;

    CharacterSelector _selector;
    InputState _serializedInput;

    string _nickname;

    int characterName;
    bool islockedIn;

    GameManager _gameManager;

    private void Awake() {
        _selector = GetComponent<CharacterSelector>();
        _serializedInput = new InputState();

        _selector.ownerId = GetComponent<PhotonView>().ownerId;
    }
    
    // Use this for initialization
    void Start () {
        _characterSelect = FindObjectOfType<NetworkedCharacterSelect>();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.numPlayers++;

        _nickname = PhotonNetwork.playerName;

        PhotonNetwork.networkingPeer.DisconnectTimeout = 90000;
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
            /*
            _selector.GetInput();

            stream.Serialize(ref _selector.InputState.left.isDown);
            stream.Serialize(ref _selector.InputState.left.isJustPressed);
            stream.Serialize(ref _selector.InputState.right.isDown);
            stream.Serialize(ref _selector.InputState.right.isJustPressed);
            stream.Serialize(ref _selector.InputState.up.isDown);
            stream.Serialize(ref _selector.InputState.up.isJustPressed);
            stream.Serialize(ref _selector.InputState.down.isDown);
            stream.Serialize(ref _selector.InputState.down.isJustPressed);
            stream.Serialize(ref _selector.InputState.swing.isJustPressed);
            stream.Serialize(ref _selector.InputState.attack.isJustPressed);


            ResetInput();
            */

            if (_selector.curCharacterIcon != null) {
                characterName = (int)_selector.curCharacterIcon.characterName;
            } else {
                characterName = 0;
            }
            stream.Serialize(ref characterName);

            islockedIn = _selector.lockedIn;
            stream.Serialize(ref islockedIn);
        } else {
            // Character
            //int characterName = 0;
            //stream.Serialize(ref characterName);
            //_selector.SetCharacter((CHARACTERNAMES)characterName);
            /*
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
            _selector.CheckInput();

            ResetInput();
            */

            stream.Serialize(ref characterName);
            _selector.SetIcon((CHARACTERNAMES)characterName);

            stream.Serialize(ref islockedIn);
            if(islockedIn && !_selector.lockedIn) {
                _selector.LockIn();
            } else if(!islockedIn && _selector.lockedIn) {
                _selector.Unlock();
            }
        }
    }

    void ResetInput() {
        _serializedInput = InputState.ResetInput(_serializedInput);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            _characterSelect.RemoveNetworkedCharacter(_selector.InputState.controllerNum, otherPlayer.ID);
        }

        _gameManager.numPlayers--;
    }
}
