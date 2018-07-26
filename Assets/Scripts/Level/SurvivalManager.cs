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
	}

    // Update is called once per frame
    void Update() {
        if(_gameManager.gameIsOver) {
            return;
        }

        _addLineTimer += Time.deltaTime;

        if(_addLineTime - _addLineTimer <= 5f) {
            _bubbleManager.StartShaking();
        }

        if (_addLineTimer >= _addLineTime) {
            _addLineTimer = 0f;
            _linesAdded++;

            //_bubbleManager.AddLine();
            _bubbleManager.TryAddLine();
            _bubbleManager.StopShaking();

            if (_linesAdded >= 4) {
                _addLineTime -= 1f;
                if (_addLineTime < 5f) {
                    _addLineTime = 5f;
                }

                _linesAdded = 0;
            }
        }
	}
}
