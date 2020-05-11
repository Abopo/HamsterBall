using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPurchaseMenu : Menu {
    public GameObject menuObject;
    public MenuButton yesButton;

    bool _active;

    ShopMenu _shopMenu;

    protected override void Awake() {
        base.Awake();

        _shopMenu = FindObjectOfType<ShopMenu>();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        _gameManager = FindObjectOfType<GameManager>();

        if (!_active && menuObject.activeSelf) {
            menuObject.SetActive(false);
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

        menuObject.SetActive(true);
        _active = true;

        // Set YES button to selected
        yesButton.Highlight();
    }

    public void Purchase() {
        _shopMenu.PurchaseCurItem();

        Cancel();
    }

    public void Cancel() {
        base.Deactivate();

        // Close the menu
        menuObject.SetActive(false);
        _active = false;
    }
}
