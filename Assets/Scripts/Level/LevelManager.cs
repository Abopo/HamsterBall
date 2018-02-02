using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour {
    public PauseMenu pauseMenu;
    public Text marginMultiplierText;

    public float marginMultiplier = 1.00f;

    float _marginTimer = 0;
    float _marginTime = 96f;
    int _initialTargetPoints = 120;
    int _targetPoints;
    int _marginIterations = 0;
    int _prevTargetPoints;

    // Use this for initialization
    void Start () {
        _targetPoints = _initialTargetPoints;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Pause")) {
            // Pause the game
            pauseMenu.Activate();
        }

        _marginTimer += Time.deltaTime;
        if(_marginTimer >= _marginTime && _targetPoints > 1 && _marginIterations < 14) {
            IncreaseMarginMultiplier();
            _marginTime = 16f;
            _marginTimer = 0f;
        }
    }

    void IncreaseMarginMultiplier() {
        int curTargetPoints = _targetPoints;
        if(_marginIterations == 0) {
            _targetPoints = (int)(_initialTargetPoints * 0.75f);
        } else {
            _targetPoints = (int)(_prevTargetPoints / 2);
        }

        _prevTargetPoints = curTargetPoints;
        _marginIterations++;

        marginMultiplier = _initialTargetPoints / (float)_targetPoints;
        marginMultiplierText.text = "x" + marginMultiplier.ToString("0.00");
    }
}