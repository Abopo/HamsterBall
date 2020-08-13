using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopPage : Menu {

    public RectTransform content;

    public int colCount;

    public float _scrollTo;
    float _scrollSpeed = 1000f;

    float _itemYSpacing = 100f;

    protected GameObject _shopItemObj;
    protected List<ShopItem> _items = new List<ShopItem>();

    protected ShopItem _curItem;
    public ShopItem CurItem {
        get { return _curItem; }
    }

    protected Tab _tab;

    protected ShopMenu _shopMenu;

    protected override void Awake() {
        base.Awake();

        if (_shopItemObj == null) {
            _shopItemObj = Resources.Load("Prefabs/Menus/Shop/ShopItem") as GameObject;
        }

        _tab = GetComponentInChildren<Tab>();

        _shopMenu = FindObjectOfType<ShopMenu>();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        // Run back through the items to properly set their adjacent options
        for (int i = 0; i < _items.Count; ++i) {
            _items[i].FindAdjOptions();
        }
        // For some reason the bottom items adj options aren't accurate
        _items[_items.Count - 1].adjOptions[1] = null;

        // Highlight the first item
        _items[0].isFirstSelection = true;

        // Properly size the content
        if (_items.Count > 9) {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 305 + (_items.Count - 9) * _itemYSpacing);
        }

        // If we don't have focus don't show our content
        if(!hasFocus) {
            HideContent();
        } else {
            // Make sure we can see our content
            ShowContent();
        }
    }

    protected void CreateShopItems(List<ItemInfo> shopData) {
        GameObject tempItem;
        bool[] itemData;

        // Create the objects
        foreach (ItemInfo iInfo in shopData) {
            itemData = ES3.Load(iInfo.itemName, new bool[0]);

            // if this item is unlocked
            if (itemData[0]) {
                tempItem = Instantiate(_shopItemObj, content) as GameObject;
                tempItem.GetComponent<ShopItem>().ItemInfo = iInfo;
                tempItem.GetComponent<ShopItem>().SetPurchased(itemData[1]);
                tempItem.GetComponent<ShopItem>().SetParentMenu(this);
                _items.Add(tempItem.GetComponent<ShopItem>());
            }
        }

        // Position the objects
        if(colCount%2 == 0) {
            PositionItemsEven();
        } else {
            PositionItemsOdd();
        }

        StartCoroutine("SetUIWrap");
    }

    void PositionItemsEven() {
        float xStart = -content.rect.width / 2;
        int colHalf = Mathf.FloorToInt(colCount / 2);
        int xOffset = 1;
        float xSpacing = content.rect.width / (colCount + 1);
        int yOffset = 0;

        foreach (ShopItem item in _items) {
            ((RectTransform)item.transform).anchoredPosition = new Vector3(xStart + xSpacing * xOffset, -10 - (_itemYSpacing * yOffset));

            xOffset++;
            if (xOffset > colCount) {
                xOffset = 1;
                yOffset++;
            }
        }
    }

    void PositionItemsOdd() {
        int colHalf = Mathf.FloorToInt(colCount / 2);
        int xOffset = -colHalf;
        float xSpacing = (content.rect.width + content.rect.width * 0.28f) / (colCount + 1);
        int yOffset = 0;

        foreach (ShopItem item in _items) {
            ((RectTransform)item.transform).anchoredPosition = new Vector3(xSpacing * xOffset, -10 - (_itemYSpacing * yOffset));

            xOffset++;
            if (xOffset > colHalf) {
                xOffset = -colHalf;
                yOffset++;
            }
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        // Smooth scroll?
        if (content.anchoredPosition.y != _scrollTo) {
            // Scroll towards the position
            if (content.anchoredPosition.y > _scrollTo) {
                // Scroll down
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y < _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            } else if (content.anchoredPosition.y < _scrollTo) {
                // Scroll up
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y + (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y > _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            }
        }
    }

    public override void TakeFocus() {
        base.TakeFocus();

        ShowContent();
    }

    void ShowContent() {
        // Turn on content
        content.gameObject.SetActive(true);

        _tab.Select();
    }

    public void HideContent() {
        // Turn off content
        content.gameObject.SetActive(false);

        _tab.Deselect();
    }

    public void SetCurItem(ShopItem item) {
        _curItem = item;

        // Scroll down the content based on which item is selected

        // If the item is out of the screen upwards
        if (item.worldY > 4f) {
            // scroll up one row
            _scrollTo -= 90;
            if (_scrollTo < 0) {
                _scrollTo = 0;
            }
        }
        // If the item is out of the screen downwards
        if (item.worldY < -6) {
            // scroll down one row
            _scrollTo += 90;
        }

        // If the item is in the top row
        if(_items.IndexOf(item) < 3) {
            // Turn off the up arrow

        } else {
            // Turn on the up arrow

        }
        // If the item is in the bottom row
        // TODO: this is actually a bit more complicated than just the last indexes
        if(_items.IndexOf(item) > _items.Count-4) {
            // Turn off the down arrow

        } else {
            // Turn on the down arrow

        }
    }

    public virtual void PurchaseCurItem() {
        // Change item to purchased
        _curItem.SetPurchased(true);

        // Unlock the item in the game data
        UnlockItem();

        // Save that this item has been purchased
        bool[] itemData = ES3.Load(_curItem.ItemInfo.itemName, new bool[0]);
        itemData[1] = true;
        ES3.Save<bool[]>(_curItem.ItemInfo.itemName, itemData);
    }

    protected virtual void UnlockItem() {

    }

    IEnumerator SetUIWrap() {
        yield return new WaitForSeconds(0.1f);

        int offset = colCount - 1;

        if(offset == 0) {
            // we only have 1 column so this is unnecessary
            yield break;
        }

        // Manually setup ui navigation wrap
        for (int i = 0; i < _items.Count; ++i) {
            // If this item has no right side adjOption
            if (_items[i].adjOptions[0] == null) {
                // Assign the option two behind
                if (i >= offset) {
                    _items[i].adjOptions[0] = _items[i - offset];
                }
            }
            // If this item has no left side adjOption
            if (_items[i].adjOptions[2] == null) {
                // Assign the option two ahead
                if (i < _items.Count - offset) {
                    _items[i].adjOptions[2] = _items[i + offset];
                }
            }
        }
    }
}
