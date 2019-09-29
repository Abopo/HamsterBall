using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCountSelectMenu : MonoBehaviour {
    public MenuButton[] playerButtons;
    public MenuButton[] comButtons;
    public GameObject playerCountMenu;
    public GameObject aiCountMenu;

    LocalPlayMenu lpMenu;

    int _maxPlayers;
    int _minPlayers;
    bool _aiAllowed;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        lpMenu = FindObjectOfType<LocalPlayMenu>();
        _gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.activeSelf) {
            if(_gameManager.playerInput.GetButtonDown("Cancel")) {
                Deactivate();
            }
        }
	}

    public void Activate(int maxPlayers, int minPlayers, bool aiAllowed) {
        gameObject.SetActive(true);
        _maxPlayers = maxPlayers;
        _minPlayers = minPlayers;
        _aiAllowed = aiAllowed;

        if (!aiAllowed) {
            // TODO: Adjust for number of connected controllers?
            for (int i = 0; i < minPlayers - 1; ++i) {
                playerButtons[i].Disable();
            }
        }

        // Highlight the first available button
        playerButtons[minPlayers - 1].isFirstSelection = true;
        playerButtons[minPlayers - 1].Highlight();
        for(int i = minPlayers-1; i < maxPlayers; ++i) {
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
/*
    public void OnePlayer() {
        Debug.Log("One Player");

        // Set player count
        _gameManager.numPlayers = 1;

        if (_aiAllowed && _minPlayers > 1) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            _gameManager.CharacterSelectButton();
        }
    }
    public void TwoPlayers() {
        Debug.Log("Two Players");
        
        // Set player count
        _gameManager.numPlayers = 2;

        if (_aiAllowed && _maxPlayers > 2) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            _gameManager.CharacterSelectButton();
        }
    }
    public void ThreePlayers() {
        Debug.Log("Three Players");

        // Set player count
        _gameManager.numPlayers = 3;

        if (_aiAllowed && _maxPlayers > 3) {
            // Open AI selection
            OpenAICountMenu();
        } else {
            // Go to character select
            _gameManager.CharacterSelectButton();
        }
    }
    public void FourPlayers() {
        Debug.Log("Four Players");

        // Can't have more than 4 players so go straight to character select

        // Set player count
        _gameManager.numPlayers = 4;
        _gameManager.numAI = 0;

        _gameManager.CharacterSelectButton();
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
            comButtons[1].Highlight();
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

        _gameManager.CharacterSelectButton();
    }
    public void OneComPlayers() {
        // Set ai count
        _gameManager.numAI = 1;

        _gameManager.CharacterSelectButton();
    }
    public void TwoComPlayers() {
        // Set ai count
        _gameManager.numAI = 2;

        _gameManager.CharacterSelectButton();
    }
    public void ThreeComPlayers() {
        // Set ai count
        _gameManager.numAI = 3;

        _gameManager.CharacterSelectButton();
    }

    */
}
