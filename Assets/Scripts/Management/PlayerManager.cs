using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerInfo {
    public int playerNum;
    //public CHARACTERNAMES characterName;
    public CharaInfo charaInfo = new CharaInfo();
    public int team; // -1 == none; 0 == left; 1 == right
    public bool aimAssist;
    public bool isAI; // whether this character is an ai or not
    public int difficulty; // Only used for AI, 0-3: easy-expert
    public string characterAI; // Only used for AI
    public int ownerID; // used for networking
}

public class PlayerManager : MonoBehaviour {
    List<PlayerInfo> _players = new List<PlayerInfo>();

    public int NumPlayers {
        get {
            return _players.Count;
        }
    }

    public bool AreAI {
        get {
            foreach(PlayerInfo pI in _players) {
                if(pI.isAI) {
                    return true;
                }
            }

            return false;
        }
    }

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(transform.gameObject);

        SceneManager.sceneLoaded += SceneChanged;
        //NumPlayers = 0;
    }

    public void MakeBackupPlayers() {
        PlayerInfo newPlayer = new PlayerInfo();
        newPlayer.playerNum = 0;
        newPlayer.charaInfo.name = CHARACTERS.BOY;
        newPlayer.isAI = false;
        newPlayer.team = 0;
        _players.Add(newPlayer);

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (!gameManager.isSinglePlayer) {
            PlayerInfo newPlayer2 = new PlayerInfo();
            newPlayer2.playerNum = 1;
            newPlayer.charaInfo.name = CHARACTERS.GIRL;
            newPlayer.isAI = true;
            newPlayer2.team = 1;
            _players.Add(newPlayer2);
        }
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

    public void SetTeam(int playerNum, int team) {
        foreach (PlayerInfo p in _players) {
            if (p.playerNum == playerNum) {
                // TODO: Might need to clean up memory here.
                int i = _players.IndexOf(p);
                _players[i].team = team;
                Debug.Log("Player " + playerNum + " added to Team " + team);
            }
        }
    }

    public void AddPlayer(int playerNum, bool isAI, CharaInfo charaInfo) {
        if (playerNum != -1) {
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.playerNum = playerNum;
            newPlayer.isAI = isAI;
            //newPlayer.characterName = charaName;
            newPlayer.charaInfo = charaInfo;
            newPlayer.team = charaInfo.team;
            newPlayer.difficulty = 0;
            _players.Add(newPlayer);
            _players.Sort((x, y) => x.playerNum.CompareTo(y.playerNum));
        }
    }

    public void AddPlayer(PlayerInfo player) {
        _players.Add(player);
        _players.Sort((x, y) => x.playerNum.CompareTo(y.playerNum));
    }

    // For networking
    public void AddPlayer(int playerNum, CharaInfo charaInfo, int ownerID) {
        if (playerNum != -1) {
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.playerNum = playerNum;
            newPlayer.isAI = false;
            newPlayer.charaInfo = charaInfo;
            newPlayer.team = charaInfo.team;
            newPlayer.difficulty = 0;
            newPlayer.ownerID = ownerID;
            _players.Add(newPlayer);
            _players.Sort((x, y) => x.playerNum.CompareTo(y.playerNum));
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

    public void SetAimAssist(bool on) {
        foreach(PlayerInfo p in _players) {
            p.aimAssist = on;
        }

        
    }

    public void ClearAllPlayers() {
        _players.Clear();
    }

    // Used in story mode to replay a stage
    public void ClearAIPlayers() {
        _players.RemoveAll(PlayerInfo => PlayerInfo.isAI == true);
    }

    public void SceneChanged(Scene scene, LoadSceneMode mode) {
        // If we've gone back to the main menu or reloaded the character select screen
        if (scene.name == "MainMenu" || scene.name == "CharacterSelect" || scene.name == "DemoCharacterSelect" || scene.name == "NetworkedCharacterSelect") {
            // Clear players
            ClearAllPlayers();
        }

		if (scene.name == "LocalPlay")
		{
			SoundManager.mainAudio.MenuGeneralEvent.setPaused(false);
			SoundManager.mainAudio.VillageMusicEvent.setPaused(true);
		} else if (scene.name == "VillageScene")
		{
			SoundManager.mainAudio.VillageMusicEvent.setPaused(false);
			SoundManager.mainAudio.MenuGeneralEvent.setPaused(true);
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
