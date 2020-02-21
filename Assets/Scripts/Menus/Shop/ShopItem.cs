using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MenuButton, IScrollHandler {

    public Sprite itemSprite;
    public string itemName;
    public string itemDescription;
    public bool purchased;

    ShopMenu _shopMenu;
    ScrollRect _mainScroll;

    protected override void Awake() {
        base.Awake();

        _shopMenu = FindObjectOfType<ShopMenu>();
        _mainScroll = FindObjectOfType<ScrollRect>();
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

        // Change item description
        _shopMenu.itemDescription.text = itemDescription;
    }

    public void MouseHighlight() {
        base.Highlight();

        // Display item image

        // Change item description
        _shopMenu.itemDescription.text = itemDescription;

        // Do a little shake?

    }

    protected override void Select() {
        base.Select();

        // Open up purchase confirmation window

    }

    // Force scroll even when pointer is over buttons
    public void OnScroll(PointerEventData data) {
        _mainScroll.OnScroll(data);
    }
}
