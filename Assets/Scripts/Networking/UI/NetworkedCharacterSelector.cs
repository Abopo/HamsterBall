using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    CharacterSelector _selector;

    string _nickname;

    CharaInfo charaInfo = new CharaInfo();
    int characterName;
    int characterColor;
    bool islockedIn;
    bool isReady;

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

        PhotonNetwork.networkingPeer.DisconnectTimeout = 90000;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            if (_selector.curCharacterIcon != null) {
                characterName = (int)_selector.curCharacterIcon.charaName;
                characterColor = _selector.charaColor;
            } else {
                characterName = 0;
                characterColor = 0;
            }
            stream.Serialize(ref characterName);
            stream.Serialize(ref characterColor);


            islockedIn = _selector.lockedIn;
            stream.Serialize(ref islockedIn);
            isReady = _selector.isReady;
            stream.Serialize(ref isReady);
        } else {
            stream.Serialize(ref characterName);
            stream.Serialize(ref characterColor);

            charaInfo.name = (CHARACTERS)characterName;
            charaInfo.color = characterColor;
            _selector.SetColor(charaInfo.color);

            stream.Serialize(ref islockedIn);

            stream.Serialize(ref isReady);
        }
    }

    private void FixedUpdate() {
        if(PhotonNetwork.connectedAndReady && !photonView.isMine) {
            if (islockedIn && !_selector.lockedIn) {
                _selector.LockIn();
            } else if (!islockedIn && _selector.lockedIn) {
                _selector.Unlock();
            }

            if(!_selector.lockedIn) {
                if(_selector.curCharacterIcon.charaName != charaInfo.name) {
                    _selector.SetIcon(charaInfo);
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
