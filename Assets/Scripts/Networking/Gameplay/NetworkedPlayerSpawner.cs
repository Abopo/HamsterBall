using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Collections.Generic;

public class NetworkedPlayerSpawner : Photon.MonoBehaviour {
    public GameObject aiPlayerObj;
    public ShiftMeter[] shiftMeters = new ShiftMeter[4];
    int leftMeters = 0;
    int rightMeters = 0;
    Sprite[,] playerIcons = new Sprite[4, 4];

    List<PlayerController> _players = new List<PlayerController>();
    PlayerManager _playerManager;

    Transform[] spawns = new Transform[4]; // 0 - leftspawn1; 1 - leftspawn2; 2 - rightspawn1; 3 - rightspawn2
    int leftSpawned = 0;
    int rightSpawned = 0;

    // Use this for initialization
    void Start() {
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        // Get sprites

        // Boy
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        playerIcons[0, 0] = sprites[0];
        playerIcons[0, 1] = sprites[1];
        playerIcons[0, 2] = sprites[2];
        playerIcons[0, 3] = sprites[3];
        // Girl
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Girl-Icon");
        playerIcons[1, 0] = sprites[0];
        playerIcons[1, 1] = sprites[1];
        playerIcons[1, 2] = sprites[2];
        playerIcons[1, 3] = sprites[3];
        // Rooster
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Rooster-Icon");
        playerIcons[2, 0] = sprites[0];
        playerIcons[2, 1] = sprites[1];
        playerIcons[2, 2] = sprites[0];
        playerIcons[2, 3] = sprites[1];
        // Lackey
        sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Carl-Icons");
        playerIcons[3, 0] = sprites[0];
        playerIcons[3, 1] = sprites[1];
        playerIcons[3, 2] = sprites[0];
        playerIcons[3, 3] = sprites[1];

        GetSpawnLocations();

        // Only spawn characters on the master client
        if (PhotonNetwork.isMasterClient) {
            SpawnPlayers();
        }
    }

    void GetSpawnLocations() {
        SpawnPoint[] spawnPoints = GetComponentsInChildren<SpawnPoint>();
        for (int i = 0; i < spawnPoints.Length; ++i) {
            spawns[i] = spawnPoints[i].transform;
        }
    }

    void SpawnPlayers() {
        PlayerInfo tempPlayerInfo;

        for (int i = 0; i < _playerManager.NumPlayers; ++i) {
            tempPlayerInfo = _playerManager.GetPlayerByIndex(i);

            PlayerController newPlayer;
            if (tempPlayerInfo.isAI) {
                newPlayer = Instantiate(aiPlayerObj).GetComponent<PlayerController>();
                newPlayer.GetComponent<AIBrain>().Difficulty = tempPlayerInfo.difficulty;
            } else {
                Vector2 spawnPos = FindSpawnPosition(tempPlayerInfo.team);
                newPlayer = PhotonNetwork.Instantiate("Prefabs/Networking/Player_PUN2", spawnPos, Quaternion.identity, 0, 
                                                      new object[] { i, tempPlayerInfo.team, tempPlayerInfo.charaInfo.name, tempPlayerInfo.charaInfo.color }).GetComponent<PlayerController>();
                // Transfer ownership to appropriate player
                newPlayer.GetComponent<PhotonView>().TransferOwnership(tempPlayerInfo.ownerID);
                Debug.Log("Spawned player " + tempPlayerInfo.playerNum + "on Team " + tempPlayerInfo.team);
            }
            newPlayer.SetPlayerNum(tempPlayerInfo.playerNum);
            newPlayer.team = tempPlayerInfo.team;
            newPlayer.SetCharacterInfo(tempPlayerInfo.charaInfo);

            SetupSwitchMeter(newPlayer);

            _players.Add(newPlayer);
        }
    }

    Vector2 FindSpawnPosition(int team) {
        if (team == 0) {
            return spawns[leftSpawned++].position;
        } else if (team == 1) {
            return spawns[2 + rightSpawned++].position;
        } else {
            return Vector2.zero;
        }
    }

    // TODO: this function is a little gross still, fix it up a bit
    public void SetupSwitchMeter(PlayerController player) {
        if (player.team == 0) {
            shiftMeters[leftMeters].gameObject.SetActive(true);
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[leftMeters]);
            shiftMeters[leftMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name, player.CharaInfo.color - 1];
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if (player.team == 1) {
            shiftMeters[2 + rightMeters].gameObject.SetActive(true);
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[2 + rightMeters]);
            shiftMeters[2 + rightMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name, player.CharaInfo.color - 1];
            shiftMeters[2 + rightMeters++].GetIcon().enabled = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddPlayer(PlayerController player) {
        _players.Add(player);
    }

    public PlayerController GetPlayer(int playerNum) {
        PlayerController player = null;

        foreach (PlayerController p in _players) {
            if (p.playerNum == playerNum) {
                player = p;
            }
        }

        return player;
    }
}
