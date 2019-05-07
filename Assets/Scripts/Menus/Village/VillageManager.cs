using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class VillageManager : MonoBehaviour {
    public bool resetPrefs;

    PauseMenu _pauseMenu;
    GameManager _gameManager;

    Player _player;

	// Use this for initialization
	void Start () {
        _pauseMenu = FindObjectOfType<PauseMenu>();

        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.isSinglePlayer = true;

        _player = ReInput.players.GetPlayer(0);

        InitPlayerPrefs();
	}

    void InitPlayerPrefs() {
        if (resetPrefs) {
            // TODO: Remove for final build
            PlayerPrefs.SetInt("FirstTimePlaying", 0);
        }
        //PlayerPrefs.SetInt("FirstTimePlaying", 0);

        // These prefs reset on every game close/launch.
        PlayerPrefs.SetInt("Player1Character", (int)CHARACTERCOLORS.BOY1);
        PlayerPrefs.SetInt("Player2Character", (int)CHARACTERCOLORS.GIRL1);

        // These prefs are only reset on the first launch of the game.
        if (PlayerPrefs.GetInt("FirstTimePlaying", 0) == 0) {
            PlayerPrefs.SetString("StoryProgress", "2-3"); // How far into the story the player is (used to lock/unlock story levels)-
            PlayerPrefs.SetString("StoryPos", "1-3"); // Last place in the story the player was on (used to position the selector in the story select scene)

            // Stages
            PlayerPrefs.SetInt("Forest", 1);
            PlayerPrefs.SetInt("Mountain", 1);
            PlayerPrefs.SetInt("Beach", 1);
            PlayerPrefs.SetInt("City2", 0);
            PlayerPrefs.SetInt("Sewers", 0);
            PlayerPrefs.SetInt("Laboratory", 0);
            PlayerPrefs.SetInt("Dark Forest", 0);
            PlayerPrefs.SetInt("Airship", 0);

            // Highscores

            // World 1
            PlayerPrefs.SetInt("1-0HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-0HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-1HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-1HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-2HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-2HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-3HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-3HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-4HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-4HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-5HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-5HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-6HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-6HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-7HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-7HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-8HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-8HighscoreCoop", 0);
            PlayerPrefs.SetInt("1-9HighscoreSolo", 0);
            PlayerPrefs.SetInt("1-9HighscoreCoop", 0);
            // World 2
            PlayerPrefs.SetInt("2-0HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-0HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-1HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-1HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-2HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-2HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-3HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-3HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-4HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-4HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-5HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-5HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-6HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-6HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-7HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-7HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-8HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-8HighscoreCoop", 0);
            PlayerPrefs.SetInt("2-9HighscoreSolo", 0);
            PlayerPrefs.SetInt("2-9HighscoreCoop", 0);
            // World 3
            PlayerPrefs.SetInt("3-0HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-0HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-1HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-1HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-2HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-2HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-3HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-3HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-4HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-4HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-5HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-5HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-6HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-6HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-7HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-7HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-8HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-8HighscoreCoop", 0);
            PlayerPrefs.SetInt("3-9HighscoreSolo", 0);
            PlayerPrefs.SetInt("3-9HighscoreCoop", 0);

            PlayerPrefs.SetInt("FirstTimePlaying", 1);
        }
    }

    // Update is called once per frame
    void Update () {
		if(_player.GetButtonDown("Pause")) {
            _pauseMenu.Activate(0);
        }
	}
}
