﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerInfo {
    public int playerNum;
    public CHARACTERNAMES characterName;
    public int controllerNum;
    public int team; // -1 == none; 0 == left; 1 == right
    public bool aimAssist;
    public int difficulty; // Only used for AI, 0-3: easy-expert
    public string characterAI; // Only used for AI
    public int ownerID; // used for networking
}

public class PlayerManager : MonoBehaviour {
    public bool backup;
    List<PlayerInfo> _players = new List<PlayerInfo>();
    List<PlayerInfo> _backupPlayers = new List<PlayerInfo>(); // Mainly used for the test scene

    public int NumPlayers {
        get {
            //if (backup) {
            //    return _backupPlayers.Count;
            //} else {
                return _players.Count;
            //}
        }
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(transform.gameObject);
        if (backup) {
            BackupPlayers();
        }

        SceneManager.sceneLoaded += SceneChanged;
        //NumPlayers = 0;
    }

    void BackupPlayers() {
        PlayerInfo newPlayer = new PlayerInfo();
        newPlayer.playerNum = 1;
        newPlayer.controllerNum = 1;
        newPlayer.team = 0;
        _players.Add(newPlayer);

        PlayerInfo newPlayer2 = new PlayerInfo();
        newPlayer2.playerNum = 2;
        newPlayer2.controllerNum = -1;
        newPlayer2.team = 1;
        _players.Add(newPlayer2);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public PlayerInfo GetPlayerByIndex(int index) {
        //if (_players.Count > 0) {
            return _players[index];
        //} else {
        //    return _backupPlayers[index];
        //}
    }

    public PlayerInfo GetPlayerByNum(int playerNum) {
        PlayerInfo tempPlayer = new PlayerInfo();

        foreach (PlayerInfo p in _players) {
            if(p.playerNum == playerNum) {
                tempPlayer = p;
            }
        }

        return tempPlayer;
    }

    public int GetControllerNum(int playerNum) {
        foreach (PlayerInfo p in _players) {
            if (p.playerNum == playerNum) {
                // TODO: Might need to clean up memory here.
                return p.controllerNum;
            }
        }

        return -1;
    }

    public void SetTeam(int playerNum, int team) {
        foreach (PlayerInfo p in _players) {
            if (p.playerNum == playerNum) {
                // TODO: Might need to clean up memory here.
                int i = _players.IndexOf(p);
                _players[i].team = team;
                //Debug.Log("Player " + playerNum + " added to Team " + team);
            }
        }
    }

    public void AddPlayer() {
        PlayerInfo newPlayer = new PlayerInfo();
        newPlayer.playerNum = _players.Count+1;
        newPlayer.controllerNum = -1;
        newPlayer.characterName = GetNextAvailableCharacterUp(CHARACTERNAMES.BOY1);
        newPlayer.team = -1;
        newPlayer.difficulty = 0;
        _players.Add(newPlayer);
    }

    public void AddPlayer(int playerNum, int controllerNum) {
        if (playerNum != -1) {
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.playerNum = playerNum;
            newPlayer.controllerNum = controllerNum;
            newPlayer.characterName = GetNextAvailableCharacterUp(CHARACTERNAMES.BOY1);
            newPlayer.team = -1;
            newPlayer.difficulty = 0;
            _players.Add(newPlayer);
        }
    }

    public void AddPlayer(PlayerInfo player) {
        _players.Add(player);
    }

    // For networking
    public void AddPlayer(int playerNum, int controllerNum, int ownerID) {
        if (playerNum != -1) {
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.playerNum = playerNum;
            newPlayer.controllerNum = controllerNum;
            newPlayer.team = -1;
            newPlayer.difficulty = 0;
            newPlayer.ownerID = ownerID;
            _players.Add(newPlayer);
        }
    }

    public int RemovePlayerByNum(int playerNum) {
        foreach (PlayerInfo p in _players) {
            if (p.playerNum == playerNum) {
                // TODO: Might need to clean up memory here.
                _players.Remove(p);
                return playerNum;
            }
        }
        return -1;
    }

    // Returns the player number to keep track on which one has been removed.
    public int RemovePlayerByController(int controllerNum) {
        foreach(PlayerInfo p in _players) {
            if(p.controllerNum == controllerNum) {
                // TODO: Might need to clean up memory here.
                _players.Remove(p);
                return p.playerNum;
            }
        }
        return -1;
    }

    // Only used when networked
    public int RemovePlayerByOwner(int owner) {
        foreach (PlayerInfo p in _players) {
            if (p.ownerID == owner) {
                // TODO: Might need to clean up memory here.
                _players.Remove(p);
                return p.playerNum;
            }
        }
        return -1;
    }

    public CHARACTERNAMES GetNextAvailableCharacterUp(CHARACTERNAMES startChara) {
        CHARACTERNAMES character = startChara;

        int counter = 0;
        while (IsCharacterTaken(character)) {
            character = character + 1;
            if(character >= CHARACTERNAMES.NUM_CHARACTERS) {
                character = 0;
            }

            // If we've looped through the whole thing and haven't found an open slot
            counter++;
            if(counter >= (int)CHARACTERNAMES.NUM_CHARACTERS) {
                // Leave and return the starting character
                character = startChara;
                break;
            }
        }

        return character;
    }
    public CHARACTERNAMES GetNextAvailableCharacterDown(CHARACTERNAMES startChara) {
        CHARACTERNAMES character = startChara;

        int counter = 0;
        while (IsCharacterTaken(character)) {
            character = character - 1;
            if (character < 0) {
                character = CHARACTERNAMES.NUM_CHARACTERS-1;
            }

            // If we've looped through the whole thing and haven't found an open slot
            counter++;
            if (counter >= (int)CHARACTERNAMES.NUM_CHARACTERS) {
                // Leave and return the starting character
                character = startChara;
                break;
            }
        }

        return character;
    }

    public bool IsCharacterTaken(CHARACTERNAMES chara) {
        foreach (PlayerInfo p in _players) {
            if(p.characterName == chara) {
                return true;
            }
        }

        return false;
    }

    public void SetAimAssist(bool on) {
        foreach(PlayerInfo p in _players) {
            p.aimAssist = on;
        }
    }

    public void ClearAllPlayers() {
        _players.Clear();
    }

    public void SceneChanged(Scene scene, LoadSceneMode mode) {
        // If we've gone back to the main menu or reloaded the character select screen
        if (scene.name == "MainMenu" || scene.name == "CharacterSelect" || scene.name == "DemoCharacterSelect" || scene.name == "NetworkedCharacterSelect") {
            // Clear players
            ClearAllPlayers();
        }

        // Debugging
        if(scene.name == "NetworkedMapSelectWheel") {
            Debug.Log("Current Players:");

            for (int i = 0; i < NumPlayers; ++i) {
                Debug.Log("Player " + _players[i].playerNum + " on Team " + _players[i].team);
            }
        }
    }
}
