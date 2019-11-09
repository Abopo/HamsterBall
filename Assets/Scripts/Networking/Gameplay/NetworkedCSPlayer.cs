using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedCSPlayer : MonoBehaviour {
    public PhotonView photonView;

    CSPlayerController _csPlayer;
    bool _underControl;
    bool _inPlayArea;

    // Give the client side some time to catch up
    float _bufferTime = 3f;
    float _bufferTimer;

    private void Awake() {
        _csPlayer = GetComponent<CSPlayerController>();
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.Serialize(ref _csPlayer.underControl);
            stream.Serialize(ref _csPlayer.inPlayArea);
        } else {
            // Receive latest state information
            stream.Serialize(ref _underControl);
            stream.Serialize(ref _inPlayArea);
        }
    }

    // Update is called once per frame
    void Update() {
        if (PhotonNetwork.connectedAndReady && !photonView.isMine) {
            if (_csPlayer.CurState != PLAYER_STATE.SHIFT) {
                if (_inPlayArea != _csPlayer.inPlayArea || _underControl != _csPlayer.underControl) {
                    _bufferTimer += Time.deltaTime;
                    if(_bufferTimer >= _bufferTime) {
                        Debug.Log("Syncing shift state");
                        // We need to sync up
                        if (_inPlayArea) {
                            // Go into the play area
                            _csPlayer.EnterPlayArea();
                        } else {
                            // Leave the play area
                            _csPlayer.EnterPullDownWindow();
                        }

                        _bufferTimer = 0f;
                    }
                } else {
                    _bufferTimer = 0f;
                }
            }
        }
    }

    [PunRPC]
    public void CSShift() {
        Debug.Log("RPC Shift");

        _csPlayer.ShiftIntoPlayArea();
    }

    [PunRPC]
    public void EnterPlayArea() {
        _csPlayer.PlayArea();
    }
    [PunRPC]
    public void EnterPullDownWindow() {
        _csPlayer.PullDownWindow();
    }
}
