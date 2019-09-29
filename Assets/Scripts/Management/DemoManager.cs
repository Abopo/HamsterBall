﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class DemoManager : MonoBehaviour {

    string _curScene;
    float _waitTime = 30f;
    float _waitTimer = 0f;

    GameManager _gameManager;
    PlayerManager _playerManager;

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _playerManager = transform.parent.GetComponent<PlayerManager>();

        SceneManager.sceneLoaded += OnSceneChange;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(_curScene == "PlayableCharacterSelect") {
            // If we're on the character select screen for too long (without any input)
            _waitTimer += Time.deltaTime;
            if(_waitTimer >= _waitTime) {
                // Start up a random match with cpu's
                StartComMatch();
            }

            // If any button is pressed reset the timer
            if(AnyButtonPressed()) {
                _waitTimer = 0f;
            }
        }
    }

    void StartComMatch() {
        // Make two random com players
        PlayerInfo com1 = new PlayerInfo();
        com1.playerNum = 1;
        com1.charaInfo.name = (CHARACTERS)Random.Range(0, (int)CHARACTERS.NUM_CHARACTERS);
        com1.charaInfo.color = Random.Range(0, 4);
        com1.charaInfo.team = 0;
        com1.team = 0;
        com1.isAI = true;
        com1.difficulty = Random.Range(3, 8);
        PlayerInfo com2 = new PlayerInfo();
        com2.playerNum = 2;
        com2.charaInfo.name = (CHARACTERS)Random.Range(0, (int)CHARACTERS.NUM_CHARACTERS);
        com2.charaInfo.color = Random.Range(1, 5);
        com2.charaInfo.team = 1;
        com2.team = 1;
        com2.isAI = true;
        com2.difficulty = Random.Range(3, 8);
        
        // Add the players to the player manager
        _playerManager.AddPlayer(com1);
        _playerManager.AddPlayer(com2);

        // Load a random stage
        int board = Random.Range(0, 3);
        _gameManager.selectedBoard = (BOARDS)board;
        SceneManager.LoadScene("VersusMultiplayer");
    }

    void OnSceneChange(Scene scene, LoadSceneMode mode) {
        _curScene = scene.name;
    }

    bool AnyButtonPressed() {
        foreach(Controller con in ReInput.controllers.Controllers) {
            if(con.GetAnyButtonChanged()) {
                return true;
            }
        }

        return false;
    }
}
