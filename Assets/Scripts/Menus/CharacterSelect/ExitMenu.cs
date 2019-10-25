using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenu : MonoBehaviour {
    public MenuButton yesButton;

    CharacterSelect _charaSelect;
    GameManager _gameManager;

    private void Awake() {
    }

    // Start is called before the first frame update
    void Start() {
        _charaSelect = FindObjectOfType<CharacterSelect>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Cancel();
        }
    }

    public void Activate() {
        gameObject.SetActive(true);

        // Set YES button to selected
        yesButton.Highlight();
    }

    public void Exit() {
        // Go back to local play menu
        _gameManager.LocalPlayButton();
    }
    public void Cancel() {
        // Close the menu
        gameObject.SetActive(false);
        _charaSelect.Reactivate();
    }
}
