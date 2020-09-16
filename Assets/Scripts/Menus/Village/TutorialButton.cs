using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour {

    PlayerController _player;
    PlayerManager _playerManager;

    // Start is called before the first frame update
    void Start() {
        _player = FindObjectOfType<PlayerController>();
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update() {
    }

    public void StartTutorial() {
        _player = FindObjectOfType<PlayerController>();

        // Put the player and an ai into the player manager
        PlayerInfo player1 = new PlayerInfo(), aiplayer = new PlayerInfo();
        player1.playerNum = 0;
        player1.team = 0;
        player1.charaInfo.name = CHARACTERS.BOY;
        player1.charaInfo.color = 1;
        player1.charaInfo.team = 0;
        _playerManager.AddPlayer(player1);

        aiplayer.playerNum = 1;
        aiplayer.team = 1;
        aiplayer.charaInfo.name = CHARACTERS.GIRL;
        aiplayer.charaInfo.color = 1;
        aiplayer.charaInfo.team = 1;
        aiplayer.isAI = true;
        _playerManager.AddPlayer(aiplayer);

        // Set up the level
        GameManager _gameManager = _playerManager.GetComponent<GameManager>();
        _gameManager.selectedBoard = BOARDS.FOREST;
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
        _gameManager.prevMenu = MENU.VILLAGE;

        // Set the welcome screen to seen?
        WelcomeScreen.shown = true;

        // Load the tutorial scene
        SceneManager.LoadScene("OpeningTutorial");
    }
}
