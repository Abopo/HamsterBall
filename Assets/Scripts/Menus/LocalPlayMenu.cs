using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalPlayMenu : MonoBehaviour {

    public PlayerCountSelectMenu pcsMenu;

    public MenuOption[] buttons;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StandardVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);

        OpenPlayerCountSelectMenu(4, 2, true);
        //if (_gameManager.demoMode) {
            //SceneManager.LoadScene("DemoCharacterSelect");
        //} else {
        //    SceneManager.LoadScene("CharacterSelect");
        //}
    }

    public void PartyVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_PARTY);
        OpenPlayerCountSelectMenu(4, 2, true);
        //SceneManager.LoadScene("DemoCharacterSelect");
    }

    public void Survival() {
        _gameManager.SetGameMode(GAME_MODE.SURVIVAL);
        OpenPlayerCountSelectMenu(2, 1, false);
        //SceneManager.LoadScene("DemoCharacterSelect");
    }

    public void TeamSurvival() {
        _gameManager.SetGameMode(GAME_MODE.TEAMSURVIVAL);
        OpenPlayerCountSelectMenu(4, 2, false);
    }

    public void PuzzleChallenge() {
        _gameManager.SetGameMode(GAME_MODE.SP_CLEAR);
        OpenPlayerCountSelectMenu(2, 1, false);
        //SceneManager.LoadScene("DemoCharacterSelect");
    }

    public void OpenPlayerCountSelectMenu(int maxPlayers, int minPlayers, bool aiAllowed) {
        pcsMenu.Activate(maxPlayers, minPlayers, aiAllowed);
        Deactivate();
    }

    public void Activate() {
        // Turn on buttons
        foreach(MenuOption mb in buttons) {
            mb.isReady = true;
        }
    }
    void Deactivate() {
        // Turn off buttons
        foreach (MenuOption mb in buttons) {
            mb.isReady = false;
        }
    }
}
