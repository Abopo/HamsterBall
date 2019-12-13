using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class TitleScreen : MonoBehaviour {

    InputState _input;
    GameManager _gameManager;

    private void Awake() {
        _input = new InputState(0);
    }

    // Use this for initialization
    void Start () {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update () {
        _input.GetInput();
        if(_input.AnyButtonPressed()) {
            if (_gameManager.demoMode) {
                // Since it's demo mode make sure the correct stages are unlocked
                PlayerPrefs.SetInt("Forest", 1);
                PlayerPrefs.SetInt("Mountain", 1);
                PlayerPrefs.SetInt("Beach", 1);
                PlayerPrefs.SetInt("City", 0);
                PlayerPrefs.SetInt("Sewers", 0);
                PlayerPrefs.SetInt("Corporation", 0);
                PlayerPrefs.SetInt("Laboratory", 0);
                PlayerPrefs.SetInt("Airship", 0);

                SceneManager.LoadScene("PlayableCharacterSelect");
            } else {
                // Load village
                SceneManager.LoadScene("VillageScene");
            }
        }
	}
}
