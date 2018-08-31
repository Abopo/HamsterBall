using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour {

    GameMarker[] _gameMarkers;

    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();

        SetupGameMarkers();
	}

    void SetupGameMarkers() {
        _gameMarkers = GetComponentsInChildren<GameMarker>();

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
		
	}

    public void FillInGameMarker(int team) {
        foreach(GameMarker gM in _gameMarkers) {
            if(gM.team == team && !gM.isFilledIn) {
                gM.FillIn();
                break;
            }
        }
    }
}
