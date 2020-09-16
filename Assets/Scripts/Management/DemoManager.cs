﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class DemoManager : MonoBehaviour {

    bool _comMatch;

    public string _curScene;
    float _waitTime = 30f;
    float _waitTimer = 0f;

    float _nextMatchTime = 15f;
    float _nextMatchTimer = 0f;

    SuperTextMesh _controllerText;

    GameManager _gameManager;
    PlayerManager _playerManager;
    LevelManager _levelManager;

    public bool ComMatch {
        get { return _comMatch; }
    }

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _playerManager = transform.parent.GetComponent<PlayerManager>();

        _controllerText = GetComponentInChildren<SuperTextMesh>();

        SceneManager.sceneLoaded += OnSceneChange;
    }

    // Start is called before the first frame update
    void Start() {
        _comMatch = false;
        _controllerText.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if(_curScene == "PlayableCharacterSelect") {
            // If we're on the character select screen for too long (without any input)
            _waitTimer += Time.deltaTime;
            if(_waitTimer >= _waitTime) {
                _waitTimer = 0f;
                // Start up a random match with cpu's
                StartComMatch();
            }

            // If any button is pressed reset the timer
            if(AnyButtonPressed()) {
                _waitTimer = 0f;
            }
        // If we're in the versus scene
        } else if(_comMatch && _curScene == "VersusMultiplayer") {
            // And any button is pressed
            if(AnyButtonPressed()) {
                _comMatch = false;

                // Turn off the text
                _controllerText.enabled = false;

                // Return to the character select screen
                _gameManager.CharacterSelectButton();
            }

            // If the match is over and the results are showing
            // Continue to the next game
            if(_levelManager.GameOver) {
                _nextMatchTimer += Time.deltaTime;
                if(_nextMatchTimer >= _nextMatchTime) {
                    _nextMatchTimer = 0f;

                    // If the set isn't over yet
                    if(_gameManager.leftTeamGames < 2 && _gameManager.rightTeamGames < 2) {
                        // Hit continue
                        _levelManager.continueScreen.ContinueToNextLevel();
                    } else {
                        // We want to start a new random cpu match
                        StartComMatch();
                    }
                }
            }
        }
    }

    public void StartComMatch() {
        // Make sure the game manager is clean
        _gameManager.CleanUp(true);

        // Make two random com players
        PlayerInfo com1 = new PlayerInfo();
        com1.playerNum = 1;
        com1.charaInfo.name = (CHARACTERS)Random.Range(0, (int)CHARACTERS.NUM_CHARACTERS);
        com1.charaInfo.color = Random.Range(1, 4);
        com1.charaInfo.team = 0;
        com1.team = 0;
        com1.isAI = true;
        com1.difficulty = Random.Range(3, 8);
        Debug.Log("Spawned " + com1.charaInfo.name.ToString() + " as " + com1.charaInfo.color.ToString());
        PlayerInfo com2 = new PlayerInfo();
        com2.playerNum = 2;
        com2.charaInfo.name = (CHARACTERS)Random.Range(0, (int)CHARACTERS.NUM_CHARACTERS);
        com2.charaInfo.color = Random.Range(1, 4);
        com2.charaInfo.team = 1;
        com2.team = 1;
        com2.isAI = true;
        com2.difficulty = Random.Range(3, 8);
        Debug.Log("Spawned " + com2.charaInfo.name.ToString() + " as " + com2.charaInfo.color.ToString());

        // Add the players to the player manager
        _playerManager.ClearAllPlayers();
        _playerManager.AddPlayer(com1);
        _playerManager.AddPlayer(com2);

        // Show the text
        //_controllerText.enabled = true;

        _comMatch = true;


        // Load a random stage
        int board = Random.Range(0, 4);
        _gameManager.selectedBoard = (BOARDS)board;
        SceneManager.LoadScene("VersusMultiplayer");
    }

    void OnSceneChange(Scene scene, LoadSceneMode mode) {
        _curScene = scene.name;

        _levelManager = FindObjectOfType<LevelManager>();
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
