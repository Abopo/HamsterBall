using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    public GameObject playerObj;
    public GameObject aiPlayerObj;
    //public SpriteRenderer[] shiftMeterIcons = new SpriteRenderer[4]; // 0 - leftmeter1; 1 - leftmeter2; 2 - rightmeter; 3 - rightmeter2
    //public Image[] shiftMeterFronts = new Image[4];
    //public Image[] shiftMeterBacks = new Image[4];
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
    void Start () {
        _playerManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        bubSprites = Resources.LoadAll<Sprite>("Art/Hamsters_and_Bubbles/Bub_Sheet");
        GetSpawnLocations();
        SpawnPlayers();
    }

    void GetSpawnLocations() {
        spawns[0] = transform.GetChild(0);
        spawns[1] = transform.GetChild(1);
        spawns[2] = transform.GetChild(2);
        spawns[3] = transform.GetChild(3);
    }

    void SpawnPlayers() {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        PlayerInfo tempPlayerInfo;
        for(int i = 0; i < _playerManager.NumPlayers; ++i) {
            tempPlayerInfo = _playerManager.GetPlayerByIndex(i);

            PlayerController newPlayer;
            // If the player should be an AI
            if (tempPlayerInfo.controllerNum < 0) {
                newPlayer = Instantiate(aiPlayerObj).GetComponent<PlayerController>();
                newPlayer.GetComponent<AIBrain>().Difficulty = tempPlayerInfo.difficulty;
                newPlayer.GetComponent<AIBrain>().characterAI = tempPlayerInfo.characterAI;
            } else {
                newPlayer = Instantiate(playerObj).GetComponent<PlayerController>();
            }
            newPlayer.SetPlayerNum(tempPlayerInfo.playerNum);
            //newPlayer.inputState.controllerNum = tempPlayerInfo.joystickNum;
            //newPlayer.isLeftTeam = (tempPlayerInfo.team == 0 ? true : false);
            newPlayer.team = tempPlayerInfo.team;
            newPlayer.transform.position = FindSpawnPosition(newPlayer.team);
            newPlayer.GetComponent<Animator>().runtimeAnimatorController = FindAnimatorController(newPlayer.playerNum);

            if (!gameManager.isSinglePlayer) {
                SetupSwitchMeter(newPlayer);
                newPlayer.aimAssist = tempPlayerInfo.aimAssist;
            } else {
                newPlayer.aimAssist = gameManager.aimAssist;
            }

            _players.Add(newPlayer);
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

    RuntimeAnimatorController FindAnimatorController(int playerNum) {
        RuntimeAnimatorController controller = null;
        switch(playerNum) {
            case 1:
                controller = Resources.Load("Art/Animations/Player/Bub") as RuntimeAnimatorController;
                break;
            case 2:
                controller = Resources.Load("Art/Animations/Player/Bub2") as RuntimeAnimatorController;
                break;
            case 3:
                controller = Resources.Load("Art/Animations/Player/Bub3") as RuntimeAnimatorController;
                break;
            case 4:
                controller = Resources.Load("Art/Animations/Player/Bub4") as RuntimeAnimatorController;
                break;
        }

        return controller;
    }

    // TODO: this function is a little gross still, fix it up a bit
    public void SetupSwitchMeter(PlayerController player) {
        if(player.team == 0) {
            shiftMeters[leftMeters].GetMeterFront().enabled = true;
            shiftMeters[leftMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeterPosition(shiftMeters[leftMeters].GetMeterFront().GetComponent<RectTransform>());
            shiftMeters[leftMeters].GetIcon().sprite = bubSprites[(player.playerNum - 1)*12];
            shiftMeters[leftMeters++].GetIcon().enabled = true;
        } else if(player.team == 1) {
            shiftMeters[2+rightMeters].GetMeterFront().enabled = true;
            shiftMeters[2+rightMeters].GetMeterBack().enabled = true;
            player.GetComponent<PlayerGUI>().SetMeterPosition(shiftMeters[2+rightMeters].GetMeterFront().GetComponent<RectTransform>());
            shiftMeters[2+rightMeters].GetIcon().sprite = bubSprites[(player.playerNum - 1)*12];
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
