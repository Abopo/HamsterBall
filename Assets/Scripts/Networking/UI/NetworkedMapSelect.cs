using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedMapSelect : MonoBehaviour {
    public bool allPlayersLoaded;

    int _mapSelectReady;

    PhotonView _photonView;
    PlayerManager _playerManager;

    private void Awake() {
        _photonView = GetComponent<PhotonView>();
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
    }
    // Start is called before the first frame update
    void Start() {
        allPlayersLoaded = false;


        PhotonNetwork.RPC(_photonView, "MapSelectReady", PhotonTargets.MasterClient, false);
    }

    // Networking
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    // Update is called once per frame
    void Update() {

    }

    [PunRPC]
    void MapSelectReady() {
        _mapSelectReady++;

        if (!allPlayersLoaded && PhotonNetwork.isMasterClient) {
            if (_mapSelectReady >= _playerManager.NumPlayers) {
                allPlayersLoaded = true;
            }
        }
    }
}
