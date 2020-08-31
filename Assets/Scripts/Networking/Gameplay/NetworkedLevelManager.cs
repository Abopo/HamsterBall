using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Photon;

public class NetworkedLevelManager : Photon.MonoBehaviour {
    public GameObject disconnectMessage;

    int _masterClientID;
    int _playersReady;
    bool _gameStarted;

    bool _disconnected;
    float _disconnectTime = 3f;
    float _disconnectTimer = 0f;

    float _gameCountdownStartTime = 3f;
    float _gameCountdownStartTimer = 0f;

    PlayerManager _playerManager;
    GameCountdown _gameCountdown;
    LevelManager _levelManager;
    NetworkedPlayerSpawner _netPlayerSpawner;
    GameManager _gameManager;

    private void Awake() {
        _playerManager = FindObjectOfType<GameManager>().GetComponent<PlayerManager>();
        _gameCountdown = FindObjectOfType<GameCountdown>();
        _levelManager = GetComponent<LevelManager>();
        _netPlayerSpawner = GetComponent<NetworkedPlayerSpawner>();
        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    void Start() {
        FindMasterClient();

        PhotonNetwork.RPC(photonView, "PlayerReady", PhotonTargets.MasterClient, false);
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

        if(_disconnected && PhotonNetwork.isMasterClient) {
            _disconnectTimer += Time.unscaledDeltaTime;
            if(_disconnectTimer >= _disconnectTime) {
                // Exit out to the character select
                _gameManager.CharacterSelectButton();
            }
        }
    }

    // TODO: put this function in a place that makes more sense
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        Debug.Log("Player disconnected, ID: " + otherPlayer.ID);

        // If the player was not a spectator
        foreach(NetworkedPlayer nPlayer in FindObjectsOfType<NetworkedPlayer>()) {
            if(nPlayer.photonID == otherPlayer.ID) {
                HandlePlayerDisconnect(otherPlayer);
                break;
            }
        }
    }

    void HandlePlayerDisconnect(PhotonPlayer otherPlayer) {
        _disconnected = true;
        disconnectMessage.SetActive(true);

        // Pause the game
        _gameManager.FullPause();

        // If the disconnected player was the master client
        if (otherPlayer.ID == _masterClientID) {
            // Throw up a message saying the host disconnected
            disconnectMessage.GetComponentInChildren<SuperTextMesh>().text = "Host has disconnected, ending match...";
        } else {
            // Throw up a message saying the player disconnected
            disconnectMessage.GetComponentInChildren<SuperTextMesh>().text = otherPlayer.NickName + " has disconnected, ending match...";
        }
    }

    void InitializeGame() {
        _levelManager.LoadStagePrefab();
        _netPlayerSpawner.SpawnNetworkPlayers();
        photonView.RPC("StartGameCountdown", PhotonTargets.AllViaServer);

        photonView.RPC("SyncGameCounts", PhotonTargets.Others, _gameManager.leftTeamGames, _gameManager.rightTeamGames);
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

    [PunRPC]
    void SyncGameCounts(int leftGames, int rightGames) {
        _gameManager.leftTeamGames = leftGames;
        _gameManager.rightTeamGames = rightGames;

        _levelManager.LevelUI.SetupGameMarkers();
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
