using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.IO;

public class MainMenu : MonoBehaviour {
    public bool resetPrefs;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.isOnline = false;
    }

    void InitDirectories() {
        System.IO.Directory.CreateDirectory(Application.dataPath + "/Created Boards");
    }

    // Update is called once per frame
    void Update () {
    }

    public void LoadStoryMode() {
        _gameManager.isOnline = false;
        SceneManager.LoadScene("StorySelect");
    }

    public void LoadLocalMultiplayer() {
        SceneManager.LoadScene("LocalPlay");
    }

    public void LoadOnline() {
        _gameManager.isOnline = true;
        _gameManager.SetGameMode(GAME_MODE.MP_VERSUS);
        SceneManager.LoadScene("OnlineLobby");
    }

    public void LoadBoardEditor() {
        SceneManager.LoadScene("BoardEditor");
    }

    public void LoadOptions() {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
