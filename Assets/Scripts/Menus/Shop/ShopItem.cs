using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MenuButton, IScrollHandler {

    public GameObject outImage;

    ShopMenu _shopMenu;
    ScrollRect _mainScroll;
    ItemSprite _itemSprite;
    ConfirmPurchaseMenu _confirmMenu;

    ItemInfo _itemInfo;
    public ItemInfo ItemInfo {
        get { return _itemInfo; }

        set {
            _itemInfo = value;

            if (ItemInfo.purchased) {
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

        GetComponentInChildren<SuperTextMesh>().text = _itemInfo.itemName;
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
        _shopMenu.itemCost.text = _itemInfo.price.ToString();

        // Change item description
        _shopMenu.itemDescription.text = _itemInfo.description;
    }

    public void TryPurchase() {
        if (!_itemInfo.purchased && _shopMenu.playerCurrency >= _itemInfo.price) {
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

    public void SetPurchased(bool purchased) {
        _itemInfo.purchased = purchased;

        if (_itemInfo.purchased) {
            outImage.SetActive(true);
        } else {
            outImage.SetActive(false);
        }
    }
}
