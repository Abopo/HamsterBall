using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScreen : MonoBehaviour {

    GameObject _menuObj;

    GameManager _gameManager;

    public static bool shown = false;

    bool _frameskip = true;

    private void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        _menuObj = transform.GetChild(0).gameObject;
    }
    // Start is called before the first frame update
    void Start() {
        if (_gameManager.demoMode && !shown) {
            FindObjectOfType<VillageManager>().villageStart.AddListener(OnVillageStart);
        }
    }
    void OnVillageStart() {
        // Pause the game
        // TODO: just stop player movement instead of pausing?
        _gameManager.FullPause();

        // Show the menu
        _menuObj.SetActive(true);

        _frameskip = true;
    }

    // Update is called once per frame
    void Update() {
    }

    private void LateUpdate() {
        if (!_frameskip && InputState.GetButtonOnAnyControllerPressed("Submit") || InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Close();
        }

        _frameskip = false;
    }

    public void Close() {
        _gameManager.Unpause();
        _menuObj.SetActive(false);
        shown = true;
    }
}
