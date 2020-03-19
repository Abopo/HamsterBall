using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : Menu {

    public RectTransform content;
    public Image itemImage;
    public SuperTextMesh itemDescription;

    float _scrollTo;
    float _scrollSpeed = 1000f;

    Object _shopItemObj;
    List<ShopItem> _items = new List<ShopItem>();

    ShopItem _curItem;

    ExitMenu _exitMenu;

    protected override void Awake() {
        base.Awake();

        _shopItemObj = Resources.Load("Prefabs/Menus/Shop/ShopItem");

        _exitMenu = FindObjectOfType<ExitMenu>();
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
            if(readLine == "") {
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

                // Get the item description
                readLine = linesFromFile[index++];
                tempItem.GetComponent<ShopItem>().itemDescription = readLine;
            }

            // Get to the next item
            while (readLine != "") {
                readLine = linesFromFile[index++];
            }
            readLine = linesFromFile[index++];
        }

        // Run back through the items to properly set their adjacent options
        for (i = 0; i < _items.Count; ++i) {
            _items[i].FindAdjOptions();
        }
        // For some reason the bottom items adj options aren't accurate
        _items[_items.Count-1].adjOptions[1] = null;

        // Highlight the first item
        _items[0].isFirstSelection = true;
        itemDescription.text = _items[0].itemDescription;

        // Properly size the content
        if (_items.Count > 5) {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 305 + (_items.Count - 5) * 45);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        // Smooth scroll?
        if(content.anchoredPosition.y != _scrollTo) {
            // Scroll towards the position
            if(content.anchoredPosition.y > _scrollTo) {
                // Scroll down
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y < _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            } else if(content.anchoredPosition.y < _scrollTo) {
                // Scroll up
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y + (_scrollSpeed * Time.deltaTime));
                // If we go too far
                if (content.anchoredPosition.y > _scrollTo) {
                    // just set to
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, _scrollTo);
                }
            }
        }

        if(InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            _exitMenu.Activate();
        }
    }

    public void SetCurItem(ShopItem item) {
        _curItem = item;

        // Scroll down the content based on which item is selected
        int index = _items.IndexOf(item);
        if (index > 1 && index < _items.Count - 2) {
            _scrollTo = (index - 2) * 50;
            //content.anchoredPosition = new Vector2(content.anchoredPosition.x, (index-2) * 50);
        }
    }
}
