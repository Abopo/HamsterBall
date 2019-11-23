using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

public class NetworkedLevelManager : Photon.MonoBehaviour {
    int _masterClientID;
    int _playersReady;
    bool _gameStarted;

    PhotonView _photonView;

    PlayerManager _playerManager;
    GameCountdown _gameCountdown;

    private void Awake() {
        _photonView = GetComponent<PhotonView>();
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        _gameCountdown = FindObjectOfType<GameCountdown>();
    }
    // Use this for initialization
    void Start() {
        FindMasterClient();

        PhotonNetwork.RPC(_photonView, "PlayerReady", PhotonTargets.MasterClient, false);
    }

    void FindMasterClient() {
        foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
            if (pp.IsMasterClient) {
                _masterClientID = pp.ID;
                Debug.Log("Master Client ID: " + _masterClientID);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    // Update is called once per frame
    void Update() {
        // Wait until all players have loaded into the scene
        if (!_gameStarted && PhotonNetwork.isMasterClient) {
            if(_playersReady >= _playerManager.NumPlayers) {
                PhotonNetwork.RPC(_photonView, "StartGameCountdown", PhotonTargets.All, false);
                _gameStarted = true;
            }
        }
    }

    // TODO: put this function in a place that makes more sense
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        Debug.Log("Player disconnected, ID: " + otherPlayer.ID);

        // If the disconnected player was the master client
        if (otherPlayer.ID == _masterClientID) {
            // TODO: throw up a message saying the host disconnected

            // Leave the room
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("OnlineLobby");
        }
    }

    // Should only be used by master client
    [PunRPC]
    void PlayerReady() {
        _playersReady++;
    }

    // Should only be used by master client
    [PunRPC]
    void StartGameCountdown() {
        _gameCountdown.StartCountdown();
    }

    // RPC to reload the current level
    [PunRPC]
    void ReloadCurrentLevel() {
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
    }
}
