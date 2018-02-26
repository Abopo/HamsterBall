using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour {
    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.isOnline = false;

        InitPlayerPrefs();
    }

    void InitPlayerPrefs() {
        // TODO: Remove for final build
        PlayerPrefs.SetInt("FirstTimePlaying", 0);

        if(PlayerPrefs.GetInt("FirstTimePlaying") == 0) {
            PlayerPrefs.SetString("StoryProgress", "1-1"); // How far into the story the player is (used to lock/unlock story levels)
            PlayerPrefs.SetString("StoryPos", "1-1"); // Last place in the story the player was on (used to position the selector in the story select scene)

            // Highscores

            // World 1
            PlayerPrefs.SetInt("1-1Highscore", 0);
            PlayerPrefs.SetInt("1-2Highscore", 0);
            PlayerPrefs.SetInt("1-3Highscore", 0);
            PlayerPrefs.SetInt("1-4Highscore", 0);
            PlayerPrefs.SetInt("1-5Highscore", 0);
            PlayerPrefs.SetInt("1-6Highscore", 0);
            // World 2
            PlayerPrefs.SetInt("2-1Highscore", 0);
            PlayerPrefs.SetInt("2-2Highscore", 0);
            PlayerPrefs.SetInt("2-3Highscore", 0);
            PlayerPrefs.SetInt("2-4Highscore", 0);
            PlayerPrefs.SetInt("2-5Highscore", 0);
            PlayerPrefs.SetInt("2-6Highscore", 0);

            PlayerPrefs.SetInt("FirstTimePlaying", 1);
        }
    }

    // Update is called once per frame
    void Update () {
    }

    public void LoadStoryMode() {
        _gameManager.isOnline = false;
        SceneManager.LoadScene("StorySelect");
    }

    public void LoadCharacterSelect() {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void LoadOnline() {
        _gameManager.isOnline = true;
        SceneManager.LoadScene("OnlineLobby");
    }

    public void LoadOptions() {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
