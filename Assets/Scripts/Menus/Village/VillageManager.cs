using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class VillageManager : MonoBehaviour {

    PauseMenu _pauseMenu;
    GameManager _gameManager;

    Player _player;

	// Use this for initialization
	void Start () {
        _pauseMenu = FindObjectOfType<PauseMenu>();

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isSinglePlayer = true;

        _player = ReInput.players.GetPlayer(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(_player.GetButtonDown("Start")) {
            _pauseMenu.Activate(0);
        }
	}
}
