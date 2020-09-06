using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScreen : MonoBehaviour {

    GameObject _menuObj;

    GameManager _gameManager;

    static bool shown = false;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _menuObj = transform.GetChild(0).gameObject;
    }
    // Start is called before the first frame update
    void Start() {
        if (_gameManager.demoMode && !shown) {
            // Pause the game
            _gameManager.FullPause();

            // Show the menu
            _menuObj.SetActive(true);

            shown = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if(InputState.GetButtonOnAnyControllerPressed("Submit") || InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            _gameManager.Unpause();
            _menuObj.SetActive(false);
        }
    }
}
