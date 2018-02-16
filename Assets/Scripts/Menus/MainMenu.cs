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
        //PlayerPrefs.SetInt("FirstTimePlaying", 0);

        if(PlayerPrefs.GetInt("FirstTimePlaying") == 0) {
            // Highscores
            PlayerPrefs.SetInt("1-1Highscore", 0);
            PlayerPrefs.SetInt("1-2Highscore", 0);
            PlayerPrefs.SetInt("1-3Highscore", 0);
            PlayerPrefs.SetInt("1-4Highscore", 0);
            PlayerPrefs.SetInt("1-5Highscore", 0);

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
