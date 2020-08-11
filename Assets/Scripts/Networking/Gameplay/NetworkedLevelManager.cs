using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon;

public class NetworkedLevelManager : Photon.MonoBehaviour {
    int _masterClientID;
    int _playersReady;
    bool _gameStarted;

    PhotonView _photonView;

    PlayerManager _playerManager;
    GameCountdown _gameCountdown;
    LevelManager _levelManager;
    NetworkedPlayerSpawner _netPlayerSpawner;

    private void Awake() {
        _photonView = GetComponent<PhotonView>();
        _playerManager = FindObjectOfType<GameManager>().GetComponent<PlayerManager>();
        _gameCountdown = FindObjectOfType<GameCountdown>();
        _levelManager = GetComponent<LevelManager>();
        _netPlayerSpawner = GetComponent<NetworkedPlayerSpawner>();
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
                _gameStarted = true;
                InitializeGame();
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

    void InitializeGame() {
        _levelManager.LoadStagePrefab();
        _netPlayerSpawner.SpawnNetworkPlayers();
        PhotonNetwork.RPC(_photonView, "StartGameCountdown", PhotonTargets.AllViaServer, false);
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

    public void SendResultsCheck(int team, int result) {
        photonView.RPC("ResultsCheck", PhotonTargets.Others, team, result);
    }

    [PunRPC]
    void ResultsCheck(int team, int result) {
        // If we haven't shown our results yet
        if (!_levelManager.GameOver) {
            Debug.LogError("Results should be showing but aren't. Showing...");
            // Do it
            _levelManager.ActivateResultsScreen(team, result);
        }
    }
}
