using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkedHamsterMeter : Photon.MonoBehaviour {

    HamsterMeter _hamsterMeter;

    float _sendSyncTime = 5f;
    float _sendSyncTimer = 0f;

    int _syncedMeter;
    bool _needsToSync;

    float _trySyncTime = 0.5f;
    float _trySyncTimer = 0f;

    StockOrb[] _stockOrbs;

    private void Awake() {
        _hamsterMeter = GetComponent<HamsterMeter>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {

        } else {

        }
    }

    // Update is called once per frame
    void Update() {
        _stockOrbs = FindObjectsOfType<StockOrb>();

        if (PhotonNetwork.isMasterClient) {
            if (_stockOrbs.Length == 0) {
                _sendSyncTimer += Time.deltaTime;
                if (_sendSyncTimer >= _sendSyncTime) {
                    photonView.RPC("TrySyncMeter", PhotonTargets.Others, _hamsterMeter.CurStock);
                    _sendSyncTimer = 0f;
                    _needsToSync = false;
                }
            } else {
                _sendSyncTimer = 0f;
            }
        } else {
            if (_needsToSync) {
                _trySyncTimer += Time.deltaTime;
                if (_trySyncTimer >= _trySyncTime) {
                    _trySyncTimer = 0f;
                    // If there are no stock orbs out
                    if (_stockOrbs.Length == 0) {
                        SyncMeter();
                    }
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.H)) {
            _hamsterMeter.IncreaseStock(1);
            _hamsterMeter.IncreaseStock(1);
            _hamsterMeter.IncreaseStock(1);
        }
#endif
    }

    public void NeedSync() {
        _sendSyncTimer = 5f;
    }

    [PunRPC]
    void TrySyncMeter(int syncData) {
        _syncedMeter = syncData;
        _needsToSync = true;
    }

    void SyncMeter() {
        if(_hamsterMeter.CurStock != _syncedMeter) {
            Debug.LogError("Hamster meter out of sync, syncing...");

            if(_hamsterMeter.CurStock < _syncedMeter) {
                int dif = _syncedMeter - _hamsterMeter.CurStock;
                for(int i = 0; i < dif; ++i) {
                    _hamsterMeter.IncreaseStock(1);
                }
            } else if(_hamsterMeter.CurStock > _syncedMeter) {
                int dif = _hamsterMeter.CurStock - _syncedMeter;
                for (int i = 0; i < dif; ++i) {
                    _hamsterMeter.DecreaseStock(1);
                }
            }
        }

        _needsToSync = false;
    }
}
