using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenu : Menu {
    public MenuButton yesButton;

    protected override void Awake() {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        _gameManager = FindObjectOfType<GameManager>();

        if(gameObject.activeSelf) {
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

        // Set YES button to selected
        yesButton.Highlight();
    }

    public void Exit() {
        // Go back to local play menu
        _gameManager.LocalPlayButton();
    }
    public void Cancel() {
        base.Deactivate();

        // Close the menu
        gameObject.SetActive(false);
    }
}
