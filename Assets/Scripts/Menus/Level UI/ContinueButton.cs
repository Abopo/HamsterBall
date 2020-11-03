using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour {

    //GameManager _gameManager;
    //LevelManager _levelManager;

	// Use this for initialization
	void Start () {
        //_gameManager = GameManager.instance;
        //_levelManager = FindObjectOfType<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    public void ContinueToNextLevel() {
        _gameManager.Unpause();
        if (_gameManager.nextLevel != "") {
            // Load the next level
            _levelManager.GetComponent<BoardLoader>().ReadBoardSetup(_gameManager.nextLevel);
        } else if (_gameManager.nextCutscene != "") {
            // If we are in a verus stage
            if(_gameManager.gameMode == GAME_MODE.MP_VERSUS) {
                // Make sure the entire set is done before playing the cutscene
                if (_levelManager.setOver) {
                    // Load a cutscene
                    CutsceneManager.fileToLoad = _gameManager.nextCutscene;
                    SceneManager.LoadScene("Cutscene");
                // Otherwise, play the next game in the set
                } else {
                    _levelManager.NextGame();
                }
            } else {
                // Load a cutscene
                CutsceneManager.fileToLoad = _gameManager.nextCutscene;
                SceneManager.LoadScene("Cutscene");
            }
        } else {
            // It's probably a versus match so
            // Replay the current level
            _levelManager.NextGame();
        }
    }
    */
}
