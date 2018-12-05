﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerInfoBox : MonoBehaviour {

    public CHARACTERNAMES characterName;
    public bool playerAssigned;
    public int playerID;
    public Text playerText;
    public GameObject[] infoObjects;

    Player _player;

    SpriteRenderer _sprite;
    Sprite[] characterSprites = new Sprite[6];

    Player _player1; // The first player's player object

    CharacterSelectWindow _characterSelectWindow;
    GameManager _gameManager;

    private void Awake() {
        _characterSelectWindow = FindObjectOfType<StorySelectMenu>().characterSelectWindow;
        _gameManager = FindObjectOfType<GameManager>();
    }
    // Use this for initialization
    void Start () {
        _player = ReInput.players.GetPlayer(playerID);

        _sprite = GetComponentInChildren<SpriteRenderer>(true);
        Sprite[] boySprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Warp-Screen-Assets");
        Sprite[] girlSprites = Resources.LoadAll<Sprite>("Art/UI/Level UI/Girl-Icon");
        characterSprites[0] = boySprites[0];
        characterSprites[1] = boySprites[1];
        characterSprites[2] = boySprites[2];
        characterSprites[3] = boySprites[3];
        characterSprites[4] = girlSprites[0];
        characterSprites[5] = girlSprites[1];

        if (playerAssigned) {
            _player.controllers.GetController(ControllerType.Joystick, 0);
        }

        // Get the first player
        _player1 = ReInput.players.GetPlayer(0);
    }

    private void OnLevelWasLoaded(int level) {
        // If we've loaded in story select
        if (level == 2) {
            _characterSelectWindow = FindObjectOfType<StorySelectMenu>().characterSelectWindow;
        }
    }

    // Update is called once per frame
    void Update () {
		if(!playerAssigned) {
            // Wait for some kind of input
            foreach (Joystick jStick in ReInput.controllers.Joysticks) {
                // Don't check the controller already assigned to player1
                if (_player1.controllers.ContainsController(jStick)) {
                    continue;
                }

                if(jStick.GetAnyButtonDown()) {
                    _player.controllers.AddController(jStick, false);
                    // Open the character select window so the player can choose a character
                    _characterSelectWindow.Activate(this);
                    _gameManager.isCoop = true;
                }
            }
        } else {
            // if the player wants to change characters
            if(_player.GetButtonDown("Shift")) {
                // Open the character select window
                _characterSelectWindow.Activate(this);
            }
            // If the second player wants to quit out
            if(playerID != 0 && _player.GetButtonDown("Cancel")) {
                // Reset this info box
                playerAssigned = false;
                _player.controllers.ClearAllControllers();

                _gameManager.isCoop = false;
            }
        }
	}

    public void SetCharacter(CHARACTERNAMES character) {
        playerAssigned = true;

        characterName = character;

        if (playerID == 0) {
            playerText.text = "Player 1";
        } else {
            playerText.text = "Player 2";
        }
        foreach (GameObject gO in infoObjects) {
            gO.SetActive(true);
        }

        // Set sprite to correct character
        _sprite.sprite = characterSprites[(int)characterName];
    }

    public void LoadCharacter() {
        if (playerAssigned) {
            PlayerManager playerManager = FindObjectOfType<PlayerManager>();

            PlayerInfo player = new PlayerInfo();
            player.playerNum = playerID;
            player.characterName = characterName;
            player.team = 0;
            playerManager.AddPlayer(player);
        }
    }
}
