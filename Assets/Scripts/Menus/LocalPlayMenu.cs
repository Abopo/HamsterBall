using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalPlayMenu : MonoBehaviour {

    public PlayerCountSelectMenu pcsMenu;

    public MenuButton[] buttons;

    bool _isActive;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();

        _isActive = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (_isActive) {
            if (Input.GetButtonDown("Cancel")) {
                FindObjectOfType<GameManager>().MainMenuButton();
            }
        }
    }

    public void StandardVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
        OpenPlayerCountSelectMenu(4, 2, true);
    }

    public void PartyVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_PARTY);
        OpenPlayerCountSelectMenu(4, 2, true);
    }

    public void Survival() {
        _gameManager.SetGameMode(GAME_MODE.SURVIVAL);
        OpenPlayerCountSelectMenu(2, 1, false);
    }

    public void TeamSurvival() {
        _gameManager.SetGameMode(GAME_MODE.TEAMSURVIVAL);
        OpenPlayerCountSelectMenu(4, 2, false);
    }

    public void PuzzleChallenge() {
        _gameManager.SetGameMode(GAME_MODE.SP_CLEAR);
        OpenPlayerCountSelectMenu(2, 1, false);
    }

    public void OpenPlayerCountSelectMenu(int maxPlayers, int minPlayers, bool aiAllowed) {
        Deactivate();
        pcsMenu.Activate(maxPlayers, minPlayers, aiAllowed);
    }

    public void Activate() {
        // Turn on buttons
        foreach(MenuButton mb in buttons) {
            if (mb.IsInteractable) {
                mb.isReady = true;
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
