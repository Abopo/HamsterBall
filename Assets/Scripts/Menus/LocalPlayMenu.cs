using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class LocalPlayMenu : MonoBehaviour {

    public PlayerCountSelectMenu pcsMenu;

    public MenuButton[] buttons;

    bool _isActive;

    GameManager _gameManager;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();

        // If this is the demo
        if (_gameManager.demoMode) {
            // Skip straight to character select
            SceneManager.LoadScene("PlayableCharacterSelect");
        }
    }
    // Use this for initialization
    void Start () {

        _isActive = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (_isActive) {
            if (ReInput.players.GetPlayer(0).GetButtonDown("Cancel")) {
                FindObjectOfType<GameManager>().VillageButton();
            }
        }
    }

    public void StandardVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
        _gameManager.maxPlayers = 4;
        _gameManager.CharacterSelectButton();

        //OpenPlayerCountSelectMenu(4, 2, true);
    }

    public void PartyVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_PARTY);
        _gameManager.maxPlayers = 4;
        _gameManager.CharacterSelectButton();
        //OpenPlayerCountSelectMenu(4, 2, true);
    }

    public void Survival() {
        _gameManager.SetGameMode(GAME_MODE.SURVIVAL);
        _gameManager.maxPlayers = 2;
        _gameManager.CharacterSelectButton();
        //OpenPlayerCountSelectMenu(2, 1, false);
    }

    public void TeamSurvival() {
        _gameManager.SetGameMode(GAME_MODE.TEAMSURVIVAL);
        _gameManager.maxPlayers = 4;
        _gameManager.CharacterSelectButton();
        //OpenPlayerCountSelectMenu(4, 2, false);
    }

    public void PuzzleChallenge() {
        _gameManager.SetGameMode(GAME_MODE.SP_CLEAR);
        _gameManager.maxPlayers = 2;
        _gameManager.CharacterSelectButton();
        //OpenPlayerCountSelectMenu(2, 1, false);
    }

    public void OpenPlayerCountSelectMenu(int maxPlayers, int minPlayers, bool aiAllowed) {
        Deactivate();
        pcsMenu.Activate(maxPlayers, minPlayers, aiAllowed);
    }

    public void Activate() {
        // Turn on buttons
        foreach(MenuButton mb in buttons) {
            if (mb.IsInteractable) {
                mb.Enable();
            }
        }

        _isActive = true;
    }
    void Deactivate() {
        // Turn off buttons
        foreach (MenuButton mb in buttons) {
            mb.isReady = false;
        }

        _isActive = false;
    }
}
