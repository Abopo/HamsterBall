using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    public GameObject playerObj;
    public GameObject aiPlayerObj;
    public ShiftMeter[] shiftMeters = new ShiftMeter[4];

    int leftMeters = 0;
    int rightMeters = 0;

    Sprite[,] playerIcons = new Sprite[7,4];

    List<PlayerController> _players = new List<PlayerController>();
    PlayerManager _playerManager;

    Transform[] spawns = new Transform[4]; // 0 - leftspawn1; 1 - leftspawn2; 2 - rightspawn1; 3 - rightspawn2
    int leftSpawned = 0;
    int rightSpawned = 0;

    private void Awake() {
        
    }
    // Use this for initialization
    void Start () {
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();

        LoadPlayerIcons(playerIcons);
        GetSpawnLocations();
        SpawnPlayers();
    }

    static public void LoadPlayerIcons(Sprite[,] icons) {
        // Get sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Icons-Master-File");

        // Boy
        icons[(int)CHARACTERS.BOY, 0] = sprites[0];
        icons[(int)CHARACTERS.BOY, 1] = sprites[1];
        icons[(int)CHARACTERS.BOY, 2] = sprites[2];
        icons[(int)CHARACTERS.BOY, 3] = sprites[3];
        // Girl
        icons[(int)CHARACTERS.GIRL, 0] = sprites[4];
        icons[(int)CHARACTERS.GIRL, 1] = sprites[5];
        icons[(int)CHARACTERS.GIRL, 2] = sprites[6];
        icons[(int)CHARACTERS.GIRL, 3] = sprites[7];
        // Rooster
        icons[(int)CHARACTERS.ROOSTER, 0] = sprites[12];
        icons[(int)CHARACTERS.ROOSTER, 1] = sprites[13];
        icons[(int)CHARACTERS.ROOSTER, 2] = sprites[14];
        icons[(int)CHARACTERS.ROOSTER, 3] = sprites[15];
        // Bat
        icons[(int)CHARACTERS.BAT, 0] = sprites[8];
        icons[(int)CHARACTERS.BAT, 1] = sprites[9];
        icons[(int)CHARACTERS.BAT, 2] = sprites[10];
        icons[(int)CHARACTERS.BAT, 3] = sprites[11];
        // Lackey
        icons[(int)CHARACTERS.LACKEY, 0] = sprites[16];
        icons[(int)CHARACTERS.LACKEY, 1] = sprites[17];
        icons[(int)CHARACTERS.LACKEY, 2] = sprites[18];
        icons[(int)CHARACTERS.LACKEY, 3] = sprites[19];
        // Snail
        icons[(int)CHARACTERS.SNAIL, 0] = sprites[20];
        icons[(int)CHARACTERS.SNAIL, 1] = sprites[21];
        icons[(int)CHARACTERS.SNAIL, 2] = sprites[22];
        icons[(int)CHARACTERS.SNAIL, 3] = sprites[23];
        // Lizard
        icons[(int)CHARACTERS.LIZARD, 0] = sprites[24];
        icons[(int)CHARACTERS.LIZARD, 1] = sprites[25];
        icons[(int)CHARACTERS.LIZARD, 2] = sprites[24];
        icons[(int)CHARACTERS.LIZARD, 3] = sprites[25];
    }

    void GetSpawnLocations() {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
        // Get the spawn points in the correct order
        int team = 0, order = 0, i = 0, j = 0, loopCount = 0;
        while(j < 4 && loopCount < 24) {
            if(spawnPoints[i].team == team && spawnPoints[i].order == order) {
                spawns[j] = spawnPoints[i].transform;
                ++j;

                if(order == 1) {
                    order = 0;
                    team++;
                } else {
                    order++;
                }
            }

            ++i;
            if(i >= spawnPoints.Length) {
                i = 0;
            }

            loopCount++;
        }

        // If we looped too many times
        if(loopCount == 24) {
            // Something was initialized wrong
            Debug.Log("Spawn points not initialized properly, assigning randomly");

            // Just assign best we can
            for(i = 0; i < 4; ++i) {
                spawns[i] = spawnPoints[i].transform;
            }
        }
    }

    void SpawnPlayers() {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        PlayerInfo tempPlayerInfo;

        if(_playerManager.NumPlayers == 0) {
            _playerManager.MakeBackupPlayers();
        }

        for(int i = 0; i < _playerManager.NumPlayers; ++i) {
            tempPlayerInfo = _playerManager.GetPlayerByIndex(i);

            PlayerController newPlayer;
            // If the player should be an AI
            if (tempPlayerInfo.isAI) {
                newPlayer = Instantiate(aiPlayerObj).GetComponentInChildren<PlayerController>();
                newPlayer.GetComponent<AIBrain>().Difficulty = tempPlayerInfo.difficulty;
                SetCharacterAI(tempPlayerInfo.characterAI, newPlayer);
            } else {
                newPlayer = Instantiate(playerObj).GetComponentInChildren<PlayerController>();
            }
            newPlayer.SetPlayerNum(tempPlayerInfo.playerNum);
            newPlayer.SetInputID(tempPlayerInfo.playerNum);
            newPlayer.team = tempPlayerInfo.team;
            // Default player to left team if they aren't assigned one
            if (newPlayer.team == -1) {
                newPlayer.team = 0;
            }
            newPlayer.transform.position = FindSpawnPosition(newPlayer.team);
            newPlayer.SetCharacterInfo(tempPlayerInfo.charaInfo);

            if (!gameManager.isSinglePlayer) {
                SetupSwitchMeter(newPlayer);
                // TODO: Have aim assist per-player?
                //newPlayer.aimAssist = tempPlayerInfo.aimAssist;
                newPlayer.aimAssist = gameManager.gameSettings.aimAssistMultiplayer;
            } else {
                if (gameManager.gameSettings.aimAssistSetting == AIMASSIST.ALWAYS) {
                    newPlayer.aimAssist = true;
                } else {
                    newPlayer.aimAssist = gameManager.gameSettings.aimAssistSingleplayer;
                }
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

    Vector3 FindSpawnPosition(int team) {
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
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[leftMeters]);
            shiftMeters[leftMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name, player.CharaInfo.color-1];
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if(player.team == 1) {
            shiftMeters[2 + rightMeters].gameObject.SetActive(true);
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[2+rightMeters]);
            shiftMeters[2+rightMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name, player.CharaInfo.color-1];
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
