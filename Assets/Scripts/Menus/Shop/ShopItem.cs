using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MenuButton {

    public Sprite itemSprite;
    public string itemName;
    public string itemDescription;
    public bool purchased;

    ShopMenu _shopMenu;

    protected override void Awake() {
        base.Awake();

        _shopMenu = FindObjectOfType<ShopMenu>();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        GetComponentInChildren<SuperTextMesh>().text = itemName;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

    }

    public override void Highlight() {
        base.Highlight();

        // Tell shop menu that we are selected
        _shopMenu.SetCurItem(this);

        // Display item image

    }

    protected override void Select() {
        base.Select();

        // Open up purchase confirmation window

    }
}
