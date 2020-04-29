using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public int team = 0;

    public SuperTextMesh _totalScoreText;
    public SuperTextMesh _scorePoolText;

    int _totalScore; // the current total score
    int _scorePool; // a pool of score to add after a second

    float _poolTime = 1.5f;
    float _poolTimer = 0f;

    public int TotalScore {
        get { return _totalScore; }
    }

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();

        _totalScore = _gameManager.scoreOverflow;
        _totalScoreText.text = _totalScore.ToString();
        _scorePoolText.text = "";

        _poolTimer = 0f;
    }

    // Update is called once per frame
    void Update () {
		if(_scorePool > 0) {
            _poolTimer += Time.deltaTime;
            if(_poolTimer >= _poolTime) {
                CombineScore();
            }
        }
	}

    // Combines the scorePool into the totalScore
    public void CombineScore() {
        _totalScore += _scorePool;
        _totalScoreText.text = _totalScore.ToString();

        _scorePool = 0;
        _scorePoolText.text = "";

        _poolTimer = 0;
    }

    public void IncreaseScore(int incScore) {
        _scorePool += incScore;

        // change score text
        _scorePoolText.text = "+" + _scorePool.ToString();

        _poolTimer = 0;
    }
}
