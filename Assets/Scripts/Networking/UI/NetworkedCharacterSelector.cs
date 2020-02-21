using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    CharacterSelector _selector;

    string _nickname;

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

        _nickname = PhotonNetwork.playerName;

        // If we are owned by the local player
        if(GetComponent<PhotonView>().owner == PhotonNetwork.player) {
            // Don't worry about synching
            _synched = true;
        }

        PhotonNetwork.networkingPeer.DisconnectTimeout = 90000;
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
            _selector.SetColor(charaInfo.color);

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

            if((_selector.isActive && !_selector.lockedIn) || !_synched) {
                if(_selector.curCharacterIcon.charaName != charaInfo.name) {
                    _selector.SetIcon(charaInfo);
                    _synched = true;
                }
            }

            //if (isReady && !_selector.isReady) {
                //_selector.ShiftCSPlayer();
            //} else if (!isReady && _selector.isReady) {
                //_selector.Unready();
            //}
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            _netCharaSelect.RemoveNetworkedCharacter(otherPlayer.ID);
        }

        _charaSelect.numPlayers--;
    }
}
