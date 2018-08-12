using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalPlayMenu : MonoBehaviour {

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
        //if (_gameManager.demoMode) {
            SceneManager.LoadScene("DemoCharacterSelect");
        //} else {
        //    SceneManager.LoadScene("CharacterSelect");
        //}
    }

    public void PartyVersus() {
        _gameManager.SetGameMode(GAME_MODE.MP_PARTY);
        SceneManager.LoadScene("CharacterSelect");
    }

    public void Survival() {
        _gameManager.SetGameMode(GAME_MODE.SURVIVAL);
        SceneManager.LoadScene("CharacterSelect");
    }

    public void PuzzleChallenge() {
        _gameManager.SetGameMode(GAME_MODE.SP_CLEAR);
        SceneManager.LoadScene("CharacterSelect");
    }
}
