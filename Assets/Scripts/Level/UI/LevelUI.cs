using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {
    public Text levelTimer;

    GameMarker[] _gameMarkers;

    GameManager _gameManager;
    LevelManager _levelManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        _levelManager = FindObjectOfType<LevelManager>();

        SetupGameMarkers();
	}

    public void SetupGameMarkers() {
        _gameMarkers = GetComponentsInChildren<GameMarker>();

        // Clear all the game markers first
        foreach (GameMarker gM in _gameMarkers) {
            gM.FillOut();
        }

        if (_gameManager.isSinglePlayer || _gameManager.gameMode == GAME_MODE.TEAMSURVIVAL) {
            foreach(GameMarker gM in _gameMarkers) {
                gM.gameObject.SetActive(false);
            }
        } else {
            if (_gameManager.leftTeamGames > 0) {
                FillInGameMarker(0);
            }
            if (_gameManager.rightTeamGames > 0) {
                FillInGameMarker(1);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (levelTimer != null) {
            int seconds = Mathf.FloorToInt(_levelManager.LevelTimer % 60);
            int minutes = Mathf.FloorToInt(_levelManager.LevelTimer / 60);
            levelTimer.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
	}

    public void FillInGameMarker(int team) {
        //Debug.Log("Should fill in");
        foreach(GameMarker gM in _gameMarkers) {
            if(gM.team == team && !gM.isFilledIn) {
                gM.FillIn();
                break;
            }
        }
    }
}
