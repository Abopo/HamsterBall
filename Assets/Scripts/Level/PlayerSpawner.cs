using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    public GameObject playerObj;
    public GameObject aiPlayerObj;
    public ShiftMeter[] shiftMeters = new ShiftMeter[4];

    int leftMeters = 0;
    int rightMeters = 0;

    Sprite[] playerIcons = new Sprite[(int)CHARACTERS.NUM_CHARACTERS];

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

    static public void LoadPlayerIcons(Sprite[] icons) {
        // Get sprites
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Icons-False-Colors-Master-File");

        icons[(int)CHARACTERS.BOY] = sprites[1];
        icons[(int)CHARACTERS.GIRL] = sprites[4];
        icons[(int)CHARACTERS.ROOSTER] = sprites[9];
        icons[(int)CHARACTERS.BAT] = sprites[0];
        icons[(int)CHARACTERS.OWL] = sprites[8];
        icons[(int)CHARACTERS.GOAT] = sprites[5];
        icons[(int)CHARACTERS.SNAIL] = sprites[10];
        icons[(int)CHARACTERS.LIZARD] = sprites[7];
        icons[(int)CHARACTERS.LACKEY] = sprites[6];
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
            shiftMeters[leftMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name];
            shiftMeters[leftMeters].GetIcon().material = player.SpriteRenderer.material;
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if(player.team == 1) {
            shiftMeters[2 + rightMeters].gameObject.SetActive(true);
            player.GetComponent<PlayerGUI>().SetMeter(shiftMeters[2+rightMeters]);
            shiftMeters[2+rightMeters].GetIcon().sprite = playerIcons[(int)player.CharaInfo.name];
            shiftMeters[2+rightMeters].GetIcon().material = player.SpriteRenderer.material;
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
