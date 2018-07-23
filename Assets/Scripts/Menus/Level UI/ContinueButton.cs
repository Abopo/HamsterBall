using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour {

    GameManager _gameManager;
    LevelManager _levelManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ContinueToNextLevel() {
        _gameManager.Unpause();
        if (_gameManager.nextLevel != "") {
            if (_gameManager.nextLevel == "Puzzle Challenge") {
                // Find a new puzzle and load it
                _gameManager.LoadPuzzleChallenge();
            } else {
                // Load the next level
                GetComponent<BoardLoader>().ReadBoardSetup(_gameManager.nextLevel);
            }
        } else if (_gameManager.nextCutscene != "") {
            // Load a cutscene
            CutsceneManager.fileToLoad = _gameManager.nextCutscene;
            SceneManager.LoadScene("Cutscene");
        } else {
            // It's probably a versus match so
            // Replay the current level
            _levelManager.NextGame();
        }
    }
}
