using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MenuButton, IScrollHandler {
    public string itemName;
    public int itemCost;
    public string itemDescription;
    public Sprite itemSprite;
    public GameObject outImage;


    ShopMenu _shopMenu;
    ScrollRect _mainScroll;
    ItemSprite _itemSprite;
    ConfirmPurchaseMenu _confirmMenu;

    bool _purchased;
    public bool Purchased {
        get { return _purchased; }

        set {
            _purchased = value;

            if (_purchased) {
                outImage.SetActive(true);
            } else {
                outImage.SetActive(false);
            }
        }
    }

    protected override void Awake() {
        base.Awake();

        _shopMenu = FindObjectOfType<ShopMenu>();
        _mainScroll = FindObjectOfType<ScrollRect>();
        _itemSprite = FindObjectOfType<ItemSprite>();
        _confirmMenu = FindObjectOfType<ConfirmPurchaseMenu>();
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

        SetItemData();
    }

    public void MouseHighlight() {
        base.Highlight();

        if (IsReady) {
            SetItemData();

            // Do a little shake?

        }
    }

    void SetItemData() {
        // Display item image
        _itemSprite.SetItem(this);

        // Change item cost
        _shopMenu.itemCost.text = itemCost.ToString();

        // Change item description
        _shopMenu.itemDescription.text = itemDescription;
    }

    public void TryPurchase() {
        if (!_purchased && _shopMenu.playerCurrency >= itemCost) {
            // Open up purchase confirmation window
            _confirmMenu.Activate();

            PlaySelectSound();
        } else {
            // Play error sound?

        }
    }

    // Force scroll even when pointer is over buttons
    public void OnScroll(PointerEventData data) {
        _mainScroll.OnScroll(data);
    }
}
