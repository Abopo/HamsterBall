using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    public GameObject playerObj;
    public GameObject aiPlayerObj;
    public ShiftMeter[] shiftMeters = new ShiftMeter[4];

    int leftMeters = 0;
    int rightMeters = 0;
    Sprite[] playerIcons = new Sprite[8];

    List<PlayerController> _players = new List<PlayerController>();
    PlayerManager _playerManager;

    Transform[] spawns = new Transform[4]; // 0 - leftspawn1; 1 - leftspawn2; 2 - rightspawn1; 3 - rightspawn2
    int leftSpawned = 0;
    int rightSpawned = 0;

    // Use this for initialization
    void Start () {
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();

        // Get sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        playerIcons[0] = sprites[0];
        playerIcons[1] = sprites[1];
        playerIcons[2] = sprites[2];
        playerIcons[3] = sprites[3];
        sprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Girl-Icon");
        playerIcons[4] = sprites[0];
        playerIcons[5] = sprites[1];
        playerIcons[6] = sprites[2];
        playerIcons[7] = sprites[3];

        GetSpawnLocations();
        SpawnPlayers();
    }

    void GetSpawnLocations() {
        SpawnPoint[] spawnPoints = GetComponentsInChildren<SpawnPoint>();
        for(int i = 0; i < spawnPoints.Length; ++i) {
            spawns[i] = spawnPoints[i].transform;
        }
        //spawns[0] = spawnPoints[0].transform;
        //spawns[1] = spawnPoints[1].transform;
        //spawns[2] = spawnPoints[2].transform;
        //spawns[3] = spawnPoints[3].transform;
    }

    void SpawnPlayers() {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        PlayerInfo tempPlayerInfo;
        for(int i = 0; i < _playerManager.NumPlayers; ++i) {
            tempPlayerInfo = _playerManager.GetPlayerByIndex(i);

            PlayerController newPlayer;
            // If the player should be an AI
            if (tempPlayerInfo.controllerNum < 0) {
                newPlayer = Instantiate(aiPlayerObj).GetComponentInChildren<PlayerController>();
                newPlayer.GetComponent<AIBrain>().Difficulty = tempPlayerInfo.difficulty;
                SetCharacterAI(tempPlayerInfo.characterAI, newPlayer);
            } else {
                newPlayer = Instantiate(playerObj).GetComponentInChildren<PlayerController>();
            }
            newPlayer.SetPlayerNum(tempPlayerInfo.playerNum);
            newPlayer.team = tempPlayerInfo.team;
            newPlayer.transform.position = FindSpawnPosition(newPlayer.team);
            newPlayer.SetCharacterName(tempPlayerInfo.characterName);

            if (!gameManager.isSinglePlayer) {
                SetupSwitchMeter(newPlayer);
                newPlayer.aimAssist = tempPlayerInfo.aimAssist;
            } else {
                newPlayer.aimAssist = gameManager.aimAssist;
            }

            _players.Add(newPlayer);
        }
    }

    void SetCharacterAI(string character, PlayerController player) {
        switch (character) {
            case "GeneralHam":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<GeneralHamAI>();
                break;
            case "Owl":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<OwlAI>();
                break;
            case "MountainGoat":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<MountainGoatAI>();
                break;
            case "Snail":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<SnailAI>();
                break;
            case "Rooster":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<RoosterAI>();
                break;
            case "Slime":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<SlimeAI>();
                break;
            case "City":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<CityCharaAI>();
                break;
            case "Bat":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<BatAI>();
                break;
            case "Villain":
                player.GetComponent<AIBrain>().characterAI = player.gameObject.AddComponent<VillainAI>();
                break;
        }
    }

    Vector2 FindSpawnPosition(int team) {
        if(team == 0) {
            return spawns[leftSpawned++].position;
        } else if (team == 1) {
            return spawns[2+rightSpawned++].position;
        } else {
            return Vector2.zero;
        }
    }

    // TODO: this function is a little gross still, fix it up a bit
    public void SetupSwitchMeter(PlayerController player) {
        if(player.team == 0) {
            shiftMeters[leftMeters].gameObject.SetActive(true);
            //shiftMeters[leftMeters].GetMeterFront().enabled = true;
            //shiftMeters[leftMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[leftMeters]);
            shiftMeters[leftMeters].GetIcon().sprite = playerIcons[(int)player.CharacterName];
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if(player.team == 1) {
            shiftMeters[2 + rightMeters].gameObject.SetActive(true);
            //shiftMeters[2+rightMeters].GetMeterFront().enabled = true;
            //shiftMeters[2+rightMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[2+rightMeters]);
            shiftMeters[2+rightMeters].GetIcon().sprite = playerIcons[(int)player.CharacterName];
            shiftMeters[2+rightMeters++].GetIcon().enabled = true;
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

    public PlayerController GetPlayer(int playerNum) {
        PlayerController player = null;

        foreach (PlayerController p in _players) {
            if(p.playerNum == playerNum) {
                player = p;
            }
        }

        return player;
    }
}
