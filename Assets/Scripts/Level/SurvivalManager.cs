using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalManager : MonoBehaviour {
    float _addLineTime = 20f;
    float _addLineTimer = 0f;
    int _linesAdded;

    BubbleManager _bubbleManager;
    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _bubbleManager = GetComponent<BubbleManager>();
        _gameManager = FindObjectOfType<GameManager>();

        // Adjust addLineTime based on the number of players in the game
        _addLineTime = 60f - _gameManager.numPlayers * 10;
        _addLineTime = 2f - _gameManager.numPlayers * 0.25f;
	}

    // Update is called once per frame
    void Update() {
        if(_gameManager.gameIsOver) {
            return;
        }

        _addLineTimer += Time.deltaTime;

        if(_linesAdded >= 26f) {
            _bubbleManager.StartShaking();
        }

        if (_addLineTimer >= _addLineTime) {
            _addLineTimer = 0f;
            _linesAdded++;

            //_bubbleManager.TryAddLine();

            if (_linesAdded >= _bubbleManager.HamsterMeter.MeterSize) {
                _bubbleManager.StopShaking();

                _addLineTime -= 0.1f;
                if (_addLineTime < 0.1f) {
                    _addLineTime = 0.1f;
                }

                _linesAdded = 0;
            }

            _bubbleManager.HamsterMeter.IncreaseStock(1);
        }
    }
}
