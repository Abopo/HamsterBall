using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : Menu {

    public RectTransform content;
    public Image itemImage;

    Object _shopItemObj;
    List<ShopItem> _items = new List<ShopItem>();

    ShopItem _curItem;

    protected override void Awake() {
        base.Awake();

        _shopItemObj = Resources.Load("Prefabs/Menus/Shop/ShopItem");
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        // Figure out what's available in the shop
        TextAsset palettes = Resources.Load<TextAsset>("Text/Shop/CharacterPalettes");
        string[] linesFromFile = palettes.text.Split("\n"[0]);
        int i = 0;
        foreach (string line in linesFromFile) {
            linesFromFile[i] = line.Replace("\r", "");
            i++;
        }

        int index = 0;
        string readLine = linesFromFile[index++];
        bool[] itemData;
        GameObject tempItem;

        while (readLine != "End") {
            if(readLine.Length < 5) {
                readLine = linesFromFile[index++];
                continue;
            }

            // Check if the item is available for purchase
            itemData = ES3.Load<bool[]>(readLine);
            if(itemData[0]) {
                // Create the item object
                tempItem = Instantiate(_shopItemObj, content) as GameObject;
                ((RectTransform)tempItem.transform).anchoredPosition = new Vector3(0, -30 - (50 * _items.Count));
                tempItem.GetComponent<ShopItem>().itemName = readLine;
                // If the item has already been purchased
                tempItem.GetComponent<ShopItem>().purchased = itemData[1];
                _items.Add(tempItem.GetComponent<ShopItem>());
            }

            readLine = linesFromFile[index++];
        }

        // Create the shop items
        //GameObject tempItem;
        //for (int i = 0; i < 15; ++i) {
        //    tempItem = Instantiate(_shopItemObj, content) as GameObject;
        //    ((RectTransform)tempItem.transform).anchoredPosition = new Vector3(0, -30 - (50 * i));
        //    _items.Add(tempItem.GetComponent<ShopItem>());
        //}

        // Run back through the items to properly set their adjacent options
        for (i = 0; i < _items.Count; ++i) {
            _items[i].FindAdjOptions();
        }
        // For some reason the bottom items adj options aren't accurate
        _items[_items.Count-1].adjOptions[1] = null;

        // Highlight the first item
        _items[0].isFirstSelection = true;

        // Properly size the content
        if (_items.Count > 5) {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 305 + (_items.Count - 5) * 45);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

    }

    public void SetCurItem(ShopItem item) {
        _curItem = item;

        // Scroll down the content based on which item is selected
        int index = _items.IndexOf(item);
        if (index > 1 && index < _items.Count - 2) {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, (index-2) * 50);
        }
    }
}
