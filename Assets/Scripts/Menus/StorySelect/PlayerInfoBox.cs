using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class PlayerInfoBox : MonoBehaviour {

    //public CHARACTERNAMES characterName;
    public CharaInfo characterInfo;
    public bool playerAssigned;
    public int playerID;
    public GameObject activeObjects;
    public GameObject deactiveObjects;

    Player _myPlayer;
    public Player MyPlayer {
        get { return _myPlayer; }
    }

    SpriteRenderer _sprite;
    Sprite _boySprite;
    Sprite _girlSprite;
    //Sprite[] characterSprites = new Sprite[8];

    Player _player1; // The first player's player object

    CharacterSelectWindow _characterSelectWindow;
    GameManager _gameManager;


    private void Awake() {
        _sprite = GetComponentInChildren<SpriteRenderer>(true);

        Sprite[] icons = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Icons-False-Colors-Master-File");
        _boySprite = icons[1];
        _girlSprite = icons[4];

        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();

        _myPlayer = ReInput.players.GetPlayer(playerID);

        if (playerAssigned) {
            _myPlayer.controllers.GetController(ControllerType.Joystick, 0);
        }

        UpdateCharacterInfo();

        // Get the first player
        _player1 = ReInput.players.GetPlayer(0);

        _characterSelectWindow = FindObjectOfType<CharacterSelectWindow>();
    }

    void UpdateCharacterInfo() {
        if (playerAssigned) {
            // Set player to correct character
            CharaInfo tempInfo = new CharaInfo();
            if (playerID == 0) {
                tempInfo.name = (CHARACTERS)ES3.Load<int>("Player1Character", 0);
                tempInfo.color = ES3.Load<int>("Player1Color", 0);
            } else if (playerID == 1) {
                tempInfo.name = (CHARACTERS)ES3.Load<int>("Player2Character", 0);
                tempInfo.color = ES3.Load<int>("Player2Color", 0);
            }

            SetCharacter(tempInfo);
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode) {
        // Why the fuck can we get here if the object is null
        if (this == null) {
            return;
        }
        // If we've loaded in story select
        if (scene.buildIndex == 3) {
            _characterSelectWindow = FindObjectOfType<CharacterSelectWindow>();
        }

        UpdateCharacterInfo();
    }

    // Update is called once per frame
    void Update () {
		if(!playerAssigned) {
            // Wait for some kind of input
            foreach(Player p in ReInput.players.AllPlayers) {
                if(_player1 != p) {
                    if(p.GetAnyButtonDown()) {
                        _myPlayer = p;
                        // Open the character select window so the player can choose a character
                        _characterSelectWindow.Activate(this);
                        _gameManager.isCoop = true;
                    }
                }
            }
        } else {
            // if the player wants to change characters
            if(_myPlayer.GetButtonDown("Shift")) {
                // Open the character select window
                _characterSelectWindow.Activate(this);
            }
            // If the second player wants to quit out
            if(playerID != 0 && _myPlayer.GetButtonDown("Cancel")) {
                // Reset this info box
                TurnOff();

                playerAssigned = false;
                _myPlayer = null;

                _gameManager.isCoop = false;
            }
        }
	}

    public void SetCharacter(CharaInfo charaInfo) {
        playerAssigned = true;

        characterInfo = charaInfo;

        if (playerID == 0) {
            ES3.Save<int>("Player1Character", charaInfo.name);
            ES3.Save<int>("Player1Color", charaInfo.color);
        } else {
            ES3.Save<int>("Player2Character", charaInfo.name);
            ES3.Save<int>("Player2Color", charaInfo.color);
        }

        // Turn on the correct UI
        activeObjects.SetActive(true);
        deactiveObjects.SetActive(false);

        // Set sprite to correct character
        if (charaInfo.name == CHARACTERS.BOY) {
            _sprite.sprite = _boySprite;
            _sprite.material = Resources.Load<Material>("Materials/Character Palettes/Boy/Boy" + charaInfo.color);
        } else {
            _sprite.sprite = _girlSprite;
            _sprite.material = Resources.Load<Material>("Materials/Character Palettes/Girl/Girl" + charaInfo.color);
        }
    }

    void TurnOff() {
        // Turn off the correct UI
        activeObjects.SetActive(false);
        deactiveObjects.SetActive(true);
    }

    public void LoadCharacter() {
        if (playerAssigned) {
            PlayerManager playerManager = FindObjectOfType<PlayerManager>();

            PlayerInfo player = new PlayerInfo();
            player.playerNum = playerID;
            player.charaInfo = characterInfo;
            player.team = 0;

            /*
            if (charaColor < CHARACTERCOLORS.GIRL1) {
                player.charaInfo.name = CHARACTERS.BOY;
                player.charaInfo.color = (int)charaColor + 1;
            } else {
                player.charaInfo.name = CHARACTERS.GIRL;
                player.charaInfo.color = (int)charaColor - 3;
            }
            */

            playerManager.AddPlayer(player);
        }
    }
}
