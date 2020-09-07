﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMenu : Menu {
    public MenuButton yesButton;

    public string returnMenu;

    bool _active;

    protected override void Awake() {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        _gameManager = FindObjectOfType<GameManager>();

        if(!_active && menuObj.activeSelf) {
            menuObj.SetActive(false);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void CheckInput() {
        base.CheckInput();

        if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Cancel();
        }
    }

    public override void Activate() {
        base.Activate();

        _active = true;

        // Set YES button to selected
        yesButton.Highlight();
    }

    public void Exit() {
        if (returnMenu == "Village") {
            _gameManager.VillageButton();
        } else if (returnMenu == "LocalPlay") {
            // For the demo just skip over the local play scene
            if (_gameManager.demoMode) {
                _gameManager.VillageButton();
            } else {
                // Go back to local play menu
                _gameManager.LocalPlayButton();
            }
        } else if(returnMenu == "OnlineLobby") {
            SceneManager.LoadScene("OnlineLobby");
        } else {
            // default to village i guess
            _gameManager.VillageButton();
        }
    }
    public void Cancel() {
        base.Deactivate();

        StartCoroutine("DeactivateLater");
    }

    // Attempting to avoid input overflow
    IEnumerator DeactivateLater() {
        yield return null;

        // Close the menu
        _active = false;
    }
}
