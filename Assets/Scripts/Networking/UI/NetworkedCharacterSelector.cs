using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedCharacterSelector : Photon.MonoBehaviour {
    NetworkedCharacterSelect _characterSelect;

    CharacterSelector _selector;

    string _nickname;

    CharaInfo charaInfo = new CharaInfo();
    int characterName;
    int characterColor;
    bool islockedIn;
    bool isReady;

    GameManager _gameManager;

    private void Awake() {
        _selector = GetComponent<CharacterSelector>();

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
            _selector.SetIcon(charaInfo);

            stream.Serialize(ref islockedIn);
            if(islockedIn && !_selector.lockedIn) {
                _selector.LockIn();
            } else if(!islockedIn && _selector.lockedIn) {
                _selector.Unlock();
            }

            stream.Serialize(ref isReady);
            if(isReady && !_selector.isReady) {
                _selector.ShiftCSPlayer();
            } else if(!isReady && _selector.isReady) {
                _selector.Unready();
            }
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        // If this character was owned by the disconnected player
        // TODO: maybe make this based on ownerID instead of nickname (it's possible for two players to have the same name?)
        if (otherPlayer.NickName == _nickname) {
            _characterSelect.RemoveNetworkedCharacter(otherPlayer.ID);
        }

        _gameManager.numPlayers--;
    }
}
