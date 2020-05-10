using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : Menu {

    public RectTransform content;
    public Image itemImage;
    public SuperTextMesh itemCost;
    public SuperTextMesh itemDescription;

    public int playerCurrency;
    public SuperTextMesh currencyText;

    ShopData _shopData;

    float _scrollTo;
    float _scrollSpeed = 1000f;

    GameObject _shopItemObj;
    List<ShopItem> _items = new List<ShopItem>();

    ShopItem _curItem;

    ExitMenu _exitMenu;

    protected override void Awake() {
        base.Awake();

        _shopItemObj = Resources.Load("Prefabs/Menus/Shop/ShopItem") as GameObject;

        _exitMenu = FindObjectOfType<ExitMenu>();
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        // Testing out xml loading stuff
        LoadXMLShopData();
        CreateShopItems();

        // Figure out what's available in the shop
        //LoadShopData();

        // Do this now that the item objects are actually made
        GetChildOptions();

        // Run back through the items to properly set their adjacent options
        for (int i = 0; i < _items.Count; ++i) {
            _items[i].FindAdjOptions();
        }
        // For some reason the bottom items adj options aren't accurate
        _items[_items.Count-1].adjOptions[1] = null;

        // Highlight the first item
        _items[0].isFirstSelection = true;
        itemDescription.text = _items[0].ItemInfo.description;

        // Properly size the content
        if (_items.Count > 5) {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 305 + (_items.Count - 5) * 45);
        }

        playerCurrency = ES3.Load<int>("Currency", 100);
        currencyText.text = playerCurrency.ToString();

    }

    void LoadXMLShopData() {
#if UNITY_EDITOR
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "Resources/Text/Shop/ShopPaletteData.xml"));
#else
        _shopData = ShopData.Load(Path.Combine(Application.dataPath, "ShopPaletteData.xml"));
#endif
    }

    void CreateShopItems() {
        GameObject tempItem;
        bool[] itemData;

        // Palettes
        foreach(ItemInfo iInfo in _shopData.paletteData) {
            itemData = ES3.Load<bool[]>(iInfo.itemName, new bool[0]);

            // if this item is unlocked
            if (itemData[0]) {
                tempItem = Instantiate(_shopItemObj, content) as GameObject;
                ((RectTransform)tempItem.transform).anchoredPosition = new Vector3(0, -30 - (50 * _items.Count));
                tempItem.GetComponent<ShopItem>().ItemInfo = iInfo;
                tempItem.GetComponent<ShopItem>().SetPurchased(itemData[1]);
                _items.Add(tempItem.GetComponent<ShopItem>());
            }
        }
    }

    /*
    void LoadShopData() {
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
            if (readLine == "") {
                readLine = linesFromFile[index++];
                continue;
            }

            // Check if the item is available for purchase
            itemData = ES3.Load<bool[]>(readLine);
            if (itemData[0]) {
                // Create the item object
                tempItem = Instantiate(_shopItemObj, content) as GameObject;
                ((RectTransform)tempItem.transform).anchoredPosition = new Vector3(0, -30 - (50 * _items.Count));
                tempItem.GetComponent<ShopItem>().itemName = readLine;
                // If the item has already been purchased
                tempItem.GetComponent<ShopItem>().Purchased =itemData[1];
                _items.Add(tempItem.GetComponent<ShopItem>());

                // Get the item cost
                readLine = linesFromFile[index++];
                tempItem.GetComponent<ShopItem>().itemCost = int.Parse(readLine);

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
    }
    */

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

    protected override void TakeFocus() {
        base.TakeFocus();

        _curItem.Highlight();
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

    public void PurchaseCurItem() {
        // Unlock whatever the item is?
        if(_curItem.ItemInfo.itemName.Contains("Palette")) {
            UnlockPalette();
        }

        // Reduce player currency by the cost
        playerCurrency -= _curItem.ItemInfo.price;
        currencyText.text = playerCurrency.ToString();
        ES3.Save<int>("Currency", playerCurrency);

        // Change item to purchased
        _curItem.SetPurchased(true);

        // Save that this item has been purchased
        //_shopData.Save(Path.Combine(Application.dataPath, "Resources/Text/Shop/ShopPaletteData.xml"));
        bool[] itemData = ES3.Load<bool[]>(_curItem.ItemInfo.itemName, new bool[0]);
        itemData[1] = true;
        ES3.Save<bool[]>(_curItem.ItemInfo.itemName, itemData);
    }

    void UnlockPalette() {
        string paletteString = new String(_curItem.ItemInfo.itemName.Where(Char.IsDigit).ToArray());
        int paletteNum = int.Parse(paletteString);

        bool[] characterPaletteData;

        // Save palette based on the character
        if (_curItem.ItemInfo.itemName.Contains("Kaden")) {
            paletteString = "BoyPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Quinn")) {
            paletteString = "GirlPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Gail")) {
            paletteString = "OwlPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Bexal")) {
            paletteString = "GoatPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Don")) {
            paletteString = "SnailPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Jodi")) {
            paletteString = "LizardPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Rooben")) {
            paletteString = "RoosterPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Carmela")) {
            paletteString = "BatPalettes";
        } else if (_curItem.ItemInfo.itemName.Contains("Lackey")) {
            paletteString = "LackeyPalettes";
        } else {
            Debug.LogError("Character palette data not found");
            return;
        }

        characterPaletteData = ES3.Load<bool[]>(paletteString, new bool[0]);
        characterPaletteData[paletteNum-2] = true;
        ES3.Save<bool[]>(paletteString, characterPaletteData);
    }
}
