using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if(!_active && gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if (InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Cancel();
        }
    }

    public override void Activate() {
        base.Activate();

        gameObject.SetActive(true);
        _active = true;

        // Set YES button to selected
        yesButton.Highlight();
    }

    public void Exit() {
        if (returnMenu == "Village") {
            _gameManager.VillageButton();
        } else if (returnMenu == "LocalPlay") {
            // Go back to local play menu
            _gameManager.LocalPlayButton();
        } else {
            // default to village i guess
            _gameManager.VillageButton();
        }
    }
    public void Cancel() {
        base.Deactivate();

        // Close the menu
        gameObject.SetActive(false);
        _active = false;
    }
}
