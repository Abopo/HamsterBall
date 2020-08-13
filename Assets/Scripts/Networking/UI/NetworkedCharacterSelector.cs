using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    CharacterSelector _selector;

    string _nickname;
    int _ownerID = -1;

    CharaInfo charaInfo = new CharaInfo();
    int _characterName;
    int _characterColor;
    bool _islockedIn;
    bool _isReady;

    bool _synched = false;

    NetworkedCharacterSelect _netCharaSelect;
    CharacterSelect _charaSelect;

    //float _bufferTime = 3f;
    //float _bufferTimer = 0f;

    private void Awake() {
        _selector = GetComponent<CharacterSelector>();

        _selector.ownerId = GetComponent<PhotonView>().ownerId;
    }
    
    // Use this for initialization
    void Start () {
        _netCharaSelect = FindObjectOfType<NetworkedCharacterSelect>();
        _charaSelect = FindObjectOfType<CharacterSelect>();

        _ownerID = photonView.ownerId;

        // If we are owned by the local player
        if(GetComponent<PhotonView>().owner == PhotonNetwork.player) {
            // Don't worry about synching
            _synched = true;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            if (_selector.curCharacterIcon != null) {
                _characterName = (int)_selector.curCharacterIcon.charaName;
                _characterColor = _selector.charaColor;
            } else {
                _characterName = 0;
                _characterColor = 0;
            }
            stream.Serialize(ref _characterName);
            stream.Serialize(ref _characterColor);


            _islockedIn = _selector.lockedIn;
            stream.Serialize(ref _islockedIn);
            _isReady = _selector.isReady;
            stream.Serialize(ref _isReady);
        } else {
            stream.Serialize(ref _characterName);
            stream.Serialize(ref _characterColor);

            charaInfo.name = (CHARACTERS)_characterName;
            charaInfo.color = _characterColor;

            stream.Serialize(ref _islockedIn);

            stream.Serialize(ref _isReady);
        }
    }

    private void FixedUpdate() {
        if(PhotonNetwork.connectedAndReady && !photonView.isMine) {
            if (_islockedIn && !_selector.lockedIn) {
                _selector.LockIn();
            } else if (!_islockedIn && _selector.lockedIn) {
                _selector.Unlock();
            }

            if(_selector.isReady) {
                if(_selector.curCharacterIcon.charaName != charaInfo.name) {
                    _selector.SetIcon(charaInfo);
                    _synched = true;
                }
                if(_selector.charaColor != charaInfo.color) {
                    _selector.SetColor(charaInfo.color);
                }
            }

            //if (isReady && !_selector.isReady) {
                //_selector.ShiftCSPlayer();
            //} else if (!isReady && _selector.isReady) {
                //_selector.Unready();
            //}
        }
    }

    [PunRPC]
    void OwnerChanged(int playerID) {
        _ownerID = playerID;

        // Find our new owner
        for (int i = 0; i < PhotonNetwork.playerList.Length; ++i) {
            if (PhotonNetwork.playerList[i].ID == playerID) {
                // Get the nickname
                _nickname = PhotonNetwork.playerList[i].NickName;
            }
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this selector was owned by the disconnected player
        if (otherPlayer.ID == _ownerID) {
            _netCharaSelect.RemoveNetworkedCharacter(otherPlayer.ID);

            // Clear our data
            _ownerID = -1;
            _nickname = "";

            // Move our csplayer back to the window
            _selector.charaWindow.PlayerController.EnterPullDownWindow();

            // Close the window
            _selector.charaWindow.pullDownWindow.Hide();

            // Turn self off
            _selector.Deactivate();
        }
    }
}
