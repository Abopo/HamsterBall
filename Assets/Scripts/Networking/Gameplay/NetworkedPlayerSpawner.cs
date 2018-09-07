﻿using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Collections.Generic;

public class NetworkedPlayerSpawner : Photon.MonoBehaviour {
    public GameObject aiPlayerObj;
    public ShiftMeter[] shiftMeters = new ShiftMeter[4];
    int leftMeters = 0;
    int rightMeters = 0;
    Sprite[] bubSprites;

    List<PlayerController> _players = new List<PlayerController>();
    PlayerManager _playerManager;

    Transform[] spawns = new Transform[4]; // 0 - leftspawn1; 1 - leftspawn2; 2 - rightspawn1; 3 - rightspawn2
    int leftSpawned = 0;
    int rightSpawned = 0;

    // Use this for initialization
    void Start() {
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        bubSprites = Resources.LoadAll<Sprite>("Art/Hamsters_and_Bubbles/Bub_Sheet");
        GetSpawnLocations();

        // Only spawn characters on the master client
        if (PhotonNetwork.isMasterClient) {
            SpawnPlayers();
        }
    }

    void GetSpawnLocations() {
        SpawnPoint[] spawnPoints = GetComponentsInChildren<SpawnPoint>();
        spawns[0] = spawnPoints[0].transform;
        spawns[1] = spawnPoints[1].transform;
        spawns[2] = spawnPoints[2].transform;
        spawns[3] = spawnPoints[3].transform;
    }

    void SpawnPlayers() {
        PlayerInfo tempPlayerInfo;

        for (int i = 0; i < _playerManager.NumPlayers; ++i) {
            tempPlayerInfo = _playerManager.GetPlayerByIndex(i);

            PlayerController newPlayer;
            if (tempPlayerInfo.controllerNum < 0) {
                newPlayer = Instantiate(aiPlayerObj).GetComponent<PlayerController>();
                newPlayer.GetComponent<AIBrain>().Difficulty = tempPlayerInfo.difficulty;
            } else {
                Vector2 spawnPos = FindSpawnPosition(tempPlayerInfo.team);
                newPlayer = PhotonNetwork.Instantiate("Prefabs/Networking/Bub_PUN", spawnPos, Quaternion.identity, 0, 
                                                      new object[] { tempPlayerInfo.playerNum, tempPlayerInfo.team, tempPlayerInfo.controllerNum, tempPlayerInfo.characterName }).GetComponent<PlayerController>();
                // Transfer ownership to appropriate player
                newPlayer.GetComponent<PhotonView>().TransferOwnership(tempPlayerInfo.ownerID);
                Debug.Log("Spawned player " + tempPlayerInfo.playerNum + "on Team " + tempPlayerInfo.team);
            }
            newPlayer.playerNum = tempPlayerInfo.playerNum;
            newPlayer.team = tempPlayerInfo.team;
            //newPlayer.transform.position = FindSpawnPosition(newPlayer.team);
            newPlayer.GetComponentInChildren<Animator>().runtimeAnimatorController = FindAnimatorController(tempPlayerInfo.characterName);

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

    RuntimeAnimatorController FindAnimatorController(CHARACTERNAMES character) {
        RuntimeAnimatorController controller = null;
        switch (character) {
            /*
            case CHARACTERNAMES.BUB:
                controller = Resources.Load("Art/Animations/Player/Bub") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.NEGABUB:
                controller = Resources.Load("Art/Animations/Player/Bub2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOB:
                controller = Resources.Load("Art/Animations/Player/Bub3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.NEGABOB:
                controller = Resources.Load("Art/Animations/Player/Bub4") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.PEPSIMAN:
                controller = Resources.Load("Art/Animations/Player/PepsiMan/PepsiMan") as RuntimeAnimatorController;
                break;
            */
            case CHARACTERNAMES.BOY1:
                controller = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy1") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY2:
                controller = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy2") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY3:
                controller = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy3") as RuntimeAnimatorController;
                break;
            case CHARACTERNAMES.BOY4:
                controller = Resources.Load("Art/Animations/Player/Boy/Animation Objects/Boy4") as RuntimeAnimatorController;
                break;
        }

        return controller;
    }

    // TODO: this function is a little gross still, fix it up a bit
    public void SetupSwitchMeter(PlayerController player) {
        if(player.team == 0) {
            shiftMeters[leftMeters].GetMeterFront().enabled = true;
            //shiftMeters[leftMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[leftMeters]);
            shiftMeters[leftMeters].GetIcon().sprite = bubSprites[(player.playerNum - 1) * 12];
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if(player.team == 1) {
            shiftMeters[2+rightMeters].GetMeterFront().enabled = true;
            //shiftMeters[2+rightMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[2+rightMeters]);
            shiftMeters[2+rightMeters].GetIcon().sprite = bubSprites[(player.playerNum - 1) * 12];
            shiftMeters[2+rightMeters++].GetIcon().enabled = true;
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
