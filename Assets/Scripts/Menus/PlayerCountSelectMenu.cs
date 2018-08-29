﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCountSelectMenu : MonoBehaviour {

    public MenuButton[] playerButtons;
    public MenuButton[] comButtons;
    public GameObject playerCountMenu;
    public GameObject aiCountMenu;

    LocalPlayMenu lpMenu;

    int _maxPlayers;
    int _minPlayers;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        lpMenu = FindObjectOfType<LocalPlayMenu>();
        _gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.activeSelf) {
            if(Input.GetButtonDown("Cancel")) {
                Deactivate();
            }
        }
	}

    public void Activate(int maxPlayers, int minPlayers) {
        gameObject.SetActive(true);
        _maxPlayers = maxPlayers;
        _minPlayers = minPlayers;

        // TODO: Adjust for number of connected controllers?

        for(int i = 0; i < maxPlayers; ++i) {
            playerButtons[i].Enable();
        }

        // Deactivate buttons beyond max players
        for(int i = maxPlayers; i < 4; ++i) {
            playerButtons[i].Disable();
        }
    }

    public void Deactivate() {
        if (aiCountMenu.activeSelf) {
            aiCountMenu.SetActive(false);
            playerCountMenu.SetActive(true);
        } else {
            gameObject.SetActive(false);
            lpMenu.Activate();
        }
    }

    public void OnePlayer() {
        // Set player count
        _gameManager.numPlayers = 1;

        if (_minPlayers > 1) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            SceneManager.LoadScene("NewCharacterSelect");
        }
    }
    public void TwoPlayers() {
        // Set player count
        _gameManager.numPlayers = 2;

        if (_maxPlayers > 2) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            SceneManager.LoadScene("NewCharacterSelect");
        }
    }
    public void ThreePlayers() {
        // Set player count
        _gameManager.numPlayers = 3;

        if (_maxPlayers > 3) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            SceneManager.LoadScene("NewCharacterSelect");
        }
    }
    public void FourPlayers() {
        // Can't have more than 4 players so go straight to character select

        // Set player count
        _gameManager.numPlayers = 4;

        SceneManager.LoadScene("NewCharacterSelect");
    }

    void OpenAICountMenu() {
        // Close player count menu
        playerCountMenu.SetActive(false);

        // Open AI count menu
        aiCountMenu.SetActive(true);

        // If we only have 1 player but need at least 2
        if(_minPlayers > 1 && _gameManager.numPlayers < 2) {
            // Disable just the 0 ai button
            comButtons[0].Disable();
            comButtons[1].Enable();
            comButtons[2].Enable();
            comButtons[3].Enable();
        } else if(_gameManager.numPlayers == 2 && _maxPlayers == 4) {
            // Disable just the 3 ai button
            comButtons[0].Enable();
            comButtons[1].Enable();
            comButtons[2].Enable();
            comButtons[3].Disable();
        } else if(_gameManager.numPlayers == 3 && _maxPlayers == 4) {
            // Disable the 2 and 3 ai buttons
            comButtons[0].Enable();
            comButtons[1].Enable();
            comButtons[2].Disable();
            comButtons[3].Disable();
        }
    }
    public void ZeroComPlayers() {
        // Set ai count
        _gameManager.numAI = 0;

        SceneManager.LoadScene("NewCharacterSelect");
    }
    public void OneComPlayers() {
        // Set ai count
        _gameManager.numAI = 1;

        SceneManager.LoadScene("NewCharacterSelect");
    }
    public void TwoComPlayers() {
        // Set ai count
        _gameManager.numAI = 2;

        SceneManager.LoadScene("NewCharacterSelect");
    }
    public void ThreeComPlayers() {
        // Set ai count
        _gameManager.numAI = 3;

        SceneManager.LoadScene("NewCharacterSelect");
    }

}
