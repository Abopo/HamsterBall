using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MenuButton, IScrollHandler {

    public GameObject outImage;

    public SuperTextMesh itemNameText;
    public SuperTextMesh itemCostText;
    public Image icon;

    public float worldY;

    ShopMenu _shopMenu;
    ShopPage _parentPage;
    ScrollRect _mainScroll;
    ItemSprite _itemSprite;
    ConfirmPurchaseMenu _confirmMenu;

    protected ItemInfo _itemInfo;
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
        _parentPage = GetComponentInParent<ShopPage>();
        _mainScroll = FindObjectOfType<ScrollRect>();
        _itemSprite = FindObjectOfType<ItemSprite>();
        _confirmMenu = FindObjectOfType<ConfirmPurchaseMenu>();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        itemNameText.text = _itemInfo.itemName;
        itemCostText.text = _itemInfo.price.ToString();

        // Load in item icon
        LoadIcon();
    }

    protected virtual void LoadIcon() {

    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        worldY = transform.position.y;
    }

    public override void Highlight() {
        base.Highlight();

        // Tell shop menu that we are selected
        _parentPage.SetCurItem(this);

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
        // Display the large item image
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
